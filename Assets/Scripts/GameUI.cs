using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Movement UI")]
    [SerializeField] private Slider _movementSlider;
    [SerializeField] private TMP_Text _movementText;

    [Header("Timer UI")]
    [SerializeField] private Slider _timerSlider;
    [SerializeField] private TMP_Text _timerText;

    [Header("Turn Indicator")]
    [SerializeField] private TMP_Text _turnText;

    void Update()
    {
        UpdateMovementUI();
        UpdateTimerUI();
        UpdateTurnIndicator();
    }

    private void UpdateMovementUI()
    {
        Worm currentWorm = WormManager.Instance?.CurrentWorm;

        if (currentWorm == null)
        {
            if (_movementSlider != null) _movementSlider.value = 0;
            if (_movementText != null) _movementText.text = "-- / --";
            return;
        }

        float remaining = currentWorm.DistanceRemaining;
        float max = currentWorm.maxMoveDistance;
        float percent = 1f - currentWorm.DistancePercent;

        if (_movementSlider != null)
            _movementSlider.value = percent;

        if (_movementText != null)
            _movementText.text = $"{remaining:F1} / {max:F1}";
    }

    private void UpdateTimerUI()
    {
        if (TimerController.Instance == null)
            return;

        float currentTime = TimerController.Instance.GetCurrentTime();
        float maxTime = TimerController.Instance.GetTurnDuration();
        float percent = currentTime / maxTime;

        if (_timerSlider != null)
            _timerSlider.value = percent;

        if (_timerText != null)
            _timerText.text = $"{Mathf.CeilToInt(currentTime)}s";
    }

    private void UpdateTurnIndicator()
    {
        if (_turnText == null)
            return;

        Worm currentWorm = WormManager.Instance?.CurrentWorm;

        if (currentWorm == null)
        {
            _turnText.text = "Waiting...";
        }
        else
        {
            _turnText.text = $"{currentWorm.gameObject.name}'s Turn";
        }
    }
}
