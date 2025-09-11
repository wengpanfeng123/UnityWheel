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
            _model.OnStartUp();
        }

        public override void OnAppPause(bool isPause)
        {
            base.OnAppPause(isPause);
            _model.OnAppPause(isPause);
        }
        
        public override void OnAppQuit()
        { 
            _model.OnAppQuit();
        }

        public T GetModel<T>() where T:BaseModel, new()
        {
            return _model.GetModel<T>();
        }
    }
}