namespace Hotfix.Model
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
        
        
        
        /// <summary>
        /// 手动保存（仅脏时）
        /// </summary>
        // public void SaveIfDirty()
        // {
        //     if (_dirty)
        //     {
        //         Save();
        //         _dirty = false;
        //     }
        // }

        /// <summary>
        /// 标记当前数据脏
        /// </summary>
        // public void MarkDirty()
        // {
        //     _dirty = true;
        // }
    }
}