using System.Collections.Generic;

namespace xicheng.archive
{
    /// <summary>
    /// 游戏存档存取接口，支持本地存储、SQL存储等扩展
    /// </summary>
    public interface IDataStorage
    {
        Dictionary<string, object> LoadGameData(string saveKey);
        void SaveLocalData(string saveKey, Dictionary<string, object> data);
    }
}