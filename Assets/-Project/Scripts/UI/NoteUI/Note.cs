using UnityEngine;

// Holds only the text. The NoteReader does the showing.
public class Note : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private NoteReader _reader;

    [Tooltip("Optional. Needed only if the text uses {first} or {second}.")]
    [SerializeField] private SafeCodeData _codeData;

    [Header("Note Settings")]
    [SerializeField] private string _prompt = "Read Note";

    [Tooltip("Use {first} for the first half of the safe code, and {second} for the other half.")]
    [TextArea(5, 12)]
    [SerializeField]
    private string _content =
        "The safe combination starts with {first}.\n\nThe rest burns when the candle is lit.";

    // Empty text means "can't use right now"
    public string InteractText =>
        _reader != null && _reader.IsClosed
            ? _prompt
            : string.Empty;

    private void Awake()
    {
        if (_reader == null)
            Debug.LogError("Note: NoteReader is not assigned.", this);

        bool usesCode = _content.Contains("{first}") || _content.Contains("{second}");

        if (usesCode && _codeData == null)
            Debug.LogError("Note: The text uses the safe code, but Code Data is not assigned.", this);
    }

    public void Interact()
    {
        if (_reader == null || !_reader.IsClosed)
            return;

        _reader.Show(BuildText());
    }

    // Replaces the markers with the real code, so the code lives in one place
    private string BuildText()
    {
        if (_codeData == null)
            return _content;

        return _content
            .Replace("{first}", _codeData.FirstHalf)
            .Replace("{second}", _codeData.SecondHalf);
    }
}