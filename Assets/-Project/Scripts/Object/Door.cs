using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]

    [Tooltip("Negative opens one way, positive the other.")]
    [SerializeField] private float _openAngle = -90f;
    [SerializeField, Min(1f)] private float _openSpeed = 90f;

    [Header("Safety")]
    [SerializeField] private PlayerZone _playerZone;

    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private bool _isOpening;
    private bool _isOpen;

    public bool IsOpen => _isOpen;

    private void Awake()
    {
        _closedRotation = transform.localRotation;
        _openRotation = _closedRotation * Quaternion.Euler(0f, _openAngle, 0f);
    }

    private void Update()
    {
        if (!_isOpening) return;

        if (_playerZone != null && _playerZone.IsPlayerInside)
            return;

        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation,
            _openRotation,
            _openSpeed * Time.deltaTime
        );

        if (Quaternion.Angle(transform.localRotation, _openRotation) < 0.1f)
        {
            transform.localRotation = _openRotation;
            _isOpening = false;
            _isOpen = true;
        }
    }

    public void Open()
    {
        if (_isOpen || _isOpening) return;

        _isOpening = true;
    }
}
