using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour, IInteractable
{
    private enum LeverState
    {
        Up,
        PullingDown,
        Down,
        PullingUp
    }

    private LeverState _currentState = LeverState.Up;

    [Header("Lever Settings")]
    [SerializeField] private Transform _handlePivot;
    [SerializeField, Range(1f, 180f)] private float _pullingAngle = 80f;
    [SerializeField, Min(0.01f)] private float _pullSpeed = 120f;

    [Header("Lever Events")]
    [SerializeField] private UnityEvent _onPulled;
    [SerializeField] private UnityEvent _onReleased;

    private Quaternion _originalRotation;
    private Quaternion _pulledRotation;

    private bool HasValidSetup =>
        _handlePivot != null &&
        _pullingAngle > 0f &&
        _pullingAngle <= 180f &&
        _pullSpeed > 0f;


    public string InteractText =>
        !HasValidSetup
            ? string.Empty
            : _currentState switch
            {
                LeverState.Up => "Pull the lever",
                LeverState.Down => "Push the lever back",
                _ => string.Empty
            };

    private void Awake()
    {
        if (_handlePivot != null)
        {
            _originalRotation = _handlePivot.localRotation;

            _pulledRotation = _originalRotation *
                              Quaternion.Euler(-_pullingAngle, 0f, 0f);
        }

        if (!HasValidSetup)
        {
            Debug.LogError(
                "Lever setup is invalid. Check Handle Pivot, Pulling Angle, and Pull Speed.",
                this
            );
        }
    }

    public void Interact()
    {
        if (!HasValidSetup)
            return;

        if (_currentState == LeverState.Up)
        {
            _currentState = LeverState.PullingDown;
            _onPulled?.Invoke();
        }
        else if (_currentState == LeverState.Down)
        {
            _currentState = LeverState.PullingUp;
            _onReleased?.Invoke();
        }
    }

    private void Update()
    {
        if (!HasValidSetup)
            return;

        if (_currentState == LeverState.PullingDown)
        {
            RotateHandleToTarget(_pulledRotation);

            if (Quaternion.Angle(
                    _handlePivot.localRotation,
                    _pulledRotation) < 0.1f)
            {
                SetRotationAndState(
                    _pulledRotation,
                    LeverState.Down
                );
            }
        }
        else if (_currentState == LeverState.PullingUp)
        {
            RotateHandleToTarget(_originalRotation);

            if (Quaternion.Angle(
                    _handlePivot.localRotation,
                    _originalRotation) < 0.1f)
            {
                SetRotationAndState(
                    _originalRotation,
                    LeverState.Up
                );
            }
        }
    }

    private void RotateHandleToTarget(Quaternion targetRotation)
    {
        _handlePivot.localRotation = Quaternion.RotateTowards(
            _handlePivot.localRotation,
            targetRotation,
            _pullSpeed * Time.deltaTime
        );
    }

    private void SetRotationAndState(
        Quaternion rotation,
        LeverState state)
    {
        _handlePivot.localRotation = rotation;
        _currentState = state;
    }
}
