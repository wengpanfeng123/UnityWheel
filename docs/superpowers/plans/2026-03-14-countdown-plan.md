# 倒计时功能实现计划

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现一个通用可复用的 Unity 倒计时 UI 组件，支持多倒计时、自动格式化、事件回调

**Architecture:** 使用独立管理器模式，复用现有 TimerManager，通过事件驱动 UI 更新

**Tech Stack:** Unity, C#, TextMeshPro

---

## 文件结构

```
Client/Assets/Scripts/Main/Utility/Countdown/
├── CountdownData.cs      # 数据模型
├── CountdownManager.cs   # 管理器（单例）
└── CountdownUI.cs       # UI 组件
```

---

## Chunk 1: 基础数据模型

### Task 1: 创建 CountdownData 数据类

**Files:**
- Create: `Client/Assets/Scripts/Main/Utility/Countdown/CountdownData.cs`

- [ ] **Step 1: 创建目录**

```bash
mkdir -Force "Client/Assets/Scripts/Main/Utility/Countdown"
```

- [ ] **Step 2: 编写 CountdownData.cs**

```csharp
using System;
using UnityEngine;

namespace xicheng.utility
{
    public class CountdownData
    {
        private float _remainingSeconds;
        private bool _isRunning;
        private bool _isFinished;
        private string _id;

        public string Id => _id;
        public float RemainingSeconds => _remainingSeconds;
        public bool IsRunning => _isRunning;
        public bool IsFinished => _isFinished;

        public event Action<string> OnUpdate;
        public event Action<string> OnComplete;

        public CountdownData(string id, float durationSeconds)
        {
            _id = id;
            _remainingSeconds = durationSeconds;
            _isRunning = true;
            _isFinished = false;
        }

        public void Update(float deltaSeconds)
        {
            if (!_isRunning || _isFinished) return;

            _remainingSeconds -= deltaSeconds;

            if (_remainingSeconds <= 0)
            {
                _remainingSeconds = 0;
                _isRunning = false;
                _isFinished = true;
                OnComplete?.Invoke(_id);
            }
            else
            {
                OnUpdate?.Invoke(_id);
            }
        }

        public void SetRunning(bool running)
        {
            _isRunning = running;
        }
    }
}
```

- [ ] **Step 3: 提交代码**

```bash
git add Client/Assets/Scripts/Main/Utility/Countdown/CountdownData.cs
git commit -m "feat: 添加 CountdownData 倒计时数据类"
```

---

## Chunk 2: 倒计时管理器

### Task 2: 创建 CountdownManager 管理器

**Files:**
- Create: `Client/Assets/Scripts/Main/Utility/Countdown/CountdownManager.cs`

- [ ] **Step 1: 编写 CountdownManager.cs**

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using xicheng.utility;

namespace xicheng.utility
{
    public class CountdownManager : MonoSingleton<CountdownManager>
    {
        private Dictionary<string, CountdownData> _countdowns = new Dictionary<string, CountdownData>();
        private int _idCounter = 0;
        private Timer _updateTimer;

        public override void Init()
        {
            base.Init();
            _updateTimer = TimerManager.Instance.AddLoopTimer(1f, OnTick);
        }

        private void OnTick()
        {
            List<string> finishedIds = new List<string>();

            foreach (var kvp in _countdowns)
            {
                if (kvp.Value.IsRunning && !kvp.Value.IsFinished)
                {
                    kvp.Value.Update(1f);

                    if (kvp.Value.IsFinished)
                    {
                        finishedIds.Add(kvp.Key);
                    }
                }
            }

            foreach (var id in finishedIds)
            {
                RemoveCountdown(id);
            }
        }

        public CountdownData CreateCountdown(float durationSeconds, Action onComplete = null)
        {
            string id = GenerateId();
            var countdown = new CountdownData(id, durationSeconds);

            if (onComplete != null)
            {
                countdown.OnComplete += _ => onComplete();
            }

            _countdowns[id] = countdown;
            return countdown;
        }

        public CountdownData GetCountdown(string id)
        {
            return _countdowns.TryGetValue(id, out var countdown) ? countdown : null;
        }

        public void RemoveCountdown(string id)
        {
            if (_countdowns.ContainsKey(id))
            {
                _countdowns.Remove(id);
            }
        }

        private string GenerateId()
        {
            _idCounter++;
            return $"countdown_{_idCounter}_{DateTime.Now.Ticks}";
        }

