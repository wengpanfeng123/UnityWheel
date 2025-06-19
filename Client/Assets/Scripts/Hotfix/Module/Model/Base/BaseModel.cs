namespace Hotfix.Module.Model.Base
{
    //数据基类
    public abstract class ModelBase
    {
        private bool _dirty;
        private bool _init;
        
        public bool IsDirty => _dirty;

        internal void Startup()
        {
            if (_init)
                return;
            _init = true;
        }
    
        /// <summary>
        /// 加载存档
        /// </summary>
        protected virtual void Load() { }
        
        //保存存档
        protected abstract void Save();

        //启动时调用(可选)
        public virtual void OnStartup() { }
        //销毁时调用(可选)
        public virtual void OnRelease() { }
        
        
        
        /// <summary>
        /// 手动保存（仅脏时）
        /// </summary>
        public void SaveIfDirty()
        {
            if (_dirty)
            {
                Save();
                _dirty = false;
            }
        }

        /// <summary>
        /// 标记当前数据脏
        /// </summary>
        public void MarkDirty()
        {
            _dirty = true;
        }
    }
}