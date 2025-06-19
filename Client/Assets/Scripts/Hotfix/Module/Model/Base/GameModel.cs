using System.Collections.Generic;
using XiCheng.Archive;

namespace Hotfix.Module.Model.Base
{
    //游戏模块数据管理
    public class GameModel
    {
        private string _gameKey = "xicheng.game";
        private readonly Dictionary<string, ModelBase> _modelDict = new();
            
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
        /// 进入指定游戏（加载数据）
        /// </summary>
        public void EnterGame()
        {
            //初始化存档
            GameArchive.Init();
        }

        /// <summary>
        /// 退出指定游戏（保存数据并回收模型）
        /// </summary>
        public void ExitGame(string gameKey)
        {
            foreach (var model in _modelDict.Values)
            {
                model.OnRelease();
            }
            GameArchive.Close();
        }

        /// <summary>
        /// 保存指定游戏的数据
        /// </summary>
        public void SaveGameData(bool writeToDisk = false)
        {
            foreach (var model in _modelDict.Values)
            {
                //TODO:脏数据时保存。
                model.MarkDirty();
                model.SaveIfDirty();
            }
     
            if (writeToDisk)
            {
                GameArchive.SaveLocal();
            }
        }
        

        /// <summary>
        /// 保存所有脏的Model（如切后台时统一保存）
        /// </summary>
        public void SaveAllDirtyModels()
        {
            foreach (var model in _modelDict.Values)
            {
                if (model.IsDirty)
                {
                    model.SaveIfDirty();
                }
            }
        }

        public void OnRelease()
        {
            SaveGameData();
            foreach (var model in _modelDict.Values)
            {
                model.OnRelease();
            }
        }

    }
}