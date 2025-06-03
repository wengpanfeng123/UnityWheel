/// <summary>
/// Generic Mono singleton.
/// </summary>
using UnityEngine;

namespace xicheng.utility
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>{
		
		private static T m_Instance = null;
	    
		public static T Inst{
	        get{
				if( m_Instance == null ){
	            	m_Instance = FindObjectOfType(typeof(T)) as T;
	                if( m_Instance == null )
	                {
	                    m_Instance = new GameObject( $"Singleton of {typeof(T).Name}", typeof(T)).GetComponent<T>();
						 m_Instance.Init();
						 DontDestroyOnLoad(m_Instance.gameObject);
	                }
	            }
	            return m_Instance;
	        }
	    }
	    

	    public virtual void Init(){}
	 

	    private void OnApplicationQuit(){
	        m_Instance = null;
	    }
	}
	
	public class Singleton<T> where T : new()
	{
		private  static object lockObj = new object();
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