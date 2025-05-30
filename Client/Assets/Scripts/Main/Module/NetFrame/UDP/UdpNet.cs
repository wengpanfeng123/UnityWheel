using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using xicheng.common;

/* 
  ***** 指数退避算法 *****
  指数退避通常用于网络请求失败后的重试，每次重试的等待时间按指数增长，避免频繁请求加重服务器负担。
  重试延迟 = min(初始延迟 × 退避系数^尝试次数, 最大延迟)
  *****滑动窗口
    1.核心作用:
     流量控制：防止发送方淹没接收方
     提高吞吐量：允许在等待ACK时继续发送后续数据包
     可靠性保障：有序管理分片确认与重传
  通过滑动窗口机制，在保持UDP高效性的同时实现了可靠传输，是实时网络应用中平衡速度与可靠性的关键技术。
  
  
  ***** 可靠传输协议 *****
  1. ACK确认机制
     ACK包结构：
       [0-3] 0xFFFFFFFF (ACK标识)   [4-7] 序列号(大端)
     确认流程：
        (1)接收方重组完成后发送ACK
        (2)发送方维护待确认消息字典
        (3)收到ACK后移除对应消息记录
  2. 滑动窗口控制
     参数 : (1)窗口大小：8个分片  (2)超时阈值：1秒
     运行原理：
      
            
  3. 重传策略
    (1)每个分片独立记录发送时间戳
    (2)定时检查未确认分片
    (3)最大重试次数：3次
    (4)指数退避重传间隔
  
  ***** 发送/接收流程 *****
    发送流程：原始数据 -> SplitToFrames -> 滑动窗口发送 -> 等待ACK -> 超时重传
    接收流程：UDP包 -> 解析FrameHeader -> 存储分片 -> 检测完整性 -> 触发重组 -> 发送ACK
    
  ***** 连接管理系统 *****
  1. 自动重连机制。策略参数：
        class ReconnectPolicy {
            int MaxRetries = 3;     // 最大重试次数
            float BaseDelay = 2f;   // 初始延迟(秒)
            float MaxDelay = 30f;   // 最大延迟上限
            float BackoffFactor = 2;// 退避系数
        }
        退避算法：重试延迟 = min(初始延迟 × 2^尝试次数, 最大延迟)
  2. 连接恢复流程
     (1)检测到Socket异常时触发重连
     (2)释放旧连接资源
     (3)按退避策略等待重试
     (4)新建UDP客户端并重启接收线程
     
  ***** 线程安全设计 *****
  1.关键锁机制 lock锁
  2. 跨线程处理
    (1)使用ThreadPool处理发送操作
    (2)通过MainThreadDispatcher将网络事件转到主线程
    (3)异步接收使用BeginReceive/EndReceive模式
  
  // 调用层级示意
    Application Layer
        ↓ 调用
    UnityUdpClient.ReliableSend() 
        ↓ 调用
    MessageAssembler.SplitToFrames()
        ↓ 生成分片
    UnityUdpClient.Send()  // 实际发送每个分片
        ↓ 网络传输
    UDP Socket
  
  ***** 分帧机制 MessageAssembler.SplitToFrames *****
    1.计算有效载荷大小：maxFrameSize - 16字节头部
    2.生成递增序列号保证消息唯一性
    3.按MTU切割原始数据，添加包含元数据的头部
    4.使用内存块拷贝(Buffer.BlockCopy)高效组装数据包
    
  ***** 重组机制 MessageAssembler.SplitToFrames *****
    1.解析头部获取消息元数据
    2.按Sequence创建/获取重组器
    3.存储分片时记录最后接收时间
    4.使用字典存储无序到达的分片
    5.检测到全部分片到达后触发重组回调
    超时清理：
       每30秒清理超过1分钟未完成的重组器
       通过Tick方法维护生命周期
       
    消息分帧与重组 - 解决UDP粘包/拆包问题，支持大文件传输
    可靠传输机制 - 包含ACK确认、滑动窗口、超时重传
    连接管理 - 自动重连策略与指数退避算法
    线程安全设计 - 支持多线程环境下的安全操作
  
  TODO:
  FEC前向纠错 (FEC) :
    原理：添加冗余分片，允许接收方在部分分片丢失时恢复原始数据，减少重传次数。
    
  Jitter Buffer（抗抖动缓冲）:
   原理：缓存乱序到达的分片，按顺序提交给应用层。
  
  
 */
