using Hotfix.Model;
using Xicheng.Archive;
using Xicheng.Datable;

namespace Hotfix
{
    /// <summary>
    /// 数据总模块
    /// </summary>
    public class DataModule:GameModule
    {
        private readonly ModelManager _model = new();
        public override void OnStartUp()
        {
            base.OnStartUp();
            GameArchive.OnStartUp();    //初始化游戏存档数据
            _model.OnStartUp();
            DataTableManager.Inst.OnStartUp();
        }

        public override void OnAppPause(bool isPause)
        {
            base.OnAppPause(isPause);
            _model.OnAppPause(isPause);
        }
        
        public override void OnAppQuit()
        {
            base.OnAppQuit();
            _model.OnAppQuit();
            GameArchive.Close();
        }

        public T GetModel<T>() where T:BaseModel, new()
        {
            return _model.GetModel<T>();
        }
    }
}