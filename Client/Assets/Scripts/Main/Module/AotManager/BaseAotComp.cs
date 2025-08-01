using System;
using UnityEngine;

namespace Xicheng.aot
{
    public abstract class BaseAotComp:MonoBehaviour
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