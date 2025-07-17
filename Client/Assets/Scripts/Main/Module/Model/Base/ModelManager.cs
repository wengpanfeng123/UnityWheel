using System.Collections.Generic;
using Hotfix.Model;
using xicheng.archive;
using Xicheng.Utility;


//游戏模块数据管理
public class ModelManager : MonoSingleton<ModelManager>, ILogic
{
    private string _gameKey = "xicheng.game";
    private readonly Dictionary<string, ModelBase> _modelDict = new();


    public bool InitStartUp => true;

    public void OnInit()
    {
    }

    //按照模块去加载数据，本质上都是读取一个存档文件，读取文件内容，然后反序列化成对象。

    /// <summary>
    /// 获取一个Model实例
    /// </summary>
    public T GetModel<T>() where T : ModelBase, new()
    {
        string key = typeof(T).FullName;
        return RegisterModel<T>(key);
    }

    private T RegisterModel<T>(string modelName) where T : ModelBase, new()
    {
        if (!_modelDict.TryGetValue(modelName, out var modelInstance))
        {
            modelInstance = new T();
            _modelDict.Add(modelName, modelInstance);
        }
        var model = modelInstance as T;
        model?.Startup();
        return model;
    }

    /// <summary>
    ///  保存所有model数据
    /// </summary>
    /// <param name="writeToDisk">写入到磁盘保存</param>
    public void SaveData(bool writeToDisk = false)
    {
        foreach (var model in _modelDict.Values)
        {
            model.Save();
        }

        if (writeToDisk)
        {
            GameArchive.SaveLocal();
        }
    }


    public void OnRelease()
    {
        foreach (var model in _modelDict.Values)
        {
            model.OnRelease();
        }

        SaveData(false);
    }
}