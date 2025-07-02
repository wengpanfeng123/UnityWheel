namespace Main.EventTest
{
	public class TestEvent1:EventBase
	{
		
	}

	//如果没有参数需要,就不需定义。
    public class TestEvent : IEvent
    {
        public string name =string.Empty;
        
        public TestEvent()
        {
            
        }
        public TestEvent(string name)
        {
            this.name = name;
        }


        public void Clear()
        {
	         
        }

        public void Acquire()
        {
	         
        }
    }

    public class PatchEventParam
    {
	    /// <summary>
	    /// 补丁包初始化失败
	    /// </summary>
	    public class InitializeFailed : IEventParam
	    {
	    }

	    /// <summary>
	    /// 补丁流程步骤改变
	    /// </summary>
	    public class PatchStatesChange : IEventParam
	    {
		    public string Tips;
		    public PatchStatesChange(string tips)
		    {
			    Tips = tips;
		    }
	    }

	    /// <summary>
	    /// 发现更新文件
	    /// </summary>
	    public class FoundUpdateFiles : IEventParam
	    {
		    public int TotalCount;
		    public long TotalSizeBytes;
		    public FoundUpdateFiles(int totalCount, long totalSizeBytes)
		    {
			    TotalCount = 1;
			    TotalSizeBytes = 2;
		    }
	    }

	    /// <summary>
	    /// 下载进度更新
	    /// </summary>
	    public class DownloadProgressUpdate : IEventParam
	    {
		    public int TotalDownloadCount;
		    public int CurrentDownloadCount;
		    public long TotalDownloadSizeBytes;
		    public long CurrentDownloadSizeBytes;
		    public DownloadProgressUpdate(int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
		    {
			    TotalDownloadCount = totalDownloadCount;
			    CurrentDownloadCount = currentDownloadCount;
			    TotalDownloadSizeBytes = totalDownloadSizeBytes;
			    CurrentDownloadSizeBytes = currentDownloadSizeBytes;
		    }
	    }

	    /// <summary>
	    /// 资源版本号更新失败
	    /// </summary>
	    public class PackageVersionUpdateFailed : IEventParam
	    {
	    }

	    /// <summary>
	    /// 补丁清单更新失败
	    /// </summary>
	    public class PatchManifestUpdateFailed : IEventParam
	    {
	    }

	    /// <summary>
	    /// 网络文件下载失败
	    /// </summary>
	    public class WebFileDownloadFailed : IEventParam
	    {
		    public string FileName;
		    public string Error;
		    public WebFileDownloadFailed(string fileName, string error)
		    {
			    FileName = fileName;
			    Error = error;
		    }
	    }
    }
}