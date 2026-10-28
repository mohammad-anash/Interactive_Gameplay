using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerBody;

    [Header("Look Settings")]
    [SerializeField] private float _mouseSensitivity = 0.15f;
    [SerializeField] private float _smoothTime = 0.05f;
    [SerializeField] private float _maxLookAngle = 80f;

    private float _xRotation;
    private float _currentMouseX;
    private float _currentMouseY;
    private float _mouseXVelocity;
    private float _mouseYVelocity;

    private void Awake()
    {
        Debug.Assert(_playerBody != null,
            "CameraController: Player Body is not assigned.", this);
    }

    private void Start()
    {
        SetCursorLocked(true);
    }

    private void Update()
    {
        // TEMPORARY: Tab switches between look mode and UI mode.
        // Keep it ABOVE the early return, or you could never lock again.
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            SetCursorLocked(Cursor.lockState != CursorLockMode.Locked);

        // In UI mode (cursor free) the camera does not rotate
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        if (Mouse.current == null || _playerBody == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Mouse delta is already "movement since last frame",
        // so we do not multiply by Time.deltaTime.
        float targetX = mouseDelta.x * _mouseSensitivity;
        float targetY = mouseDelta.y * _mouseSensitivity;

        _currentMouseX = Mathf.SmoothDamp(
            _currentMouseX, targetX, ref _mouseXVelocity, _smoothTime);

        _currentMouseY = Mathf.SmoothDamp(
            _currentMouseY, targetY, ref _mouseYVelocity, _smoothTime);

        _xRotation -= _currentMouseY;
        _xRotation = Mathf.Clamp(_xRotation, -_maxLookAngle, _maxLookAngle);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _playerBody.Rotate(Vector3.up * _currentMouseX);
    }

    private void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}