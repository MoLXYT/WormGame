using System.Collections;
using UnityEngine;

public class WormManager : MonoBehaviour
{
    public static WormManager Instance;

    [SerializeField] private float _turnTransitionDelay = 2f;

    private Worm[] _worms;
    private Transform _wormCamera;
    private int _currWorm = -1;

    // Public getter for current worm (useful for UI)
    public Worm CurrentWorm => (_currWorm >= 0 && _currWorm < _worms.Length) ? _worms[_currWorm] : null;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        _worms = FindObjectsOfType<Worm>();
        _wormCamera = Camera.main.transform;

        // Assign IDs
        for (int i = 0; i < _worms.Length; i++)
        {
            _worms[i].wormID = i;
        }

        // Start first turn
        NextWorm();
    }

    public bool IsMyTurn(int id)
    {
        return id == _currWorm;
    }

    public void NextWorm()
    {
        StartCoroutine(NextWormCoroutine());
    }

    private IEnumerator NextWormCoroutine()
    {
        // Disable current worm's turn
        _currWorm = -1;

        // Wait for transition
        yield return new WaitForSeconds(_turnTransitionDelay);

        // Find next alive worm
        int nextWorm = FindNextAliveWorm();

        // Check for win condition (only one player left or none)
        if (nextWorm == -1)
        {
            Debug.Log("Game Over! No worms left alive.");
            yield break;
        }

        // Start next turn
        _currWorm = nextWorm;

        // Reset the new worm's movement distance
        _worms[_currWorm].StartTurn();

        // Reset timer
        TimerController.Instance.ResetTime();

        // Move camera to follow new worm
        _wormCamera.SetParent(_worms[_currWorm].transform);
        _wormCamera.localPosition = Vector3.back * 10;

        Debug.Log($"It's now {_worms[_currWorm].gameObject.name}'s turn!");
    }

    private int FindNextAliveWorm()
    {
        int startIndex = _currWorm < 0 ? 0 : _currWorm;
        int nextWorm = startIndex;
        int attempts = 0;

        do
        {
            nextWorm++;
            if (nextWorm >= _worms.Length)
                nextWorm = 0;

            attempts++;

            // Prevent infinite loop if all worms are dead
            if (attempts > _worms.Length)
                return -1;

        } while (!_worms[nextWorm].IsAlive);

        return nextWorm;
    }
}
