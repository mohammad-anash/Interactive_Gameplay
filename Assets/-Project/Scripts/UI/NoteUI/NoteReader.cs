using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// One shared note reader. The fade and zoom are done by UIPanel.
// Put this on an object that is ALWAYS active (for example the Canvas),
// not on the panel itself.
public class NoteReader : MonoBehaviour
{
  [Header("References")]
  [Tooltip("The UIPanel on NoteReadingUI.")]
  [SerializeField] private UIPanel _panel;
  [SerializeField] private TMP_Text _body;
  [SerializeField] private PlayerAndCameraController _playerController;

  // The frame in which the note was opened (see Update)
  private int _openedFrame = -1;

  private bool HasValidSetup =>
      _panel != null &&
      _body != null &&
      _playerController != null;

  // Notes ask this before they let the player read them
  public bool IsClosed => _panel == null || !_panel.IsVisible;

  private void Awake()
  {
    if (!HasValidSetup)
    {
      Debug.LogError("NoteReader: Assign the panel, body text, and player controller.", this);
      return;
    }

    _panel.HideImmediate();
  }

  public void Show(string text)
  {
    if (!HasValidSetup || !IsClosed)
      return;

    _body.text = text;
    _panel.Show();
    _playerController.DisablePlayerAndCameraMovement();

    _openedFrame = Time.frameCount;
  }

  private void Update()
  {
    if (!HasValidSetup || !_panel.IsVisible)
      return;

    // The E press that opened the note is still "pressed this frame".
    // Without this line, the note would close the moment it opens.
    if (Time.frameCount == _openedFrame)
      return;

    if (WasCloseKeyPressed())
      Close();
  }

  private bool WasCloseKeyPressed()
  {
    return Keyboard.current != null &&
           (Keyboard.current.eKey.wasPressedThisFrame ||
            Keyboard.current.escapeKey.wasPressedThisFrame);
  }

  private void Close()
  {
    _panel.Hide();
    _playerController.EnablePlayerAndCameraMovement();
  }

  private void OnDisable()
  {
    // Never leave the player frozen if this script turns off while reading
    if (HasValidSetup && _panel.IsVisible)
      _playerController.EnablePlayerAndCameraMovement();
  }
};