using UnityEngine;

public class Bookshelf : MonoBehaviour, IInteractable
{
    private enum BookshelfState
    {
        Closed,
        Moving,
        Open
    }

    [Header("Bookshelf Settings")]
    [Tooltip("Direction to slide, relative to the parent object.")]
    [SerializeField] private Vector3 _direction = new Vector3(0f, 0f, 1f);
    [SerializeField, Min(0f)] private float _distance = 2.5f;
    [SerializeField, Min(0.1f)] private float _speed = 1.7f;

    [Header("Safety")]
    [Tooltip("The shelf waits while the player is inside this zone.")]
    [SerializeField] private PlayerZone _playerZone;

    private BookshelfState _currentState = BookshelfState.Closed;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;

    private bool HasValidSetup =>
        _direction != Vector3.zero && _distance > 0f;

    // Empty text means "can't use right now"
    public string InteractText =>
        HasValidSetup && _currentState == BookshelfState.Closed
            ? "pull the book"
            : string.Empty;

    private void Awake()
    {
        // Local position, so the shelf still works if the house is moved
        _startPosition = transform.localPosition;
        _targetPosition = _startPosition + _direction.normalized * _distance;
    }

    private void OnValidate()
    {
        if (_direction == Vector3.zero)
            Debug.LogWarning("Bookshelf: Direction cannot be zero.", this);

        if (_distance <= 0f)
            Debug.LogWarning("Bookshelf: Distance must be greater than zero.", this);
    }

    public void Interact()
    {
        if (!HasValidSetup || _currentState != BookshelfState.Closed)
            return;

        _currentState = BookshelfState.Moving;
    }

    private void Update()
    {
        if (_currentState != BookshelfState.Moving)
            return;

        // The state stays Moving, so it continues when the player steps away
        if (_playerZone != null && _playerZone.IsPlayerInside)
            return;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            _targetPosition,
            _speed * Time.deltaTime);

        if (transform.localPosition == _targetPosition)
            _currentState = BookshelfState.Open;
    }
}