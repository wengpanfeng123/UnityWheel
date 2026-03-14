using System.Collections.Generic;

namespace Xicheng.Archive
{
    /// <summary>
    /// 使用SQL数据库作为数据存储实现（预留，未来可扩展）
    /// </summary>
    public class SqlDataStorage : IDataStorage
    {
        public Dictionary<string, object> LoadGameData(string saveKey)
        {
            // TODO: 这里实现从SQL读取逻辑
            return new Dictionary<string, object>();
        }

        public void SaveLocalData(string saveKey, Dictionary<string, object> data)
        {
            // TODO: 这里实现保存到SQL逻辑
        }
    }
}