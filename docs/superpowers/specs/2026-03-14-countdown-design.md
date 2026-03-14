# 倒计时功能设计文档

## 1. 概述

设计一个通用可复用的 Unity 倒计时 UI 组件，支持多个倒计时同时运行、自动格式化和事件回调。

## 2. 需求

| 需求项 | 描述 |
|--------|------|
| 通用 UI 组件 | 多种场景可复用 |
| 自动格式选择 | 根据时间长度自动选择显示格式 |
| 倒计时结束回调 | 时间到触发特定行为 |
| 每秒更新 | 精度为秒级 |

## 3. 时间格式化规则

| 剩余时间范围 | 显示格式 | 示例 |
|-------------|----------|------|
| < 60 秒 | 仅秒数 | `30` |
| < 1 小时 | 分:秒 | `59:59` |
| >= 1 小时 | 时:分:秒 | `01:30:25` |

## 4. 架构设计

### 4.1 组件列表

| 组件 | 路径 | 职责 |
|------|------|------|
| `CountdownData` | `Scripts/Main/Utility/Countdown/CountdownData.cs` | 倒计时数据模型 |
| `CountdownManager` | `Scripts/Main/Utility/Countdown/CountdownManager.cs` | 管理器：创建/更新/销毁倒计时 |
| `CountdownUI` | `Scripts/Main/Utility/Countdown/CountdownUI.cs` | UI 组件：显示格式化时间 |

### 4.2 目录结构

```
Client/Assets/Scripts/Main/Utility/Countdown/
├── CountdownData.cs      # 数据模型
├── CountdownManager.cs   # 管理器（单例）
└── CountdownUI.cs        # UI 组件
```

## 5. 接口设计

### 5.1 CountdownData

```csharp
public class CountdownData
{
    public string Id { get; }           // 唯一标识
    public float RemainingSeconds { get; } // 剩余秒数
    public bool IsRunning { get; }       // 是否运行中
    public bool IsFinished { get; }      // 是否已结束

    public event Action<string> OnUpdate;   // (id) 每秒更新
    public event Action<string> OnComplete; // (id) 倒计时完成
}
```

### 5.2 CountdownManager

```csharp
public class CountdownManager : MonoSingleton<CountdownManager>
{
    // 创建倒计时
    public CountdownData CreateCountdown(float durationSeconds, Action onComplete = null);

    // 移除倒计时
    public void RemoveCountdown(string id);

    // 暂停/恢复 (可选扩展)
    public void Pause(string id);
    public void Resume(string id);
}
```

### 5.3 CountdownUI

```csharp
public class CountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public string CountdownId { get; set; }

    private void Awake()
    {
        if (_text == null)
            _text = GetComponent<TextMeshProUGUI>();
    }

    // 绑定倒计时
    public void Bind(string countdownId);
}
```

## 6. 事件流程

```
1. 创建倒计时
   CountdownManager.CreateCountdown(60f, () => Debug.Log("完成!"));

2. 绑定 UI
   CountdownUI.Bind(countdownId);

3. 运行时
   CountdownManager.Update() → 每秒触发 OnUpdate → UI 更新显示

4. 结束
   RemainingSeconds <= 0 → 触发 OnComplete → 执行回调 → 自动移除
```

## 7. 实现要点

1. **复用 TimerManager** - 使用现有的 `AddLoopTimer` 每秒更新
2. **自动格式化** - 在 CountdownUI 中根据剩余时间选择格式
3. **自动清理** - 倒计时结束后自动从管理器中移除
4. **ID 生成** - 使用 Guid 或自增 ID 确保唯一性

## 8. 使用示例

```csharp
// 1. 创建倒计时 (60秒)
var countdown = CountdownManager.Instance.CreateCountdown(60f, () => {
    Debug.Log("倒计时结束!");
});

// 2. 在 UI 脚本中绑定
countdownUI.CountdownId = countdown.Id;

// 或通过 Inspector 绑定
```

## 9. 扩展考虑

- 毫秒级精度（通过配置选择）
- 暂停/恢复功能
- 自定义格式字符串
- 倒计时列表管理 UI
