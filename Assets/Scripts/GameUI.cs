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

    [Header("Game Over UI (Optional)")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TMP_Text _winnerText;

    void Update()
    {
        // Check if game is over
        if (WormManager.Instance != null && WormManager.Instance.IsGameOver)
        {
            ShowGameOver();
            return;
        }

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
            _turnText.text = "Switching turns...";
        }
        else
        {
            _turnText.text = $"{currentWorm.gameObject.name}'s Turn";
        }
    }

    private void ShowGameOver()
    {
        // Show game over panel if we have one
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
        }

        // Update turn text to show game over
        if (_turnText != null)
        {
            _turnText.text = "GAME OVER - Press R to restart";
        }

        // Hide movement and timer UI
        if (_movementSlider != null)
            _movementSlider.value = 0;

        if (_movementText != null)
            _movementText.text = "GAME OVER";

        if (_timerSlider != null)
            _timerSlider.value = 0;

        if (_timerText != null)
            _timerText.text = "---";
    }
}
