using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using kcp2k;
using UnityEngine;

namespace Xicheng.net.KCP
{
    public class KCPExample : MonoBehaviour 
    {
        private KcpClient kcpClient;
        private UdpClient udpClient;
        private IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
        private ConcurrentQueue<byte[]> receivedQueue = new ConcurrentQueue<byte[]>();

        // void Start() {
        //     // 初始化UDP
        //     udpClient = new UdpClient(0);
        //
        //     // 初始化KCP
        //     kcpClient = new KcpClient(12345); // convID
        //     //kcpClient.SetOutputCallback(SendUDPData);
        //     //kcpClient.SetNoDelay(true, 10, 2, true);
        //
        //     // 启动接收线程
        //     Thread receiveThread = new Thread(UDPReceive);
        //     receiveThread.Start();
        // }
        //
        //
        //
        // void SendUDPData(byte[] data, int length) {
        //     udpClient.Send(data, length, serverEndPoint);
        // }
        //
        // void UDPReceive() {
        //     while (true) {
        //         byte[] data = udpClient.Receive(ref serverEndPoint);
        //         kcpClient.Input(data);
        //         if (kcpClient.TryReceive(out byte[] message)) {
        //             receivedQueue.Enqueue(message);
        //         }
        //     }
        // }
        //
        // void Update() {
        //     kcpClient.Tick();
        //
        //     // 处理接收队列
        //     while (receivedQueue.TryDequeue(out byte[] data)) {
        //         string msg = Encoding.UTF8.GetString(data);
        //         Debug.Log($"收到消息: {msg}");
        //     }
        // }
        //
        // public void SendTestMessage() {
        //     string message = "Hello KCP!";
        //     byte[] data = Encoding.UTF8.GetBytes(message);
        //     kcpClient.Send(data);
        // }

        void OnDestroy() {
            udpClient?.Close();
        }
    }
}