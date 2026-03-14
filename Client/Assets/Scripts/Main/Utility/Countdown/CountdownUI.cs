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
