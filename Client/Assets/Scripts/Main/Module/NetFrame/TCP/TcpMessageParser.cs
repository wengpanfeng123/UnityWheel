using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Xicheng.tcp
{
    
    //参考价值：
    public class TcpMessageParser 
    {
        private Socket socket;
        private List<byte> receiveBuffer = new List<byte>();
        private const int HeaderSize = 4; // 4字节消息头

        public TcpMessageParser(Socket socket) {
            this.socket = socket;
            StartReceiving();
        }

        private void StartReceiving() {
            byte[] buffer = new byte[4096];
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, buffer);
        }

        private void OnReceive(IAsyncResult ar) {
            try {
                int received = socket.EndReceive(ar);
                byte[] buffer = (byte[])ar.AsyncState;
                receiveBuffer.AddRange(buffer.Take(received));

                // 解析所有完整消息
                while (receiveBuffer.Count >= HeaderSize) {
                    int bodyLength = BitConverter.ToInt32(receiveBuffer.ToArray(), 0);
                    int totalLength = HeaderSize + bodyLength;

                    if (receiveBuffer.Count >= totalLength) {
                        byte[] body = receiveBuffer.GetRange(HeaderSize, bodyLength).ToArray();
                        OnMessageReceived(body); // 处理消息体
                        receiveBuffer.RemoveRange(0, totalLength); // 移除已处理数据
                    } else {
                        break; // 数据不足，等待下次接收
                    }
                }
                StartReceiving(); // 继续接收
            } catch (Exception ex) {
                Console.WriteLine($"接收异常: {ex.Message}");
                socket.Close();
            }
        }

        private void OnMessageReceived(byte[] body) {
            string message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"收到消息: {message}");
        }
    }
}