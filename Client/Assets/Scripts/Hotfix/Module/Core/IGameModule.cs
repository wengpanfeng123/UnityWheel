 
namespace Hotfix
{
    /// <summary>
    /// 集成ILogic的类，自动被游戏管理器管理生命周期。
    /// </summary>
    public interface ILogic
    {
        /// <summary>
        /// 启动时初始化
        /// </summary>
        void OnStartUp();

        /// <summary>
        /// app暂停、恢复
        /// </summary>
        /// <param name="isPause"></param>
        void OnAppPause(bool isPause);

        /// <summary>
        /// app退出
        /// </summary>    
        void OnClose();

        /// <summary>
        /// app退出
        /// </summary>
        void OnAppQuit();

    }

    public interface IUpdateLogic : ILogic
    {
        void OnUpdate(float deltaTime);
    }

    public interface IGameModule : IUpdateLogic
    {

    }


    /// <summary>
    /// 游戏大模块
    /// </summary>
    public abstract class GameModule : IGameModule
    {

        public virtual void OnStartUp()
        {
        }

        public virtual void OnUpdate(float deltaTime)
        {
        }

        public virtual void OnAppPause(bool isPause)
        {
        }

        public virtual void OnClose()
        {
        }

        public virtual void OnAppQuit()
        {
        }

        public virtual void OnDestroy()
        {
        }
    }
}