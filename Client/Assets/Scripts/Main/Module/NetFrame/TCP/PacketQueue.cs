//- Created:         #xicheng#
// - CreateTime:     #CreateTime#
// - Description:    网络包队列


using System.Collections.Generic;

namespace Xicheng.tcp
{

    /// <summary>
    /// 一个网络线程安全队列，用来在主线程和io线程之间传递NetPacket对象
    /// </summary>
    public class NetPacketQueue
    {
        private Queue<NetPacket> _netPacketsQueue = new();

        /// <summary>
        /// 存储网络包
        /// </summary>
        /// <param name="netPacket"></param>
        public void Enqueue(NetPacket netPacket)
        {
            lock (netPacket)
            {
                _netPacketsQueue.Enqueue(netPacket);
            }
        }

        /// <summary>
        /// 取出网络包
        /// </summary>
        /// <returns></returns>
        public NetPacket Dequeue()
        {
            lock (_netPacketsQueue)
            {
                return _netPacketsQueue.Count > 0 ? _netPacketsQueue.Dequeue() : null;
            }
        }

        /// <summary>
        /// 清空网络队列
        /// </summary>
        public void Clear()
        {
            lock (_netPacketsQueue)
            {
                _netPacketsQueue.Clear();
                _netPacketsQueue = null;
            }
        }
    }


    /// <summary>
    /// 网络包的结构定义
    /// </summary>
    public class NetPacket
    {
        public NetPacket(PacketType packetType)
        {
            // ProtoCode = protoCode;
            PacketType = packetType;
        }


        public PacketType PacketType = PacketType.None; //消息包的类型
        public int ProtoCode; //协议号（消息id）
        public int CurReceive; //当前接收了多少字节
        public const int HeaderSize = 8; //包头长度：前4个字节,包体长度。后4个字节,协议号
        public byte[] PacketHeaderBytes; //包头内容
        public byte[] PacketBodyBytes; //包体内容
        public byte[] PacketBytes = null; //包的完整数据，发送时调用。
        public int TerminalId = 1; //TODO:
    }

    /// <summary>
    /// 网络包的数据类型
    /// </summary>
 
    public enum PacketType
    {
        None = 0, //未初始化的包
        ConnectSucceed = 1, //连接成功的
        ConnectFailed = 2, //连接失败
        TcpPacket = 3, //正常传输的tcp包
        Disconnect = 4, //网络断开的包
    }
}

