using Xicheng.Audio;

namespace Hotfix
{
    public class EnterMainSystem:ILogic
    {
      
        public void OnStartUp()
        {
            
            //播放音频
            AudioManager.Inst.PlayBGM("bgmHall.mp3",true);
        }
        
        
        
        

        public void OnAppPause(bool isPause)
        {
           
        }

        public void OnClose()
        {
           
        }

        public void OnAppQuit()
        {
 
        }
    }
}