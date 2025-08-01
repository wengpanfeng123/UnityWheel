using System;

namespace Xicheng.udp
{
    //重连策略
    [Serializable]
    public class ReconnectPolicy
    {
        public int MaxRetries = 3; //最大重试次数
        public float BaseDelay = 2f; // 初始延迟（秒）
        public float MaxDelay = 30f; // 最大延迟上限
        public float BackoffFactor = 2f;  // 指数基数
    }
}