namespace xicheng.udp
{
    /// <summary>
    /// udp 网络框架(可靠性传输)
    /// </summary>
    public class UdpNet : MonoSingleton<UdpNet>
    {
        //基础数据
        public string _serverIP;
        public int _serverPort;
        private UdpClient _udpClient;
        private IPEndPoint _remoteEP;
        
        //分片、重组
        private MessageAssembler _assembler;
        private Dictionary<int, PendingMessage> _pendingPackets = new();
        
        //滑动窗口控制
        private Dictionary<int, PendingMessage> _pendingWindows = new();
        private int _currentWindowSize = 8; // 初始窗口大小
        private const int MIN_WINDOW = 4;   // 最小窗口限制
        private const int MAX_WINDOW = 64;  // 最大窗口限制
    
        // 网络质量评估参数
        private double _avgRtt = 0.5f;      // 平均往返时间（秒）
        private float _packetLossRate;     // 丢包率百分比
        private DateTime _lastAdjustTime;  // 最后调整时间
        
        //重连
        private bool _isReconnecting; //是否正在重连
        private int _reconnectAttempts; //当前重连次数
        private Coroutine _retryCoroutine;
        private const int MaxRetriesConnect = 3;  //最大重连次数
        private readonly ReconnectPolicy _reconnectPolicy = new(); //重连策略
        
        //线程安全
        private readonly object _sendLock = new();
        private readonly object _pendingPacketsLock = new();
        

        //主线程初始化。
        public void OnMainThreadInit()
        {
            _assembler = new MessageAssembler();
            _assembler.SendAckCallback = (seq) => SendAckPacket(seq);
            StartCoroutine(HeartbeatCheckCoroutine());
        }

        // 初始化客户端
        public void Connect(string ip, int port)
        {
            Close();
            _remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
            _udpClient = new UdpClient(0); // 自动分配本地端口
            _udpClient.Client.ReceiveTimeout = 1000;
            _serverPort = port;
            _serverIP = ip;
            _isReconnecting = false; //重置重连标志
            StartReceive();
        }

        #region 消息接收逻辑
        private void StartReceive()
        {
            try 
            {
                _udpClient.BeginReceive(ReceiveCallback, null);
            }
            catch (ObjectDisposedException e) //连接已关闭
            {
                Debug.LogError("连接已关闭。"+e.Message);
            }
        }
        
        private void ReceiveCallback(IAsyncResult ar) 
        {
            try 
            {
                IPEndPoint remoteEp = null;
                byte[] data = _udpClient.EndReceive(ar, ref remoteEp);
                MainThreadDispatcher.Enqueue(() => OnDataReceived(data));
                StartReceive(); // 继续接收下一个包
            } 
            catch (Exception ex)
            {
                MainThreadDispatcher.Enqueue(() => OnError(ex));
            }
        }
        
    
        private bool IsAckPacket(byte[] data)
        {
            // ACK包长度固定为8字节，且前4字节为0xFFFFFFFF
            // 检查是否为 ACK 包
            return data.Length >= 8 && 
                   data[0] == 0xFF && 
                   data[1] == 0xFF && 
                   data[2] == 0xFF && 
                   data[3] == 0xFF;
        }
        
        // 数据接收逻辑
        private void OnDataReceived(byte[] data) 
        {
            _lastPacketReceivedTime = DateTime.UtcNow; // 更新接收时间
            // 先判断是否是ACK包
            if (IsAckPacket(data)) 
            {
                HandleAck(data);
                return;
            }
    
            // 非ACK包则走分片重组流程
            _assembler.ProcessEnhancedFrame(data, (messageId, payload) => 
            {
                MainThreadDispatcher.Enqueue(() =>
                {
                    //TODO:数据进行反序列化。
                    Debug.Log("收到消息：" + messageId);
                    EventMgr.Inst.Dispatch(EEventType.NetMsg, messageId, payload);
                });
            });
        }

      

        #endregion
        
        void Update()
        {
            // 每帧调用分片缓存清理
            _assembler?.Tick();
            //主线程update
            MainThreadDispatcher.Tick();
            
            // 每帧检查超时分片
            foreach (var msg in _pendingPackets.Values)
            {
                for (int i = 0; i < msg.SendTimestamps.Length; i++)
                {
                    // 动态超时阈值 = 平均RTT * 安全系数
                    double timeout = _avgRtt * 2.5; 
                    if ((DateTime.UtcNow - msg.SendTimestamps[i]).TotalSeconds > timeout)
                    {
                        ResendFrame(msg, i); // 触发重传
                    }
                }
            }
            
            // 接收超时检测（30秒无数据）
            if ((DateTime.UtcNow - _lastPacketReceivedTime).TotalSeconds > 30) {
                OnError(new Exception("接收数据超时"));
            }
            
            // 发送失败检测（10秒内无成功发送）
            if ((DateTime.UtcNow - _lastSendSuccessTime).TotalSeconds > 10) {
                OnError(new Exception("持续发送失败"));
            }
        }

