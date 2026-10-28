using UnityEngine;

public class Drawer : MonoBehaviour, IInteractable
{
    private enum DrawerState
    {
        Closed,
        Opening,
        Open,
        Closing
    }

    [Header("Drawer Settings")]
    [Tooltip("Direction to slide when opening, relative to the parent.")]
    [SerializeField] private Vector3 _slideDirection = new Vector3(1f, 0f, 0f);
    [SerializeField, Min(0f)] private float _slideDistance = 0.45f;
    [SerializeField, Min(0.1f)] private float _slideSpeed = 0.6f;

    [Header("Safety")]
    [Tooltip("The drawer waits while the player is inside this zone (only when opening).")]
    [SerializeField] private PlayerZone _playerZone;

    private DrawerState _currentState = DrawerState.Closed;

    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private Vector3 _targetPosition;

    private bool HasValidSetup =>
        _slideDirection.sqrMagnitude > 0.0001f &&
        _slideDistance > 0f;

    // Empty text means "can't use right now"
    public string InteractText =>
        !HasValidSetup
            ? string.Empty
            : _currentState switch
            {
                DrawerState.Closed => "Open the drawer",
                DrawerState.Open => "Close the drawer",
                _ => string.Empty
            };

    private void Awake()
    {
        _closedPosition = transform.localPosition;
        _openPosition = _closedPosition + _slideDirection.normalized * _slideDistance;
        _targetPosition = _closedPosition;

        if (!HasValidSetup)
        {
            Debug.LogError(
                "Drawer: Invalid setup. Check the slide direction and distance.", this);
        }
    }

    private void OnValidate()
    {
        if (_slideDirection == Vector3.zero)
            Debug.LogWarning("Drawer: Slide direction cannot be zero.", this);
    }

    public void Interact()
    {
        if (!HasValidSetup)
            return;

        switch (_currentState)
        {
            case DrawerState.Closed:
                _targetPosition = _openPosition;
                _currentState = DrawerState.Opening;
                break;

            case DrawerState.Open:
                _targetPosition = _closedPosition;
                _currentState = DrawerState.Closing;
                break;
        }
    }

    private void Update()
    {
        // Only Opening and Closing have work to do.
        // Without this line, an Open drawer is turned into Closed by mistake.
        if (_currentState != DrawerState.Opening &&
            _currentState != DrawerState.Closing)
        {
            return;
        }

        // Opening moves toward the player, so wait if the player is in the way.
        // The state stays Opening, so it continues when the player steps away.
        if (_currentState == DrawerState.Opening &&
            _playerZone != null &&
            _playerZone.IsPlayerInside)
        {
            return;
        }

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            _targetPosition,
            _slideSpeed * Time.deltaTime);

        if (transform.localPosition != _targetPosition)
            return;

        _currentState = _currentState == DrawerState.Opening
            ? DrawerState.Open
            : DrawerState.Closed;
    }
}