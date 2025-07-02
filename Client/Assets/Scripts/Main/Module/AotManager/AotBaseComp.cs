using System;
using UnityEngine;

namespace xicheng.aot
{
    public abstract class AotBaseComp:MonoBehaviour
    {
        protected void Awake()
        {
            AotComponentManager.RegisterComponent(this);
        }

        public virtual void ReStart()
        {
            
        }

        public virtual void Close(CloseType closeType)
        {
            
        }
    }
}