        private void HandleAck(byte[] data)
        {
            // ACK包结构：0xFFFFFFFF(4字节) + sequence(4字节)
            byte[] seqBytes = new byte[4];
            Array.Copy(data, 4, seqBytes, 0, 4);
            seqBytes = NetTool.ToBigEndian(seqBytes);
            int ackSequence = BitConverter.ToInt32(seqBytes, 0);
            lock (_pendingPacketsLock)
            {
                if (_pendingPackets.TryGetValue(ackSequence, out var msg))
                {
                    _pendingPackets.Remove(ackSequence);
                    Debug.Log($"已确认序列号: {ackSequence}");
                    
                    // 滑动窗口：继续发送后续分片
                    SendWindowFrames(msg);
                    // 全部分片确认后移除
                    if (msg.NextSendIndex >= msg.Frames.Count)
                    {
                        _pendingPackets.Remove(ackSequence);
                    }
                    
                    //计算网络指标
                    // 计算精确RTT（微秒级）
                    double rttMs = (DateTime.UtcNow - msg.LastSendTime).TotalMilliseconds;
        
                    // 使用EWMA算法平滑波动
                    _avgRtt = 0.875f * _avgRtt + 0.125f * rttMs;
        
                    // 触发窗口调整
                    AdjustWindowSize();
                }
            }
        }
        
        #region 发送数据
       
        /* 1、小数据无需可靠性传输：Send
         * 适用情况：聊天消息、实时位置更新等容忍丢包的场景
         * 2. 大数据需要可靠传输：ReliableSend
         * 内部流程：
            调用SplitToFrames进行分片
            添加序列号/消息ID等元数据
            通过Send()发送每个分片
            维护滑动窗口等待ACK
         */
        
        
        /// <summary>
        /// 对外/内接口：发送数据(线程安全)
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            //发送操作使用 ThreadPool 避免阻塞主线程
            ThreadPool.QueueUserWorkItem(_ => {
                lock (_sendLock) {
                    try {
                        _udpClient.Send(data, data.Length, _remoteEP);
                        _lastSendSuccessTime = DateTime.UtcNow; // 新增发送成功时间记录
                    } catch (SocketException e) {
                        Debug.LogError($"发送失败: {e.SocketErrorCode}");
                        MainThreadDispatcher.Enqueue(() => OnError(e));
                    } catch (Exception ex) {
                        MainThreadDispatcher.Enqueue(() => OnError(ex));
                    }
                }
            });
        }

        
        /// <summary>
        /// 对外接口： 可靠传输
        /// </summary>
        /// <param name="data"></param>
        /// <param name="messageId"></param>
        //可靠性发送。发送所有分片
        public void ReliableSend(byte[] data, int messageId)
        {
            int maxFrameSize = 1484;
            List<byte[]> frames = _assembler.SplitToFrames(data, maxFrameSize, messageId);
            
            // 获取序列号（从首帧头部解析）
            byte[] seqBytes = new byte[4];
            Array.Copy(frames[0], 12, seqBytes, 0, 4);
            seqBytes = NetTool.ToBigEndian(seqBytes);
            
            int seq = BitConverter.ToInt32(seqBytes, 0);

            lock (_pendingPacketsLock) {
                if (!_pendingPackets.ContainsKey(seq))
                {
                    var pendingMsg = new PendingMessage(seq, frames);
                    _pendingPackets[seq] = pendingMsg;
            
                    // 首次发送窗口内分片
                    SendWindowFrames(pendingMsg); 
                }
            }
        }
        
        
        private void SendWindowFrames(PendingMessage msg)
        {
            // 使用动态窗口值替换常量
            int windowStart = msg.NextSendIndex;
            int windowEnd = Math.Min(windowStart + _currentWindowSize, msg.Frames.Count);
            
            for (int i = windowStart; i < windowEnd; i++) {
                if ((DateTime.UtcNow - msg.SendTimestamps[i]).TotalSeconds > 1) {
                    Send(msg.Frames[i]);
                    msg.SendTimestamps[i] = DateTime.UtcNow; // 记录发送时间
                }
            }
            msg.NextSendIndex = windowEnd; // 滑动窗口位置
        }
        
