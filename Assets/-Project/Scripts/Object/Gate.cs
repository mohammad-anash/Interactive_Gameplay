using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Gate Settings")]
    [Tooltip("Direction to slide when opening, relative to the parent.")]
    [SerializeField] private Vector3 _slideDirection = new Vector3(0f, 1f, 0f);
    [SerializeField, Min(0f)] private float _slideDistance = 3f;
    [SerializeField, Min(0.1f)] private float _slideSpeed = 1.5f;

    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private Vector3 _targetPosition;

    private bool HasValidSetup =>
        _slideDirection != Vector3.zero &&
        _slideDistance > 0f &&
        _slideSpeed > 0f;

    private void Awake()
    {
        // Store positions first, so the gate never jumps to (0, 0, 0)
        _closedPosition = transform.localPosition;
        _targetPosition = _closedPosition;
        _openPosition = _closedPosition + _slideDirection.normalized * _slideDistance;

        if (!HasValidSetup)
            Debug.LogError("Gate: Invalid setup. Check the slide settings.", this);
    }

    private void OnValidate()
    {
        if (_slideDirection == Vector3.zero)
            Debug.LogWarning("Gate: Slide direction cannot be zero.", this);

        if (_slideDistance <= 0f)
            Debug.LogWarning("Gate: Slide distance must be greater than zero.", this);
    }

    private void Update()
    {
        if (!HasValidSetup)
            return;

        // Nothing to do when the gate is already at its target
        if (transform.localPosition == _targetPosition)
            return;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            _targetPosition,
            _slideSpeed * Time.deltaTime);
    }

    // Public so a UnityEvent in the Inspector can call it
    public void Open()
    {
        if (!HasValidSetup)
            return;

        _targetPosition = _openPosition;
    }

    // Public so a UnityEvent in the Inspector can call it
    public void Close()
    {
        if (!HasValidSetup)
            return;

        _targetPosition = _closedPosition;
    }
}