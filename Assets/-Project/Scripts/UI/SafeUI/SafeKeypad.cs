using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;


public class SafeKeypad : MonoBehaviour
{
    [Header("Buttons References")]
    [SerializeField] private Button[] _buttons;


    [Tooltip("Clear Button Reference")]
    [SerializeField] private Button _clearButton;

    [Tooltip("Enter Button Reference")]
    [SerializeField] private Button _enterButton;


    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI _codeEnter;
    [SerializeField] private TextMeshProUGUI _safeText;


    [Header("Safe Code Data")]
    [SerializeField] private SafeCodeData _safeCodeData;


    public event Action<string> CodeSubmitted;

    // --- Current Text Digit -------------------
    private string _enteredCode = string.Empty;


    // --- Add Click Event To all Button And Check Required References ------------------
    private void Awake()
    {
        if (Validation())
        {
            Debug.LogError("SafeKeypad: Required references are missing or invalid!", this);
            enabled = false;
            return;
        }

        AddEventOnEveryButton();
        ResetKeypad();
    }


    // --- Add Event On Every Button And Get The Code -----------

    private void AddEventOnEveryButton()
    {
        foreach (Button button in _buttons)
        {
            Button currentButton = button;
            TMP_Text buttonText = currentButton.GetComponentInChildren<TMP_Text>();

            currentButton.onClick.AddListener(
                () => GetCode(buttonText.text));
        }

        _clearButton.onClick.AddListener(ClearCode);
        _enterButton.onClick.AddListener(CheckEnteredCode);
    }


    // --- Get Code From All Button from 0-9 Except Clear and Enter --------------------
    private void GetCode(string text)
    {
        AddDigit(text);
    }

    // ----- Add Digit To The Code  -----------------

    public void AddDigit(string code)
    {
        if (_enteredCode.Length >= 4) return;
        if (code.Length != 1 || !char.IsDigit(code[0])) return;

        _enteredCode += code;
        _codeEnter.text = string.Join(" ", _enteredCode.PadRight(4, '_').ToCharArray());

        _safeText.text = string.Empty;
    }


    // ----- Clear The Enter Code -----------------
    public void ClearCode()
    {
        ResetKeypad();
    }


    // ----- Reset KeyPad -----------------
    public void ResetKeypad()
    {
        _enteredCode = string.Empty;
        _codeEnter.text = "_ _ _ _";
        _safeText.text = string.Empty;
    }


    public void ShowWrongCode()
    {
        _enteredCode = string.Empty;
        _codeEnter.text = "_ _ _ _";
        _safeText.text = "WRONG CODE!";
    }



    // --- Check Entered Code ----------------

    private void CheckEnteredCode()
    {
        if (_enteredCode.Length == 0)
        {
            _safeText.text = "ENTER CODE FIRST!";
        }
        else if (_enteredCode.Length < _safeCodeData.Code.Length)
        {
            _safeText.text = $"ENTER {_safeCodeData.Code.Length}-DIGIT CODE!";
        }
        else
        {
            CodeSubmitted?.Invoke(_enteredCode);
        }
    }



    // ---- Check If Any Requried Reference is Missing Warn It ------------
    private bool Validation()
    {
        return _buttons == null ||
            _buttons.Length == 0 ||
            System.Array.Exists(_buttons, button =>
                button == null ||
                button.GetComponentInChildren<TMP_Text>() == null) ||
            _safeText == null ||
            _clearButton == null ||
            _enterButton == null ||
            _codeEnter == null ||
            _safeCodeData == null;
    }

}
