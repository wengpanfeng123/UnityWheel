//- Created:        #xicheng#
// - CreateTime:    #CreateTime#
// - Description:   网络管理器

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UniLog;
using UnityEngine;
using xicheng.common;
using Console = System.Console;

/*
 SocketFlags: https://learn.microsoft.com/zh-cn/dotnet/api/system.net.sockets.socketflags?view=net-7.0#system-net-sockets-socketflags-none
 BeginReceive:
 Socket.Poll：https://learn.microsoft.com/zh-cn/dotnet/api/system.net.sockets.socket.poll?view=net-7.0#system-net-sockets-socket-poll(system-int32-system-net-sockets-selectmode)
 */
public class MsgEventArgs : EventArgs
{
}

public delegate void MsgCallBack(object sender, MsgEventArgs msg);

/*
 *消息发送：入队列
 * Packet : 表示接收到了一个网络包。
 * Msg:是一个消息体（包头+包体）
 * Packet 包含一个或者多个Msg
 */

public enum TerminalType
{
    Auth = 0, //验证服
    Center = 1, //中心服
    Logic = 2, //逻辑服
}

public delegate void TcpMsgCallBack(uint msgId, byte[] body,int terminalId);

namespace xicheng.tcp
{
    public class TcpNet : MonoSingleton<TcpNet>
    {
        private Socket _socket; //套接字.
        private NetPacketQueue _packetQueue; //接收到的网络包队列

        private bool _isSocketRunning; //当前网络状态，true-已连接 false-未连接
        private readonly int _heartbeatTime = 5; //心跳间隔

        private List<NetPacket> _tempPacketsList; //临时缓存

        private readonly Dictionary<int, TcpMsgCallBack> _messageCallback = new();

        private void Start()
        {
            _packetQueue = new NetPacketQueue();
            _tempPacketsList = new List<NetPacket>();
        }
        
        #region 消息注册

        public void RegisterMsg(int msgId, TcpMsgCallBack callback)
        {
            lock (_messageCallback)
            {
                if (!_messageCallback.TryAdd(msgId, callback))
                {
                    Log.Error($"重复注册消息回调,msgId:{msgId}");
                }
            }
        }


        public void UnRegisterMsg(int msgId, TcpMsgCallBack callback)
        {
            lock (_messageCallback)
            {
                if (_messageCallback.ContainsKey(msgId))
                {
                    _messageCallback.Remove(msgId);
                }
            }
        }
        #endregion

        #region Connect

