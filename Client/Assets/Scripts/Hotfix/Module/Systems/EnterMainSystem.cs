using Xicheng.Audio;
using Xicheng.Screens;
using Xicheng.UI;

namespace Hotfix
{
    public class EnterMainSystem:ILogic
    {
      
        public void OnStartUp()
        {
            //屏幕初始化
            ScreenManager.Inst.OnStartUp();
            //播放音频
            AudioManager.Inst.PlayBGM("bgmHall.mp3",true);
            //UI初始化
            UIManager.Inst.OnStartUp();
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