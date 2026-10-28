using UnityEngine;
using System;

public class GameTimer : MonoBehaviour
{
    private float _elapsedTime;
    private bool _isRunning = true;

    // Elapsed time in seconds. Other scripts can read it.
    public float ElapsedTime => _elapsedTime;

    public void Stop()
    {
        _isRunning = false;
    }

    private void Update()
    {
        if (!_isRunning) return;

        _elapsedTime += Time.deltaTime;
    }

    public string GetFormattedTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(_elapsedTime);

        return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";
    }
}
