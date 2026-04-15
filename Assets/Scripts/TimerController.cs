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
        _currentTime -= Time.deltaTime;
        if (_currentTime <= 0)
        {
            ResetTime();
            WormManager.Instance.NextWorm();
        }
    }

    public void ResetTime()
    {
        // FIXED: Now uses the Inspector value instead of hardcoded 60
        _currentTime = _turnDuration;
    }

    // NEW: Getter so UI can display the time
    public float GetCurrentTime()
    {
        return _currentTime;
    }

    // NEW: Getter for max time (for UI progress bar)
    public float GetTurnDuration()
    {
        return _turnDuration;
    }
}
