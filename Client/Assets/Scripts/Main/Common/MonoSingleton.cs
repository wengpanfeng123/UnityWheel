/// <summary>
/// Generic Mono singleton.
/// </summary>
using UnityEngine;

namespace xicheng.common
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
	                }
	            }
	            return m_Instance;
	        }
	    }

	    protected virtual void Awake(){
	   
	        if( m_Instance == null ){
	            m_Instance = this as T;
	            DontDestroyOnLoad(this.gameObject);
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