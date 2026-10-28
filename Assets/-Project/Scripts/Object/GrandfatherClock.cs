using UnityEngine;

public class GrandfatherClock : MonoBehaviour, IInteractable
{
    private enum ClockState
    {
        Stopped,
        Running
    }

    [Header("Clock Settings")]
    [SerializeField, Min(0.1f)] private float _duration = 30f;
    [SerializeField, Min(0f)] private float _swingSpeed = 5f;
    [SerializeField, Range(0f, 45f)] private float _swingAngle = 25f;
    [SerializeField, Min(0f)] private float _handRotationSpeed = 10f;

    [Header("References")]
    [SerializeField] private Transform _pendulumPivot;
    [SerializeField] private Transform _handPivot;

    private ClockState _currentState = ClockState.Stopped;
    private float _timer;
    private float _startPendulumAngle;
    private float _startHandAngle;

    private bool HasReferences =>
        _pendulumPivot != null && _handPivot != null;

    // Empty text means "can't use right now"
    public string InteractText =>
        HasReferences && _currentState == ClockState.Stopped
            ? "Wind the clock"
            : string.Empty;

    private void Awake()
    {
        if (!HasReferences)
        {
            Debug.LogError(
                "GrandfatherClock: Assign both pivot references.", this);
            return;
        }

        // Remember the resting angles so the clock can return to them
        _startPendulumAngle = _pendulumPivot.localEulerAngles.z;
        _startHandAngle = _handPivot.localEulerAngles.z;
    }

    public void Interact()
    {
        if (!HasReferences || _currentState != ClockState.Stopped)
            return;

        _timer = 0f;
        _currentState = ClockState.Running;
    }

    private void Update()
    {
        if (_currentState != ClockState.Running)
            return;

        _timer += Time.deltaTime;

        // Sin(0) = 0, so the pendulum starts from its resting position
        float swing = Mathf.Sin(_timer * _swingSpeed) * _swingAngle;
        SetPendulumAngle(_startPendulumAngle + swing);

        SetHandAngle(_startHandAngle - _timer * _handRotationSpeed);

        if (_timer >= _duration)
            StopClock();
    }

    private void StopClock()
    {
        _currentState = ClockState.Stopped;
        _timer = 0f;

        // Return both parts to their resting angles
        SetPendulumAngle(_startPendulumAngle);
        SetHandAngle(_startHandAngle);
    }

    private void SetPendulumAngle(float angle)
    {
        Vector3 rotation = _pendulumPivot.localEulerAngles;
        rotation.z = angle;
        _pendulumPivot.localEulerAngles = rotation;
    }

    private void SetHandAngle(float angle)
    {
        Vector3 rotation = _handPivot.localEulerAngles;
        rotation.z = angle;
        _handPivot.localEulerAngles = rotation;
    }
}