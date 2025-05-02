using UnityEngine;
using System;

public class WaitHandler : MonoBehaviour
{
    private Action _waitCallback;
    private float _secondsToWait = -1;
    private float _secondsPassed = -1;
    private bool _isWaiting;

    public bool IsWaiting { get => _isWaiting; }

    private void Awake()
    {
        reset();
    }

    public void wait(float secondsToWait, Action waitCallback)
    {
        _waitCallback = waitCallback;
        _secondsPassed = 0;
        _secondsToWait = secondsToWait;
        _isWaiting = true;
        enabled = true;
    }

    public void reset()
    {
        _secondsToWait = -1;
        _secondsPassed = -1;
        _isWaiting = false;
        enabled = false;
    }

    void Update()
    {
        if (_secondsToWait <= 0) return; // Agent is not waiting, no updating needed.

        _secondsPassed += Time.deltaTime;

        if (_secondsPassed >= _secondsToWait)
        {
            _secondsToWait = -1;
            _secondsPassed = -1;
            reset();
            _waitCallback();
        }
    }
}
