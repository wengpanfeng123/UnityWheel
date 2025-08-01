
using Xicheng.Archive;
using Xicheng.UI;

namespace Hotfix.Model
{

    public class PlayerData
    {
        public string Name;
        public int Age;
        public float Height;
    }

    public class PlayerModel:BaseModel
    {
        private PlayerData _playerData;
        private readonly string _modelKey = nameof(PlayerModel);
        
        public override void OnStartup()
        {
            _playerData = GameArchive.GetData<PlayerData>(_modelKey);
           // GameManager.GetModel<PlayerModel>();
           // GameManager.GetSystem<MyTestSystem>();
        }
        
        public override void Save()
        {
            GameArchive.SetData(_modelKey, _playerData);
        }
    }
}