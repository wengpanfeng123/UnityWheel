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
