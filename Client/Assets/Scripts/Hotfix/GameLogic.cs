using System.Collections.Generic;
using xicheng.ui;

namespace Hotfix
{
    public static class GameLogic
    {
        private static List<IHotUpdateGameLogic> _gameLogics = new();

        public static void  OnInit()
        {
            _gameLogics.Add(DataTableManager.Inst);
            _gameLogics.Add(UIManager.Inst);
            //TODO:添加不同的逻辑类。

            foreach (var logic in _gameLogics)
            {
                logic.OnInit();
            }
        }

        public static void OnUpdate(float deltaTime)
        {
            foreach (var logic in _gameLogics)
            {
                logic.OnUpdate(deltaTime);
            }
        }

        public static void OnRelease()
        {
            foreach (var logic in _gameLogics)
            {
                logic.OnRelease();
            }
            _gameLogics.Clear();
            _gameLogics = null;
        }
    }
}