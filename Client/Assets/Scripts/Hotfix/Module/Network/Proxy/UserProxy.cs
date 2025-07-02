using Google.Protobuf;
using Protocol;

namespace xicheng.tcp
{
    public class UserProxy:NetProxy<UserProxy>,IHotUpdateGameLogic
    {
        public void OnInit()
        {
            RegisterMsg(MessageId.MsgidC2SCreatePlayer, S2C_ChapterUpdate);
        }

        public void C2SCreatePlayer(string name, int roleId)
        { 
            C2SChapterInfo  c2SChapterInfo = new C2SChapterInfo();
            c2SChapterInfo.ChapterTid = 1;
            //序列化对象
            byte[] bytes = c2SChapterInfo.ToByteArray();//将对象进行序列化
            SendMsg(MessageId.MsgidC2SCheckRoleName,  bytes);
        }
        
        private void S2C_ChapterUpdate(uint msgId, byte[] body, int terminalId)
        {
            //将序列化的数据进行反序列化
            var parsedRequest = (S2CChapterUpdate)S2CChapterUpdate.Descriptor.Parser.ParseFrom(body);
            
            //TODO:逻辑处理
            
        }

    

        public void OnUpdate(float deltaTime)
        {
             
        }

        public void OnRelease()
        {
         
        }
    }
}