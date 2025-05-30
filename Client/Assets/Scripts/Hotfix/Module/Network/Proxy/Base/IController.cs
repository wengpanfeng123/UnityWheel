namespace xicheng.tcp
{
    public interface IController
    {
        void OnInit();

        void OnUpdate(float deltaTime);

        void OnRelease();
    }
}