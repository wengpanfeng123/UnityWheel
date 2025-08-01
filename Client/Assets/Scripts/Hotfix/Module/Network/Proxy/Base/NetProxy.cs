
using Protocol;

public abstract  class Singleton<T> where  T:class, new()
{
    protected static T _instance;
    private static readonly object _lock = new object();
    public static T Instance {
        get
        {
            if (_instance == null && _lock != null)
                _instance = new T();
            return _instance;
        }
    }
}


namespace Xicheng.tcp
{
    /// <summary>
    /// 网络消息代理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NetProxy<T> : Singleton<T>, IProxy where T : class, new()
    {
        protected void RegisterMsg(MessageId  msgId, TcpMsgCallBack callback)
        {
            TcpNet.Inst.RegisterMsg((int)msgId,callback);
        }
        
        /// <summary>
        /// 注销消息
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="callBack"></param>
        protected void UnRegisterMsg(MessageId msgId,TcpMsgCallBack callback)
        {
            TcpNet.Inst.UnRegisterMsg((int)msgId,callback);
        }


        protected void SendMsg( MessageId msgId, byte[] content,uint terminalId = 1)
        {
            TcpNet.Inst.SendAsync((int)msgId, content);
        }
    }
}