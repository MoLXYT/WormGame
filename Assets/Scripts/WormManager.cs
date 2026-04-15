using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormManager : MonoBehaviour
{
    public static WormManager Instance;
    [SerializeField] private int _nextWormTurnTime;

    private Worm[] _worms;
    private Transform _wormCamera;
    private int _currWorm;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        _worms = GameObject.FindObjectsOfType<Worm>();
        _wormCamera = Camera.main.transform;

        for (int i = 0; i < _worms.Length; i++)
        {
            _worms[i].wormID = i;
        }
        NextWorm();
    }

    public bool IsMyTurn(int i)
    {
        return i == _currWorm;
    }

    public void NextWorm()
    {

        StartCoroutine(NextWormCRT());
    }

    private IEnumerator NextWormCRT()
    {
    int nextWorm = _currWorm;

    do
    {
        nextWorm++;
        if (nextWorm >= _worms.Length)
            nextWorm = 0;

    } while (!_worms[nextWorm].IsAlive);

    _currWorm = -1;

    yield return new WaitForSeconds(_nextWormTurnTime);

    _currWorm = nextWorm;

    _wormCamera.SetParent(_worms[_currWorm].transform);
    _wormCamera.localPosition = Vector3.back * 10;
    }
}
