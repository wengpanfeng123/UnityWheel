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