        public void Connect(string ip, int port)
        {
            try
            {
                lock (this)
                {
                    if (!_isSocketRunning)
                    {
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPAddress ipAddress = IPAddress.Parse(ip);
                        IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
                        socket.BeginConnect(ipEndPoint, ConnectCallback, socket);

                        // 启动实时检测线程
                        new Thread(RealtimeCheckThread).Start();
                        // 启动心跳线程
                        new Thread(HeartbeatThread).Start();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("IP端口号错误或者服务器未开启."+e.Message);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            lock (this)
            {
                if (_isSocketRunning)
                    return;
                try
                {
                    _socket = (Socket)ar.AsyncState;
                    _isSocketRunning = true;
                    _socket.EndConnect(ar);
                    _packetQueue.Enqueue(new NetPacket(PacketType.ConnectSucceed));

                    //开始接收数据
                    ReadPacket();
                    //StartReceive();
                }
                catch (Exception e)
                {
                    //TODO：断网
                    _socket = null;
                    _isSocketRunning = false;
                    _packetQueue.Enqueue(new NetPacket(PacketType.ConnectFailed));
                    Debug.LogError("[ConnectCallback] Connect failed ! " + e.Message);
                }
            }
        }

        #endregion


        private void SendAsyncCallback(IAsyncResult ar)
        {
            lock (this)
            {
                try
                {
                    Socket s = (Socket)ar.AsyncState;
                    s.EndSend(ar);
                }
                catch (Exception e)
                {
                    //TODO:断网
                    Debug.LogException(e);
                }
            }

            _socket.EndSend(ar, out SocketError socketError);
            if (socketError != SocketError.Success)
            {
                Debug.LogError("发送失败，Socket Error ! " + socketError);
            }

            //TODO:做一些发送成功回调
        }

        //断开


        /// <summary>
        /// 主线程主动取走队列中所有的网络包
        /// </summary>
        /// <returns></returns>
        public List<NetPacket> GetPackets()
        {
            //_tempPacketsList.Clear();
            NetPacket packet = _packetQueue.Dequeue();
            while (packet != null)
            {
                //_tempPacketsList.Add(packet);
                packet = _packetQueue.Dequeue();
                lock (_messageCallback)
                {
                    //触发消息回调。
                    if (_messageCallback.TryGetValue(packet.ProtoCode, out var tcpMsgCallBack))
                    {
                        tcpMsgCallBack((uint)packet.ProtoCode, packet.PacketBodyBytes,packet.TerminalId);
                    }
                    else
                    {
                        Log.Error($" _messageCallback 消息未注册.id ={packet.ProtoCode} ");
                    }
                }
            }

            return _tempPacketsList;
        }

        /// <summary>
        /// 读取包的内容
        /// </summary>
        private void ReadPacket()
        {
            if (!_isSocketRunning)
            {
                Debug.LogError("[StartReceive] socket disconnect");
                return;
            }

            //创建一个网络包
            NetPacket netPacket = new NetPacket(PacketType.TcpPacket)
            {
                PacketHeaderBytes = new byte[NetPacket.HeaderSize]
            };

            _socket.BeginReceive(netPacket.PacketHeaderBytes, 0, NetPacket.HeaderSize, SocketFlags.None, ReceiveHeader,
                netPacket);
            /*
             * 参数1：它是存储接收到的数据的位置。
             * 参数2：buffer 中存储所接收数据的位置
             * 参数3：要接收的字节数。(牢记！)
             */
        }

        /// <summary>
        /// 接收头部数据
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveHeader(IAsyncResult ar)
        {
            lock (this)
            {
                try
                {
                    NetPacket netPacket = (NetPacket)ar.AsyncState;
                    //实际读取到的一个包的长度
                    int readSize = _socket.EndReceive(ar);
                    //当数据为0时，说明服务器把网络断开了。这是tcp的约定
                    if (readSize == 0)
                    {
                        //TODO:断网
                        return;
                    }

                    netPacket.CurReceive += readSize;
                    if (netPacket.CurReceive == NetPacket.HeaderSize)
                    {
                        //收到约到长度大小之后，重置标记，准备接收后面的包体
                        netPacket.CurReceive = 0;
                        //包体长度
                        //这4个字节，可以从左读到右，也可以从右读到左边(也就是大小端、高低位的概念)，需要一个标准从那边开始读。
                        int bodySize =
                            IPAddress.NetworkToHostOrder(BitConverter.ToInt32(netPacket.PacketHeaderBytes, 0));
                        int protoCode =
                            IPAddress.NetworkToHostOrder(BitConverter.ToInt32(netPacket.PacketHeaderBytes, 4));
                        netPacket.ProtoCode = protoCode; //设置协议号
                        //说明服务器发过来的消息有问题，最多是个空包为0，比如说心跳就是为0
                        if (bodySize < 0)
                        {
                            //TODO:断网
                            return;
                        }

                        //开始接收包体。
                        netPacket.PacketBodyBytes = new byte[bodySize];
                        if (bodySize == 0) //心跳包为0
                        {
                            _packetQueue.Enqueue(netPacket);
                            //到此一个包接收完毕，准备接收下一个包。
                            ReadPacket();
                            return;
                        }

                        _socket.BeginReceive(netPacket.PacketBodyBytes, 0, bodySize, SocketFlags.None, ReceiveBody,
                            netPacket);
                    }
                    else
                    {
                        //包头数据没有接收完，继续接收包头
                        int remainSize = NetPacket.HeaderSize - netPacket.CurReceive;
                        //传入的参数remainSize 表示还要收多少个字节。
                        _socket.BeginReceive(netPacket.PacketHeaderBytes, netPacket.CurReceive, remainSize,
                            SocketFlags.None, ReceiveHeader, netPacket);
                    }
                }
                catch (Exception e)
                {
                    //TODO:断网
                    Log.Warning(e);
                    throw;
                }
            }
        }


        /// <summary>
        /// 接收包体数据
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveBody(IAsyncResult ar)
        {
            lock (this)
            {
                try
                {
                    NetPacket netPacket = (NetPacket)ar.AsyncState;
                    //实际读取到的一个包的长度
                    int readSize = _socket.EndReceive(ar);
                    //当数据为0时，说明服务器把网络断开了。这是tcp的约定
                    if (readSize == 0)
                    {
                        //TODO:断网
                        return;
                    }

                    netPacket.CurReceive += readSize;
                    if (netPacket.CurReceive == netPacket.PacketBodyBytes.Length)
                    {
                        //收到约到长度大小之后，重置标记，准备接收后面的包体
                        netPacket.CurReceive = 0;
                        _packetQueue.Enqueue(netPacket);
                        //到此一个包接收完毕，准备接收下一个包。
                        ReadPacket();
                    }
                    else
                    {
                        //包头数据没有接收完，继续接收包头
                        int remainSize = netPacket.PacketBodyBytes.Length - netPacket.CurReceive;
                        //传入的参数remainSize 表示还要收多少个字节。
                        _socket.BeginReceive(netPacket.PacketBodyBytes, netPacket.CurReceive, remainSize,
                            SocketFlags.None, ReceiveBody, netPacket);
                    }
                }
                catch (Exception e)
                {
                    //TODO:断网
                    Log.Warning(e);
                    throw;
                }
            }
        }

        /// <summary>
        /// 主线程调用，发送网络报
        /// </summary>
        /// <param name="pCode">协议号(协议id)</param>
        /// <param name="body">字节流</param>
        public void SendAsync(int pCode, byte[] body)
        {
            //网络数据是大端模式，c#中的数据是小端模式
            byte[] bodySize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(body.Length));
            byte[] protoCode = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(pCode));
            byte[] packet = new byte[body.Length + protoCode.Length + bodySize.Length];
            Array.Copy(bodySize, 0, packet, 0, bodySize.Length); //填充包头前四个字节：包体长度
            Array.Copy(protoCode, 0, packet, bodySize.Length, protoCode.Length); //填充包头后四个字节：协议号
            Array.Copy(body, 0, packet, bodySize.Length + protoCode.Length, body.Length);
            SendAsync(packet);
        }


        /// <summary>
        /// 主线程调用，发送网络字节流
        /// </summary>
        /// <param name="bytes"></param>
        private void SendAsync(byte[] bytes)
        {
            lock (this)
            {
                try
                {
                    if (_isSocketRunning)
                    {
                        _socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, SendAsyncCallback, _socket);
                    }
                }
                catch (Exception e)
                {
                    //TODO:断网
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// 断开网络链接，可能是在io中调用，可能是在主线程中调用。
        /// </summary>
        public void Disconnect()
        {
            lock (this)
            {
                if (_isSocketRunning)
                {
                    Close();
                    //try
                    //{
                    //    m_Socket.Shutdown(SocketShutdown.Both);
                    //}
                    //catch (Exception ex)
                    //{
                    //    m_Socket.Close();
                    //    m_Socket = null;
                    //    m_SocketState = false;
                    //    PacketQueue.Clear();
                    //    PacketQueue.Enqueue(new NetPacket(PacketType.Disconnect));
                    //}
                }
            }
        }

        //主动断开连接
        private void Close()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Disconnect(false);
            _socket.Close();
            _socket = null;
            _isSocketRunning = false;
            _packetQueue.Clear();
            _packetQueue.Enqueue(new NetPacket(PacketType.Disconnect));
        }

        /*判定socket是否断开
         注1:Socket.Connected 不是指当前是否处于连接状态，而是指上一次收发是否完成，不是告诉你将来你收发是否能成功的
         注2:Socket.Poll 此方法无法检测某些类型的连接问题：例如网络电缆损坏，或者远程主机未正常关闭。 必须尝试发送或接收数据才能检测这些类型的错误。
         return:基于参数1中传递的轮询模式值的 Socket 的状态
         */
        public bool IsSocketDisconnect()
        {
            lock (this)
            {
                if (_socket != null && _isSocketRunning)
                {
                    bool p = _socket.Poll(1000, SelectMode.SelectRead); //ture:如果已调用且连接处于挂起状态、数据可供读取，或者连接已关闭、重置或终止
                    if (p && _socket.Available == 0) //Available 接收到的数据量
                    {
                        Debug.Log("Socket has been disconnected");
                        _isSocketRunning = false;
                        return true;
                    }
                }

                return false;
            }
        }

        private void Update()
        {
            //IsSocketDisconnect();
            GetPackets();
        }

        #region 断线检测

        // 实时检测线程（每100ms轮询）
        private void RealtimeCheckThread()
        {
            while (_isSocketRunning)
            {
                try
                {
                    bool isDisconnected = _socket.Poll(0, SelectMode.SelectRead) && _socket.Available == 0;
                    if (isDisconnected)
                    {
                        Console.WriteLine("[实时检测] 连接已断开！");
                        _isSocketRunning = false;
                        break;
                    }

                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"检测异常: {ex.Message}");
                    _isSocketRunning = false;
                }
            }
        }

        // 心跳线程
        private void HeartbeatThread()
        {
            while (_isSocketRunning)
            {
                try
                {
                    byte[] ping = new byte[1] { 0x01 };
                    _socket.Send(ping); // 发送心跳包
                    Thread.Sleep(_heartbeatTime * 1000);
                }
                catch
                {
                    Console.WriteLine("[心跳检测] 连接已断开！");
                    _isSocketRunning = false;
                    break;
                }
            }
        }

        void CheckByUnityAPI()
        {
            // 局限性：仅检测设备网络状态，无法判断游戏服务器连接性。
            // 监听网络变化事件
            NetworkReachability reachability = Application.internetReachability;
            if (reachability == NetworkReachability.NotReachable)
            {
            }
        }

        #endregion

        void OnApplicationQuit()
        {
            // if(m_Socket!= null)
            // {
            //     m_Socket?.Shutdown(SocketShutdown.Both);
            //     m_Socket?.Disconnect(true);
            //     m_Socket?.Close();
            // }
        }
    }
}