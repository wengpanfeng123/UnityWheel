using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text;
using UnityEngine.AI;

namespace Xicheng.tcp
{
    public class example_net : MonoBehaviour
    {
        private Camera cam;
        private Collider col;

        private NavMeshAgent agent;

        //定义套接字
        Socket socket;
        bool isConnencted = false;

        //UGUI
        public InputField Input;

        public Text text;

        //缓冲区
        byte[] readbuff = new byte[1024];

        //缓冲区数据长度
        int buffCount = 0;
        string recStr = "";

        void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 200, 80), "Connect"))
            {
                Connection();
            }

            if (GUI.Button(new Rect(0, 100, 200, 80), "Test_Send"))
            {
                for (int i = 0; i < 30; i++) ///客户端启动向服务端发送250条数据
                {
                    string s = string.Format("Datianshi-{0}", i);
                    socket.Send(SendMsg(s));
                }
            }
        }

        /// <summary>
        /// 构造发送数据
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public byte[] SendMsg(string msg)
        {
            int length = msg.Length;
            //构造表头数据，固定4个字节的长度，表示内容的长度
            byte[] headerBytes = BitConverter.GetBytes(length);
            //构造内容
            byte[] bodyBytes = Encoding.UTF8.GetBytes(msg);
            byte[] tempBytes = new byte[headerBytes.Length + bodyBytes.Length];
            ///拷贝到同一个byte[]数组中，发送出去..
            Buffer.BlockCopy(headerBytes, 0, tempBytes, 0, headerBytes.Length);
            Buffer.BlockCopy(bodyBytes, 0, tempBytes, headerBytes.Length, bodyBytes.Length);
            return tempBytes;
        }

        //异步连接
        public void Connection()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect("127.0.0.1", 6789, connectCallback, socket);
        }

        //连接回调
        public void connectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                Debug.Log("connect succed");
                isConnencted = true;
                ;
                Send();
            }
            catch (SocketException e)
            {
                Debug.Log("socket fail" + e.ToString());
            }

            socket.BeginReceive(readbuff, buffCount, 1024 - buffCount, 0, Receivecallback, socket);
        }

        //接收回调
        public void Receivecallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int count = socket.EndReceive(ar);
                buffCount += count;
                //处理二进制消息
                OnReceiveData();
                string s = System.Text.Encoding.Default.GetString(readbuff, 0, count);
                recStr = s + "\n" + recStr;
                socket.BeginReceive(readbuff, buffCount, 1024 - buffCount, 0, Receivecallback, socket);
            }
            catch (SocketException e)
            {
                Debug.Log("socket receive fail" + e.ToString());
            }
        }

        //异步发送
        public void Send()
        {
            // string sendStr = Input.text;
            string sendStr = "Hello Server";
            byte[] bodyBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            int len = bodyBytes.Length;
            byte[] lenBytes = BitConverter.GetBytes(len);
            byte[] sendbytes = lenBytes.Concat(bodyBytes).ToArray();
            socket.BeginSend(sendbytes, 0, sendbytes.Length, 0, sendcallback, socket);
        }

        //发送回调
        private void sendcallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int count = socket.EndSend(ar);
                Debug.Log("Socket send succ ,number is:" + count);

            }
            catch (SocketException e)
            {
                Debug.Log("socket send erro:" + e.ToString());
            }
        }

        //更新UGUI界面的文本框
        public void Update()
        {
            //text.text=recStr;
        }

        //接收数据处理
        public void OnReceiveData()
        {
            Debug.Log($"[Recv 1] PacketSize ={buffCount}"); //接收的总字节数（含长度）
            //消息长度小于4，不做处理
            if (buffCount <= 4)
            {
                return;
            }

            int bodyLength = BitConverter.ToInt32(readbuff, 0); //消息体长度
            Debug.Log("[Recv 2] BodySize = " + bodyLength); //显示消息体长度
            //消息长度大于2，但是不足长度，不做处理
            if (buffCount < 4 + bodyLength)
            {
                return;
            }

            string s = Encoding.UTF8.GetString(readbuff, 4, bodyLength); //处理消息
            Debug.Log("[Recv 3] Msg : " + s); //显示消息内容
            //更新缓冲区
            int start = 4 + bodyLength; //更新起始位置
            int count = buffCount - start; //更新计数
            Array.Copy(readbuff, start, readbuff, 0, count); //缓冲区移位
            buffCount -= start;
            Debug.Log($"[Recv 4] PacketSize1 ={buffCount}"); //显示计数
            recStr = s + "\n" + recStr;
            //继续读消息
            OnReceiveData();
        }

        public void Close()
        {
            isConnencted = false;
            socket.Close();
            socket.Shutdown(SocketShutdown.Both);
        }

        private void FixedUpdate()
        {
            if (socket != null && isConnencted)
            {
                bool p = socket.Poll(1000, SelectMode.SelectRead);
                if (p && socket.Available == 0)
                {
                    // Socket has been disconnected
                    Debug.Log("Socket has been disconnected");
                }
            }
        }
    }
}