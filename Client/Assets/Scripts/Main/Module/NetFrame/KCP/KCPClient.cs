using System;
using System.Threading;
using kcp2k;
using UnityEngine;

namespace xicheng.net.KCP
{
    public class KCPClient
    {
        private KcpConfig config;
        private KcpClient client;
        private KcpServer server;
        const ushort port = 7777;

        void InitConfig()
        {
            config = new KcpConfig();
            config.NoDelay = true; //force NoDelay和最小间隔。这样UpdateSeveralTimes（）就不需要等待很长时间 测试运行得快得多。
            config.DualMode = false; //不是所有的平台都支持双模式。DualMode=false情况下运行测试，这样它们就可以在所有平台上运行。
            config.Interval = 1; //1ms的间隔，代码至少运行一次。
            config.Timeout = 2000; //超时
            config.SendWindowSize = Kcp.WND_SND * 1000; //窗口尺寸较大，因此只需很少的更新调用即可刷新大消息。否则测试将花费太长时间。
            config.ReceiveWindowSize = Kcp.WND_RCV * 1000;
            config.CongestionWindow = false; //拥塞窗口严重限制发送/接收窗口的大小,发送最大尺寸的消息需要数千次更新。
            config.MaxRetransmits = Kcp.DEADLINK * 2;//检测到死链接前的最大重传尝试次数默认 * 2 检查配置是否有效
             
        }

        void CreateServer()
        {
            KcpServer server = new KcpServer(
                (connectionId) => {},
                (connectionId, message, channel) => Log.Info($"[KCP] OnServerDataReceived({connectionId}, {BitConverter.ToString(message.Array, message.Offset, message.Count)} @ {channel})"),
                (connectionId) => {},
                (connectionId, error, reason) => Log.Error($"[KCP] OnServerError({connectionId}, {error}, {reason}"),
                config
            );
        }
        
        void CreateClient()
        {
            KcpClient client = new KcpClient(
                () => {},
                (message, channel) => Log.Info($"[KCP] OnClientDataReceived({BitConverter.ToString(message.Array, message.Offset, message.Count)} @ {channel})"),
                () => {},
                (error, reason) => Log.Warning($"[KCP] OnClientError({error}, {reason}"),
                config
            );
        }
        
        
        // convenience function
        void UpdateSeveralTimes(int amount)
        {
            // update serveral times to avoid flaky tests.
            // => need to update at 120 times for default maxed sized messages
            //    where it requires 120+ fragments.
            // => need to update even more often for 2x default max sized
            for (int i = 0; i < amount; ++i)
            {
                client.Tick();
                server.Tick();
                // update 'interval' milliseconds.
                // the lower the interval, the faster the tests will run.
                Thread.Sleep((int)config.Interval);
            }
        }
        void TestKcp()
        {
            Debug.Log("kcp example");
            // // start server
            // server.Start(port);
            //
            // // connect client
            // client.Connect("127.0.0.1", port);
            // UpdateSeveralTimes(5);
            //
            // // send client to server
            // client.Send(new byte[]{0x01, 0x02}, KcpChannel.Reliable);
            // UpdateSeveralTimes(10);
            //
            // // send server to client
            // int firstConnectionId = server.connections.Keys.First();
            // server.Send(firstConnectionId, new byte[]{0x03, 0x04}, KcpChannel.Reliable);
            // UpdateSeveralTimes(10);
        }
    }
}