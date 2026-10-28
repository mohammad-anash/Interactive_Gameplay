using TMPro;
using UnityEngine;

// Puts one part of the safe code into a text object (for example the wall writing).
// It only sets the text. The fade is done by FadingText.
public class SafeCodePartText : MonoBehaviour
{
  private enum CodePart
  {
    FirstHalf,
    SecondHalf
  }

  [SerializeField] private SafeCodeData _codeData;
  [SerializeField] private CodePart _part = CodePart.SecondHalf;
  [SerializeField] private TMP_Text _text;

  private void Awake()
  {
    if (_text == null)
      _text = GetComponent<TMP_Text>();

    if (_text == null || _codeData == null)
    {
      Debug.LogError("SafeCodePartText: Assign the Code Data and a text.", this);
      return;
    }

    _text.text = _part == CodePart.FirstHalf
        ? _codeData.FirstHalf
        : _codeData.SecondHalf;
  }
}