using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xicheng.udp
{
    /*
      1. 分帧的核心作用
        解决粘包问题：当快速发送多个小包时，接收端可能一次性收到合并的数据
        解决拆包问题：当发送的数据超过MTU大小时，会被拆分为多个网络层数据包
      2. UDP分帧的特殊性
        虽然UDP本身不保证顺序和可靠性，但在应用层仍需处理：

        大文件分片：例如发送10MB的更新包，需手动拆分为多个UDP包
        业务消息完整性：确保每个应用层消息的完整解析
     *
     *  为什么UDP也需要分帧？
        MTU限制：单个UDP包最大约1500字节，超限会被IP层分片
        应用层大包：如发送10KB的玩家状态快照，必须手动分片
        可靠性需求：部分分片丢失时需要重传特定片段 
        
        主要功能是处理应用层的分帧和重组，解决UDP传输中的粘包、拆包问题，以及大文件的分片发送。
     */
    
    /// <summary>
    /// 消息分片
    /// </summary>
    public class MessageAssembler 
    {
        // 核心职责：
        // 1. 发送端分帧（大消息拆分为多个带业务头的UDP包）
        // 2. 接收端重组（按sequence编号重组分片）
        // 3. 基础可靠性（通过ACK确认机制）
        
        
        //分片字典：管理不同sequence的分片
        private readonly Dictionary<int, FrameReAssembler> _reassemblyBuffers = new();
        private JitterBuffer _jitterBuffer = new();

        private const int HEADER_SIZE = 16; // 新增头部大小常量
        
        public MessageAssembler() {}

        public Action<int> SendAckCallback { get; set; }
        
        // 发送时的分帧处理
        
        /// <summary>
        /// 消息分帧(分片)处理
        /// </summary>
        /// <param name="originalData"></param>
        /// <param name="maxFrameSize"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public List<byte[]> SplitToFrames(byte[] originalData, int maxFrameSize, int messageId)
        {
            if (originalData == null || originalData.Length == 0)
                throw new ArgumentException("数据不能为空");
            //计算有效载荷大小
            maxFrameSize -= HEADER_SIZE; // 确保包含头部
    
            List<byte[]> frames = new();
            //"-1"是为了得到正确的分片数
            int totalFrames = (originalData.Length + maxFrameSize - 1) / maxFrameSize;
            //生成递增序列号保证消息唯一性
            int seq = GetSequenceNumber();

            //分片保存
            for (int currentFrame = 0; currentFrame < totalFrames; currentFrame++)
            {
                FrameHeader header = new FrameHeader
                {
                    MessageId = messageId,
                    TotalFrames = totalFrames,
                    CurrentFrame = currentFrame,
                    Sequence = seq
                };

                // 序列化头部
                byte[] headerBytes = FrameHeader.Serialize(header);
        
                // 计算数据分片
                int offset = currentFrame * maxFrameSize;
                int dataSize = Math.Min(maxFrameSize, originalData.Length - offset);
                byte[] frameData = new byte[HEADER_SIZE + dataSize];
                
                // 组装数据包 使用内存块拷贝高效组装数据包
                Buffer.BlockCopy(headerBytes, 0, frameData, 0, HEADER_SIZE);
                Buffer.BlockCopy(originalData, offset, frameData, HEADER_SIZE, dataSize);
        
                frames.Add(frameData);
            }
            return frames;
        }
        
        /// <summary>
        /// 分片重组
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="onComplete"></param>
        public void ProcessEnhancedFrame(byte[] frame, Action<int, byte[]> onComplete) 
        {
            if (frame.Length < 16) 
                return; // 头部需要16字节
            // 读取消息ID（从大端转回）
            byte[] messageIdBytes = new byte[4];
            Array.Copy(frame, 0, messageIdBytes, 0, 4);
            messageIdBytes = NetTool.ToBigEndian(messageIdBytes);
            int messageId = BitConverter.ToInt32(messageIdBytes, 0);
            
            //心跳包
            if (messageId == UdpNet.HEARTBEAT_MESSAGE_ID)
            {
                UdpNet._lastHeartbeatAckTime = DateTime.UtcNow; // 更新心跳时间
                return; // 心跳包不需要业务处理
            }
            
            // 读取总帧数（从大端转回）
            byte[] totalFramesBytes = new byte[4];
            Array.Copy(frame, 4, totalFramesBytes, 0, 4);
            totalFramesBytes = NetTool.ToBigEndian(totalFramesBytes);
            int totalFrames = BitConverter.ToInt32(totalFramesBytes, 0);

            // 读取当前帧号（从大端转回）
            byte[] currentFrameBytes = new byte[4];
            Array.Copy(frame, 8, currentFrameBytes, 0, 4);
            currentFrameBytes = NetTool.ToBigEndian(currentFrameBytes);
            int currentFrame = BitConverter.ToInt32(currentFrameBytes, 0);

            // 读取序列号（从大端转回）
            byte[] seqBytes = new byte[4];
            Array.Copy(frame, 12, seqBytes, 0, 4);
            seqBytes = NetTool.ToBigEndian(seqBytes);
            int sequence = BitConverter.ToInt32(seqBytes, 0);
    
            if (!_reassemblyBuffers.TryGetValue(sequence, out var reassembler))
            {
                reassembler = new FrameReAssembler(totalFrames, messageId);
                _reassemblyBuffers[sequence] = reassembler;
            }
    
            reassembler.AddFrame(currentFrame, frame, 16);
    
            //通过IsComplete判断是否全部接收
            if (reassembler.IsComplete)
            {
                onComplete?.Invoke(messageId, reassembler.GetData());
                _reassemblyBuffers.Remove(sequence);
                // 触发ACK发送
                if (SendAckCallback != null)
                    SendAckCallback(sequence);
            }
        }
        
        
        // 分片缓存清理
        private DateTime _lastCleanupTime;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromSeconds(30);
        //TODO:外部调用 Tick
        public void Tick() 
        { // 需由外部定期调用
            if (DateTime.UtcNow - _lastCleanupTime < _cleanupInterval)
                return;

            var expiredKeys = _reassemblyBuffers
                .Where(kv => DateTime.UtcNow - kv.Value.LastReceiveTime > TimeSpan.FromMinutes(1))
                .Select(kv => kv.Key)
                .ToList();

            foreach (var key in expiredKeys) {
                _reassemblyBuffers.Remove(key);
            }
            _lastCleanupTime = DateTime.UtcNow;
        }
        
        
        //序列号生成器：改为实例级别避免冲突
        private int _localSequence;
        private readonly object _seqLock = new();

        private int GetSequenceNumber() 
        {
            lock (_seqLock) 
            {
                return _localSequence++;
            }
        }
        
        
        // 分片重组
        private class FrameReAssembler 
        {
            public int MessageId { get; }
            private Dictionary<int, byte[]> _frames; // 改为字典存储
            private int _totalFrames;
            public DateTime LastReceiveTime { get; private set; } // 新增最后接收时间

            public FrameReAssembler(int totalFrames, int messageId) {
                _frames = new Dictionary<int, byte[]>(totalFrames);
                _totalFrames = totalFrames;
                LastReceiveTime = DateTime.UtcNow;
            }

            public void AddFrame(int index, byte[] data, int offset) {
                LastReceiveTime = DateTime.UtcNow; // 更新接收时间
                if (!_frames.ContainsKey(index)) {
                    _frames[index] = data[offset..];
                }
            }

            public bool IsComplete => _frames.Count == _totalFrames;

            public byte[] GetData() {
                using MemoryStream ms = new();
                for (int i = 0; i < _totalFrames; i++) { // 按顺序合并
                    if (_frames.TryGetValue(i, out var frame)) {
                        ms.Write(frame, 0, frame.Length);
                    }
                }
                return ms.ToArray();
            }
        }
        
        private struct FrameHeader 
        {
            public int MessageId; // 消息唯一标识
            public int TotalFrames; // 总分片数
            public int CurrentFrame; // 当前分片序号
            public int Sequence; // 消息序列号

            public static byte[] Serialize(FrameHeader header)
            {
                byte[] buffer = new byte[16];
                Buffer.BlockCopy(ConvertEndian(BitConverter.GetBytes(header.MessageId)), 0, buffer, 0, 4);
                Buffer.BlockCopy(ConvertEndian(BitConverter.GetBytes(header.TotalFrames)), 0, buffer, 4, 4);
                Buffer.BlockCopy(ConvertEndian(BitConverter.GetBytes(header.CurrentFrame)), 0, buffer, 8, 4);
                Buffer.BlockCopy(ConvertEndian(BitConverter.GetBytes(header.Sequence)), 0, buffer, 12, 4);
                return buffer;
            }

            public static FrameHeader Deserialize(byte[] data) {
                return new FrameHeader {
                    MessageId = BitConverter.ToInt32(ConvertEndian(data[0..4]), 0),
                    TotalFrames = BitConverter.ToInt32(ConvertEndian(data[4..8]), 0),
                    CurrentFrame = BitConverter.ToInt32(ConvertEndian(data[8..12]), 0),
                    Sequence = BitConverter.ToInt32(ConvertEndian(data[12..16]), 0)
                };
            }

            private static byte[] ConvertEndian(byte[] bytes) {
                if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                return bytes;
            }
        }
    }
    public class JitterBuffer
    {
        private SortedDictionary<int, byte[]> _buffer = new();
        private int _expectedSequence;
    
        public void Add(int seq, byte[] data) {
            _buffer[seq] = data;
            while (_buffer.ContainsKey(_expectedSequence)) {
                Submit(_buffer[_expectedSequence]);
                _buffer.Remove(_expectedSequence);
                _expectedSequence++;
            }
        }
    
        public event Action<byte[]> OnDataReady;
        private void Submit(byte[] data) => OnDataReady?.Invoke(data);
    }
    
}