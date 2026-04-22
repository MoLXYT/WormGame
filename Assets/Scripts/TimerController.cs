using UnityEngine;

public class TimerController : MonoBehaviour
{
    public static TimerController Instance;

    [SerializeField] private float _turnDuration = 60f;

    private float _currentTime;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        _currentTime = _turnDuration;
    }

    void Update()
    {
        // Don't count down if game is over
        if (WormManager.Instance != null && WormManager.Instance.IsGameOver)
            return;

        // Don't count down if no active worm (transitioning)
        if (WormManager.Instance != null && WormManager.Instance.CurrentWorm == null)
            return;

        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0)
        {
            Debug.Log("Time's up! Switching turns...");
            ResetTime();
            WormManager.Instance.NextWorm();
        }
    }

    public void ResetTime()
    {
        _currentTime = _turnDuration;
    }

    public float GetCurrentTime()
    {
        return _currentTime;
    }

    public float GetTurnDuration()
    {
        return _turnDuration;
    }
}
