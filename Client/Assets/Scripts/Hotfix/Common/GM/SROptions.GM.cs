using System;
using System.ComponentModel;
using System.Diagnostics;
#if !DISABLE_SRDEBUGGER
using SRDebugger;
using SRDebugger.Services;
#endif

public partial class SROptions
{
 
    [Category("关卡"), DisplayName("<color=green>关卡胜利</color>"), Sort(1)]
    public void GameWin()
    {
    }

    [Category("游戏管理"), NumberRange(1, 100), DisplayName("增加血量")]
    public int HP { get; set; }


    private int level;

    [Category("关卡"), DisplayName("关卡等级"), Sort(2)]
    public int ChangeLevel
    {
        get { return level; }
        set => level = value;
    }
}