        private void ResendFrame(PendingMessage msg, int frameIndex)
        {
            // 检查重试次数
            if (msg.Retries >= MaxRetriesConnect)
            {
                lock (_pendingPacketsLock)
                {
                    _pendingPackets.Remove(msg.Sequence);
                    Debug.LogError($"消息 {msg.Sequence} 超过最大重试次数");
                    return;
                }
            }
            // 重发指定分片
            Send(msg.Frames[frameIndex]);
            // 更新分片发送时间戳
            msg.SendTimestamps[frameIndex] = DateTime.UtcNow;
            // 增加重试计数器
            msg.Retries++;
            
            // 每触发一次重传视为丢包---->增加丢包率统计。
            _packetLossRate = 0.9f * _packetLossRate + 0.1f; // 滑动平均
        }
        
        //发送ack
        private void SendAckPacket(int seq)
        {
            //ack(构造) = ack标识(4字节)+seq(4字节)
            
            byte[] ackPacket = new byte[8];
            // ACK标识（0xFFFFFFFF，大端）
            byte[] ackFlagBytes = { 0xFF, 0xFF, 0xFF, 0xFF }; // 直接构造ACK标识字节
            ackFlagBytes = NetTool.ToBigEndian(ackFlagBytes);
            Buffer.BlockCopy(ackFlagBytes, 0, ackPacket, 0, 4);

            // 序列号（大端）
            byte[] seqBytes = BitConverter.GetBytes(seq);
            seqBytes = NetTool.ToBigEndian(seqBytes);
            Buffer.BlockCopy(seqBytes, 0, ackPacket, 4, 4);
            Send(ackPacket);
        }
        #endregion
        
        // 实现错误处理
        private void OnError(Exception ex) 
        {
            Debug.LogError($"网络错误: {ex.Message}");
    
            // 新增状态判断
            if (!_isReconnecting && _udpClient != null && _udpClient.Client.IsBound) 
            {
                _isReconnecting = true;
                _reconnectAttempts++;
                ReconnectAfter();
            }
    
            // 触发全局事件
            MainThreadDispatcher.Enqueue(() => {
                EventMgr.Inst.Dispatch(EEventType.NetError, ex.Message);
            });
        }
        
        #region 监测网络状态(心跳包)
        private DateTime _lastHeartbeatSentTime;
        public static DateTime _lastHeartbeatAckTime; //最后一次心跳响应时间
        private DateTime _lastPacketReceivedTime = DateTime.UtcNow; //上次接收时间
        private DateTime _lastSendSuccessTime = DateTime.UtcNow; //上次发送成功时间
        private const float HEARTBEAT_INTERVAL = 5f; // 心跳间隔5秒
        private const float HEARTBEAT_TIMEOUT = 15f; // 超时时间15秒
        public static int HEARTBEAT_MESSAGE_ID = -999; // 特殊消息ID标识心跳包
        
        // 心跳检测
        private IEnumerator HeartbeatCheckCoroutine() {
            while (true)
            {
                yield return new WaitForSeconds(HEARTBEAT_INTERVAL);
                if (IsConnected) 
                {
                    // 发送心跳包
                    byte[] heartbeatData = new byte[0];
                    ReliableSend(heartbeatData, HEARTBEAT_MESSAGE_ID);
                    _lastHeartbeatSentTime = DateTime.UtcNow;
            
                    // 超时检测（需在主线程触发错误）正确超时判断：基于发送时间
                    bool isTimeout = (DateTime.UtcNow - _lastHeartbeatSentTime).TotalSeconds > HEARTBEAT_TIMEOUT;
                    if (isTimeout)
                    {
                        MainThreadDispatcher.Enqueue(() => 
                            OnError(new Exception($"心跳超时：{HEARTBEAT_TIMEOUT}秒未收到响应"))
                        );
                    }
                }
            }
        }
        
        public bool IsConnected {
            get {
                try {
                    return _udpClient is { Client: { IsBound: true } } //IsBound:检测底层Socket是否绑定本地端口
                           && !_isReconnecting;
                } catch {
                    return false;
                }
            }
        }
        #endregion
        
