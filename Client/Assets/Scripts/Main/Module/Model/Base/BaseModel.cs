namespace Hotfix.Model
{
    //数据基类
    public abstract class BaseModel
    {
        private bool _dirty;
        private bool _init;
        
        public bool IsDirty => _dirty;

        public void Startup()
        {
            if (_init)
                return;
            _init = true;
            OnStartup();
        }
    
        //保存存档
        public abstract void Save();

        /// <summary>
        /// 模块数据加载时调用。
        /// </summary>
        public abstract void OnStartup();
        
        //销毁时调用(可选)
        public virtual void OnRelease() { }
 
    }
}