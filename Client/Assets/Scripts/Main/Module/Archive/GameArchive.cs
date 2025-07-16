
using System.Collections.Generic;
using xicheng.log.Log;
using UnityEngine;

namespace xicheng.archive
{
    public enum StorageType
    {
        ES3,
        Sql,
    }

    /// <summary>
    /// 游戏存档数据
    /// </summary>
    public static class GameArchive
    {
        private static readonly string SaveKey = "xicheng.game";
        private static IDataStorage _dataStorage;
        private static Dictionary<string, Dictionary<string, object>> _dataDict = new();
        private static bool _isInit = false;
        public static void OnInit(StorageType storageType = StorageType.ES3)
        {
            if (_isInit)
                return;
            _isInit = false;
            
            _dataStorage = storageType switch
            {
                StorageType.ES3 => new ES3DataStorage(),
                StorageType.Sql => new SqlDataStorage(),
                _ => _dataStorage
            };
            
            LoadData(SaveKey);
        }

        /// <summary>
        /// 加载存档数据到内存。
        /// </summary>
        /// <param name="saveKey"></param>
        private static void LoadData(string saveKey)
        {
            var archive = _dataStorage.LoadGameData(SaveKey);
            if (string.IsNullOrEmpty(saveKey))
                return;

            if (!_dataDict.ContainsKey(saveKey))
            {
                var gameData = _dataStorage.LoadGameData(saveKey) ?? new Dictionary<string, object>();
                _dataDict.Add(saveKey, gameData);
            }
        }

        /// <summary>
        /// 获取model数据
        /// </summary>
        public static T GetData<T>(string modelKey)
        {
            string saveKey = SaveKey;
            if (string.IsNullOrEmpty(saveKey) || string.IsNullOrEmpty(modelKey))
                return default;
            
            if (_dataDict.TryGetValue(saveKey, out var gameData) && 
                gameData.TryGetValue(modelKey, out var value))
            {
                return (T)value;
            }

            return default;
        }
        
        /// <summary>
        /// 设置(内存)数据。(未保存)
        /// </summary>
        public static T SetData<T> (string modelKey, T value)
        { 
            if ( string.IsNullOrEmpty(modelKey))
                return default;
 
            if (!_dataDict.TryGetValue(SaveKey, out var gameData))
            {
                ULog.Error($"SetData Error: SaveKey {SaveKey} not loaded");
                return default;
            }
            gameData[modelKey] = value;
            return value;
        }

        
        /// <summary>
        /// 数据保存到本地
        /// </summary>
        /// <param name="saveKey"></param>
        public static void SaveLocal()
        {
            var saveKey = SaveKey;
            if (string.IsNullOrEmpty(saveKey))
                return;

            lock (_dataDict)
            {
                if (_dataDict.TryGetValue(saveKey, out var gameData))
                {
                    _dataStorage.SaveLocalData(saveKey, gameData);
                    ULog.Info("[Archive] save ",Color.cyan);
                }
            }
        }
        
        /// <summary>
        /// 关闭存档
        /// </summary>
        public static void Close()
        {
            SaveLocal();
            _isInit = false;
        }
    }
}