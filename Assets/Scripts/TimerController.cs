using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public static TimerController Instance;
    [SerializeField] private float _defaultTime;


    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
    }

    void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        _defaultTime -= Time.deltaTime;
        if (_defaultTime <= 0)
        {
            ResetTime();
            WormManager.Instance.NextWorm();
        }
    }

    public void ResetTime()
    {
        _defaultTime = 60;
    }
}
