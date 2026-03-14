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
