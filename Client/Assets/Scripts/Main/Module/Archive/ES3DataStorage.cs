using System.Collections.Generic;
using Main.Module.Log;

namespace XiCheng.Archive
{
    /// <summary>
    /// 使用ES3作为数据持久化存储实现
    /// </summary>
    public class ES3DataStorage : IDataStorage
    {
        private string GetFilePath(string gameKey) => $"{gameKey}.data";
        private string GetBackupFilePath(string gameKey) => $"{gameKey}_Backup.data";


        public ES3DataStorage()
        {
            ES3.Init();
        }

        public Dictionary<string, object> LoadGameData(string saveKey)
        {
            string filePath = GetFilePath(saveKey);
            try
            {
                if (!ES3.FileExists(filePath))
                {
                    string backupPath = GetBackupFilePath(saveKey);
                    if (ES3.FileExists(backupPath))
                    {
                        ES3.CopyFile(backupPath, filePath);
                    }
                }

                if (ES3.FileExists(filePath))
                {
                    return ES3.Load<Dictionary<string, object>>(saveKey, filePath);
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"ES3 Load Exception: {e.Message}");

                if (ES3.FileExists(filePath))
                {
                    ES3.DeleteFile(filePath);

                    var emptyData = new Dictionary<string, object>();
                    SaveGameData(saveKey, emptyData);
                }
            }

            return new Dictionary<string, object>();
        }

        public void SaveGameData(string saveKey, Dictionary<string, object> data)
        {
            string filePath = GetFilePath(saveKey);
            try
            {
                ES3.Save(saveKey, data, filePath);

                if (ES3.FileExists(filePath))
                {
                    string backupPath = GetBackupFilePath(saveKey);
                    ES3.CopyFile(filePath, backupPath);
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"ES3 Save Exception: {e.Message}");
            }
        }
    }
}