        private void OnGUI()
        {
            GUI.Label(new Rect(10,100,300,30), 
                $"动态窗口: {_currentWindowSize} 平均RTT:{_avgRtt:F2}s 丢包率:{_packetLossRate:P0}");
            GUI.Label(new Rect(10, 50, 200, 30), 
                $"连接状态: {(IsConnected ? "已连接" : "未连接")}"
            );
        }
        
        #region 重连逻辑
   
        private void ReconnectAfter()
        {
            if (!_isReconnecting && _reconnectAttempts <= _reconnectPolicy.MaxRetries)
            {
                _isReconnecting = true;
                _reconnectAttempts++;
                StartCoroutine(ReconnectCoroutine(_reconnectAttempts));
            }
        }

        private IEnumerator ReconnectCoroutine(int attempt)
        {
            // 计算退避延迟（BaseDelay * BackoffFactor^attempt，不超过MaxDelay）
            float delay = Mathf.Min(
                _reconnectPolicy.BaseDelay * Mathf.Pow(_reconnectPolicy.BackoffFactor, attempt - 1),
                _reconnectPolicy.MaxDelay
            );
            Debug.Log($"第 {attempt} 次重连，等待 {delay} 秒...");
            yield return new WaitForSecondsRealtime(delay);

            try
            {
                // 释放旧连接
                if (_udpClient != null)
                {
                    _udpClient.Close();
                    _udpClient = null;
                }

                // 新建连接
                _udpClient = new UdpClient(0);
                StartReceive(); // 重启接收线程

                Debug.Log("重连成功");
                _isReconnecting = false;
                _reconnectAttempts = 0; // 重置计数器
            }
            catch (Exception ex)
            {
                Debug.LogError($"重连失败: {ex.Message}");
                _isReconnecting = false;

                // 递归触发下一次重连（直到超过MaxRetries）
                if (attempt < _reconnectPolicy.MaxRetries)
                {
                    ReconnectAfter();
                }
                else
                {
                    Debug.LogError($"超过最大重试次数（{_reconnectPolicy.MaxRetries}），停止连接");
                }
            }
        }
        #endregion

        
        private void AdjustWindowSize()
        {
            // 计算时间间隔（最小调整间隔1秒）
            float timeSinceLast = (float)(DateTime.UtcNow - _lastAdjustTime).TotalSeconds;
            if (timeSinceLast < 1f)
                return;

            // 自适应算法（可根据需求调整系数）
            if (_packetLossRate > 0.3f) {
                // 高丢包率时激进缩小窗口
                _currentWindowSize = Mathf.Max(MIN_WINDOW, _currentWindowSize / 2);
            }
            else if (_avgRtt > 0.2f) {
                // 高延迟时保守缩小窗口
                _currentWindowSize = Mathf.Max(MIN_WINDOW, _currentWindowSize - 2);
            }
            else if (_packetLossRate < 0.05f && _avgRtt < 0.1f) {
                // 网络良好时指数增长
                _currentWindowSize = Mathf.Min(MAX_WINDOW, _currentWindowSize * 2);
            }
            else {
                // 线性增长
                _currentWindowSize = Mathf.Min(MAX_WINDOW, _currentWindowSize + 1);
            }

            Debug.Log($"窗口调整: {_currentWindowSize} RTT:{_avgRtt:F2}s 丢包:{_packetLossRate:P0}");
            _lastAdjustTime = DateTime.UtcNow;
        }
        
        
        public void Close()
        {
            if (_retryCoroutine != null) 
            {
                StopCoroutine(_retryCoroutine);
            }
            if (_udpClient != null)
            {
                _udpClient.Close();
                _udpClient = null; // 关键：置空以更新IsConnected状态
            }
            _isReconnecting = false; // 重置重连标志
        }
        
        
        // 修改PendingPacket为跟踪整个消息
        private class PendingMessage
        {
            public int NextSendIndex { get; set; } //窗口位置指针
            public DateTime[] SendTimestamps;     //各分片发送时间
            public int Sequence { get; }
            public List<byte[]> Frames { get; }
            public DateTime LastSendTime { get; set; }
            public int Retries { get; set; }

            public PendingMessage(int seq, List<byte[]> frames)
            {
                Sequence = seq;
                Frames = frames;
                LastSendTime = DateTime.UtcNow;
                Retries = 0;
                
                // 初始化所有分片时间戳
                SendTimestamps = new DateTime[frames.Count]; 
                for (int i = 0; i < frames.Count; i++) {
                    SendTimestamps[i] = DateTime.MinValue;
                }
            }
        }
    }
}