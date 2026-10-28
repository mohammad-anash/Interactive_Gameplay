using UnityEngine;
using UnityEngine.InputSystem;

public class Safe : MonoBehaviour, IInteractable
{
    private enum SafeState
    {
        Locked,
        KeypadOpen,
        Opening,
        Open
    }

    [Header("References")]
    [SerializeField] private SafeCodeData _safeCodeData;
    [SerializeField] private PlayerAndCameraController _controller;
    [SerializeField] private UIPanel _safeCodeUI;
    [SerializeField] private SafeKeypad _safeKeypad;
    [SerializeField] private Door _door;

    private SafeState _currentState = SafeState.Locked;

    public string InteractText =>
        _currentState == SafeState.Locked ? "Use Safe" : string.Empty;

    private void Awake()
    {
        if (HasMissingReference())
        {
            Debug.LogError("Safe: Required references are missing!", this);
            return;
        }

        _safeCodeUI.HideImmediate();

        _safeKeypad.CodeSubmitted += CheckCode;
    }

    private void Update()
    {
        // Change state when the safe door finishes opening
        if (_currentState == SafeState.Opening && _door.IsOpen)
        {
            _currentState = SafeState.Open;
            return;
        }

        // Close the keypad when Escape is pressed
        if (_currentState != SafeState.KeypadOpen) return;

        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseKeypad();
        }
    }


    public void Interact()
    {
        if (!enabled) return;
        if (_currentState != SafeState.Locked) return;

        _currentState = SafeState.KeypadOpen;

        _safeKeypad.ResetKeypad();

        _controller.DisablePlayerAndCameraMovement();
        _safeCodeUI.Show();
    }

    public void CloseKeypad()
    {
        if (_currentState != SafeState.KeypadOpen) return;

        _safeCodeUI.Hide();
        _controller.EnablePlayerAndCameraMovement();

        _currentState = SafeState.Locked;
    }

    private void OpenSafe()
    {
        if (_currentState != SafeState.KeypadOpen) return;

        _safeCodeUI.Hide();
        _controller.EnablePlayerAndCameraMovement();

        _currentState = SafeState.Opening;
        _door.Open();
    }

    private void CheckCode(string enteredCode)
    {
        if (_currentState != SafeState.KeypadOpen)
            return;

        if (enteredCode == _safeCodeData.Code)
        {
            OpenSafe();
        }
        else
        {
            _safeKeypad.ShowWrongCode();
        }
    }

    private bool HasMissingReference()
    {
        return _controller == null ||
               _safeCodeUI == null ||
               _safeKeypad == null ||
               _door == null;
    }


    private void OnDestroy()
    {
        if (_safeKeypad != null)
        {
            _safeKeypad.CodeSubmitted -= CheckCode;
        }
    }
}
