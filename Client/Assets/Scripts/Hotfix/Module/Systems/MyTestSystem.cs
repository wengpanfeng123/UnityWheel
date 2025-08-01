namespace Hotfix
{
    public class MyTestSystem:ILogic
    {
        public bool InitStartUp { get; }
        public void OnStartUp()
        {
            throw new System.NotImplementedException();
        }

        public void OnAppPause(bool isPause)
        {
            throw new System.NotImplementedException();
        }

        public void OnRelease()
        {
            throw new System.NotImplementedException();
        }

        public void OnAppQuit()
        {
            throw new System.NotImplementedException();
        }
    }
}