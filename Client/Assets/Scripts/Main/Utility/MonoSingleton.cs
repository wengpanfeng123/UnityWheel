/// <summary>
/// Generic Mono singleton.
/// </summary>

using UnityEngine;

namespace Xicheng.Utility
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance = null;

        public static T Inst
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        _instance = new GameObject($"Singleton of {typeof(T).Name}", typeof(T)).GetComponent<T>();
                        _instance.InstanceInit();
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }


        public virtual void InstanceInit()
        {
        }


        private void OnApplicationQuit()
        {
            _instance = null;
        }
    }

    public class Singleton<T> where T : new()
    {
        private static object lockObj = new object();
        private static T _instance = default(T);

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                lock (lockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }

                return _instance;
            }
        }
    }
}