        private void OnDestroy()
        {
            if (_updateTimer != null)
            {
                TimerManager.Instance.RemoveTimer(_updateTimer);
            }
            _countdowns.Clear();
        }
    }
}
```

- [ ] **Step 2: 提交代码**

```bash
git add Client/Assets/Scripts/Main/Utility/Countdown/CountdownManager.cs
git commit -m "feat: 添加 CountdownManager 倒计时管理器"
```

---

## Chunk 3: UI 组件

### Task 3: 创建 CountdownUI 组件

**Files:**
- Create: `Client/Assets/Scripts/Main/Utility/Countdown/CountdownUI.cs`

- [ ] **Step 1: 编写 CountdownUI.cs**

```csharp
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace xicheng.utility
{
    public class CountdownUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Text _legacyText;

        private CountdownData _countdown;
        private string _countdownId;

        public string CountdownId => _countdownId;

        private void Awake()
        {
            if (_text == null)
                _text = GetComponent<TextMeshProUGUI>();
            if (_legacyText == null && _text == null)
                _legacyText = GetComponent<Text>();
        }

        public void Bind(string countdownId)
        {
            _countdownId = countdownId;
            _countdown = CountdownManager.Instance.GetCountdown(countdownId);

            if (_countdown != null)
            {
                _countdown.OnUpdate += OnCountdownUpdate;
                _countdown.OnComplete += OnCountdownComplete;
                UpdateDisplay(_countdown.RemainingSeconds);
            }
        }

        public void Bind(CountdownData countdown)
        {
            _countdown = countdown;
            _countdownId = countdown.Id;

            _countdown.OnUpdate += OnCountdownUpdate;
            _countdown.OnComplete += OnCountdownComplete;
            UpdateDisplay(_countdown.RemainingSeconds);
        }

        private void OnCountdownUpdate(string id)
        {
            if (id == _countdownId && _countdown != null)
            {
                UpdateDisplay(_countdown.RemainingSeconds);
            }
        }

        private void OnCountdownComplete(string id)
        {
            if (id == _countdownId)
            {
                UpdateDisplay(0);
            }
        }

        private void UpdateDisplay(float remainingSeconds)
        {
            string formatted = FormatTime(remainingSeconds);

            if (_text != null)
                _text.text = formatted;
            else if (_legacyText != null)
                _legacyText.text = formatted;
        }

        public static string FormatTime(float seconds)
        {
            int totalSeconds = Mathf.CeilToInt(seconds);

            if (totalSeconds < 60)
            {
                return totalSeconds.ToString();
            }

            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int secs = totalSeconds % 60;

            if (hours > 0)
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, secs);
            }
            else
            {
                return string.Format("{0:D2}:{1:D2}", minutes, secs);
            }
        }

        private void OnDestroy()
        {
            if (_countdown != null)
            {
                _countdown.OnUpdate -= OnCountdownUpdate;
                _countdown.OnComplete -= OnCountdownComplete;
            }
        }
    }
}
```

- [ ] **Step 2: 提交代码**

```bash
git add Client/Assets/Scripts/Main/Utility/Countdown/CountdownUI.cs
git commit -m "feat: 添加 CountdownUI 倒计时显示组件"
```

---

## Chunk 4: 测试验证

### Task 4: 创建测试场景

**Files:**
- Modify: `Client/Assets/Scenes/TestScene1.unity` (添加测试 UI)
- Create: `Client/Assets/Scripts/Main/Utility/Countdown/CountdownTest.cs`

- [ ] **Step 1: 创建测试脚本 CountdownTest.cs**

```csharp
using UnityEngine;
using xicheng.utility;

public class CountdownTest : MonoBehaviour
{
    [SerializeField] private CountdownUI _countdownUI;
    [SerializeField] private float _duration = 60f;

    public void StartCountdown()
    {
        var countdown = CountdownManager.Instance.CreateCountdown(_duration, () => {
            Debug.Log("倒计时结束!");
        });

        if (_countdownUI != null)
        {
            _countdownUI.Bind(countdown);
        }
    }

    private void Start()
    {
        StartCountdown();
    }
}
```

- [ ] **Step 2: 提交代码**

```bash
git add Client/Assets/Scripts/Main/Utility/Countdown/CountdownTest.cs
git commit -m "feat: 添加倒计时测试脚本"
```

---

## 使用说明

### 方式一：代码创建

```csharp
// 1. 创建倒计时 (60秒)
var countdown = CountdownManager.Instance.CreateCountdown(60f, () => {
    Debug.Log("倒计时结束!");
});

// 2. 绑定 UI
countdownUI.Bind(countdown);
```

### 方式二：Inspector 绑定

1. 在场景中创建 Canvas
2. 添加 TextMeshPro - Text (UI) 对象
3. 添加 CountdownUI 组件到该对象
4. 添加 CountdownTest 组件
5. 将 CountdownUI 拖拽到 CountdownTest 的 _countdownUI 字段

---

**Plan complete and saved to `docs/superpowers/plans/2026-03-14-countdown-plan.md`. Ready to execute?**
