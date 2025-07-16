
public interface ILogic
{
    /// <summary>
    /// 是否在游戏启动时自动初始化
    /// true: 游戏启动时自动初始化
    /// false: 延迟初始化（第一次使用时初始化）
    /// </summary>
    bool InitStartUp { get; }
    void OnInit();
    
    void OnRelease();
}

public interface IUpdateLogic : ILogic
{
    void OnUpdate(float deltaTime);
}

public interface IGameLogic:IUpdateLogic
{
}