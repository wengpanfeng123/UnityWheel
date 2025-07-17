
using xicheng.archive;
using Xicheng.UI;

namespace Hotfix.Model
{

    public class PlayerData
    {
        public string Name;
        public int Age;
        public float Height;
    }

    public class PlayerModel:ModelBase
    {
        private PlayerData _playerData;
        private readonly string _modelKey = nameof(PlayerModel);
        public override void OnStartup()
        {
            _playerData = GameArchive.GetData<PlayerData>(_modelKey);
           var t = HotfixEntry.GetLogic<UIManager>().UIRoot;
           var st = HotfixEntry.GetLogic<ModelManager>();
           st.GetModel<PlayerModel>();
        }
        
        public override void Save()
        {
            GameArchive.SetData(_modelKey, _playerData);
        }
    }
}