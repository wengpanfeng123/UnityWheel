using System.Collections.Generic;
using Hotfix;
using Hotfix.Model;
using Xicheng.Archive;

namespace Hotfix
{


    /// <summary>
    /// 不同的功能模块数据管理
    /// </summary>
    public class ModelManager : ILogic
    {
        private string _gameKey = "xicheng.game";
        private readonly Dictionary<string, BaseModel> _modelDict = new();


        public bool InitStartUp => true;

        public void OnStartUp()
        {
            //提前注册或者使用时注册
        }


        /// <summary>
        /// 获取一个Model实例
        /// </summary>
        public T GetModel<T>() where T : BaseModel, new()
        {
            string key = typeof(T).FullName;
            return RegisterModel<T>(key);
        }

        private T RegisterModel<T>(string modelName) where T : BaseModel, new()
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

            SaveData();
        }


        public void OnAppPause(bool isPause)
        {
            SaveData(isPause);
        }


        public void OnAppQuit()
        {
            SaveData(true);
        }
    }
}