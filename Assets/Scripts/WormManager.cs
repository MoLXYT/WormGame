using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WormManager : MonoBehaviour
{
    public static WormManager Instance;

    [Header("Turn Settings")]
    [SerializeField] private float _turnTransitionDelay = 2f;

    // Worm tracking
    private Worm[] _worms;
    private int _currWormIndex = -1;

    // Game state
    private bool _isGameOver = false;
    private bool _isTurnTransitioning = false;

    // Camera reference
    private Transform _mainCamera;

    // Public getters for UI
    public Worm CurrentWorm
    {
        get
        {
            if (_currWormIndex >= 0 && _currWormIndex < _worms.Length)
                return _worms[_currWormIndex];
            return null;
        }
    }

    public bool IsGameOver => _isGameOver;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Get camera reference
        _mainCamera = Camera.main.transform;

        // Find all worms in the scene
        _worms = FindObjectsOfType<Worm>();

        if (_worms.Length == 0)
        {
            Debug.LogError("No worms found in scene!");
            return;
        }

        Debug.Log($"Found {_worms.Length} worms in the scene.");

        // Assign unique IDs to each worm
        for (int i = 0; i < _worms.Length; i++)
        {
            _worms[i].wormID = i;
            Debug.Log($"Assigned ID {i} to {_worms[i].gameObject.name}");
        }

        // Start the first turn
        StartCoroutine(StartFirstTurn());
    }

    void Update()
    {
        // Restart game with R key
        if (_isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    private IEnumerator StartFirstTurn()
    {
        // Small delay to ensure everything is initialized
        yield return new WaitForSeconds(0.5f);

        // Find first alive worm and start their turn
        int firstWorm = FindNextAliveWorm(-1);

        if (firstWorm == -1)
        {
            Debug.LogError("No alive worms to start!");
            yield break;
        }

        _currWormIndex = firstWorm;
        _worms[_currWormIndex].StartTurn();
        TimerController.Instance.ResetTime();

        // Move camera
        MoveCameraToCurrentWorm();

        Debug.Log($"=== GAME START: {_worms[_currWormIndex].gameObject.name}'s Turn ===");
    }

    /// <summary>
    /// Checks if it's the specified worm's turn.
    /// Returns false if game is over or transitioning between turns.
    /// </summary>
    public bool IsMyTurn(int wormID)
    {
        // Not anyone's turn if game is over
        if (_isGameOver)
            return false;

        // Not anyone's turn during transition
        if (_isTurnTransitioning)
            return false;

        // Check if this worm's turn
        return wormID == _currWormIndex;
    }

    /// <summary>
    /// Called when current turn should end. Switches to next worm.
    /// </summary>
    public void NextWorm()
    {
        // Prevent multiple calls during transition
        if (_isTurnTransitioning || _isGameOver)
            return;

        StartCoroutine(NextWormCoroutine());
    }

    private IEnumerator NextWormCoroutine()
    {
        _isTurnTransitioning = true;

        Debug.Log($"--- {CurrentWorm?.gameObject.name}'s turn ended ---");

        // Disable current worm's turn
        int previousWorm = _currWormIndex;
        _currWormIndex = -1;

        // Wait for transition (let projectiles settle, etc.)
        yield return new WaitForSeconds(_turnTransitionDelay);

        // Check for winner before proceeding
        if (CheckForWinner())
        {
            _isTurnTransitioning = false;
            yield break;
        }

        // Find next alive worm
        int nextWorm = FindNextAliveWorm(previousWorm);

        if (nextWorm == -1)
        {
            // This shouldn't happen if CheckForWinner works correctly
            Debug.LogError("No alive worms found!");
            _isTurnTransitioning = false;
            yield break;
        }

        // Start next turn
        _currWormIndex = nextWorm;
        _worms[_currWormIndex].StartTurn();
        TimerController.Instance.ResetTime();

        // Move camera to new worm
        MoveCameraToCurrentWorm();

        _isTurnTransitioning = false;

        Debug.Log($"=== {_worms[_currWormIndex].gameObject.name}'s Turn ===");
    }

    private void MoveCameraToCurrentWorm()
    {
        if (CurrentWorm == null || _mainCamera == null)
            return;

        _mainCamera.SetParent(CurrentWorm.transform);
        _mainCamera.localPosition = new Vector3(0, 0, -10);
    }

    /// <summary>
    /// Finds the next alive worm after the given index.
    /// Returns -1 if no alive worms exist.
    /// </summary>
    private int FindNextAliveWorm(int afterIndex)
    {
        if (_worms == null || _worms.Length == 0)
            return -1;

        int startIndex = afterIndex + 1;
        int attempts = 0;

        for (int i = startIndex; attempts < _worms.Length; i++, attempts++)
        {
            // Wrap around to beginning
            if (i >= _worms.Length)
                i = 0;

            if (_worms[i] != null && _worms[i].IsAlive)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Called when a worm dies. Checks if there's a winner.
    /// </summary>
    public void OnWormDied(Worm deadWorm)
    {
        Debug.Log($"*** {deadWorm.gameObject.name} has been eliminated! ***");

        // Check for winner
        CheckForWinner();
    }

    /// <summary>
    /// Checks if only one worm remains alive.
    /// Returns true if game is over.
    /// </summary>
    public bool CheckForWinner()
    {
        if (_isGameOver)
            return true;

        int aliveCount = 0;
        Worm lastAlive = null;

        foreach (Worm worm in _worms)
        {
            if (worm != null && worm.IsAlive)
            {
                aliveCount++;
                lastAlive = worm;
            }
        }

        Debug.Log($"Alive worms remaining: {aliveCount}");

        if (aliveCount <= 1)
        {
            _isGameOver = true;
            _currWormIndex = -1;

            if (aliveCount == 1 && lastAlive != null)
            {
                Debug.Log($"");
                Debug.Log($"========================================");
                Debug.Log($"   WINNER: {lastAlive.gameObject.name}!");
                Debug.Log($"========================================");
                Debug.Log($"");
                Debug.Log($"Press R to restart the game.");
            }
            else
            {
                Debug.Log($"");
                Debug.Log($"========================================");
                Debug.Log($"   DRAW - Everyone is dead!");
                Debug.Log($"========================================");
                Debug.Log($"");
                Debug.Log($"Press R to restart the game.");
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Restarts the game by reloading the current scene.
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Gets the count of alive worms.
    /// </summary>
    public int GetAliveWormCount()
    {
        int count = 0;
        foreach (Worm worm in _worms)
        {
            if (worm != null && worm.IsAlive)
                count++;
        }
        return count;
    }
}
