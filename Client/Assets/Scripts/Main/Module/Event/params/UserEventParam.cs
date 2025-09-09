namespace Module.Event
{
	public class DefaultParam : IEventParam
	{
		public int Id;
		public string Name;
		public DefaultParam(int eventId)
		{
			Id = eventId;
		}
		public DefaultParam(string name) => Name = name;
	}

	public class UserEventParam
    {
        	/// <summary>
        	/// 用户尝试再次初始化资源包
        	/// </summary>
        	public class UserTryInitialize : IEventParam
        	{
            }
        
        	/// <summary>
        	/// 用户开始下载网络文件
        	/// </summary>
        	public class UserBeginDownloadWebFiles : IEventParam
        	{
            }
        
        	/// <summary>
        	/// 用户尝试再次更新静态版本
        	/// </summary>
        	public class UserTryUpdatePackageVersion : IEventParam
        	{
            }
        
        	/// <summary>
        	/// 用户尝试再次更新补丁清单
        	/// </summary>
        	public class UserTryUpdatePatchManifest : IEventParam
        	{
            }
        
        	/// <summary>
        	/// 用户尝试再次下载网络文件
        	/// </summary>
        	public class UserTryDownloadWebFiles : IEventParam
        	{
            }
    }
}