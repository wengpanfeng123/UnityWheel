using System.IO;
using Google.Protobuf;
using Protocol;
using UnityEngine;

namespace Xicheng.tcp
{
    public class TestProto:MonoBehaviour
    {
        private void Start()
        {
            //C2S
            C2SChapterInfo  c2SChapterInfo = new C2SChapterInfo();
            c2SChapterInfo.ChapterTid = 1;
            //序列化对象
            byte[] bytes = c2SChapterInfo.ToByteArray();//将对象进行序列化
            //网络传输 Net.Send(bytes);
            Debug.Log(bytes);

            byte[] bytesArr = new byte[1024];
            //S2C
            S2CChapterUpdate update = (S2CChapterUpdate)S2CChapterUpdate.Descriptor.Parser.ParseFrom(bytes);
            Debug.Log(update);


            C2SChapterInfo enter = new C2SChapterInfo();
            //EventMgr.Inst.Dispatch(EEventType.NetMsg, 1, enter);
        
            using (var ms = new MemoryStream())
            {
                enter.WriteTo(ms);
                TcpNet.Inst.SendAsync((int)MessageId.MsgidA2CAuth, ms.ToArray());
            }
        }
    }
}