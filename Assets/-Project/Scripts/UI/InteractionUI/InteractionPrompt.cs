using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class InteractionPrompt : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private InteractionManager _interactionManager;
  [SerializeField] private TextMeshProUGUI _promptText;

  [Header("Fade Settings")]
  [SerializeField, Min(0.1f)] private float _fadeSpeed = 5f;

  private CanvasGroup _canvasGroup;
  private float _targetAlpha;

  private void Awake()
  {
    _canvasGroup = GetComponent<CanvasGroup>();

    if (_interactionManager == null || _promptText == null)
    {
      Debug.LogError(
          "InteractionPrompt: Assign all required references.",
          this
      );

      enabled = false;
      return;
    }

    _canvasGroup.alpha = 0f;
    _canvasGroup.interactable = false;
    _canvasGroup.blocksRaycasts = false;

    _promptText.text = string.Empty;
    _targetAlpha = 0f;
  }

  private void OnEnable()
  {
    if (_interactionManager == null)
      return;

    _interactionManager.PromptChanged += HandlePromptChanged;
  }

  private void OnDisable()
  {
    if (_interactionManager == null)
      return;

    _interactionManager.PromptChanged -= HandlePromptChanged;
  }

  private void Update()
  {
    _canvasGroup.alpha = Mathf.MoveTowards(
        _canvasGroup.alpha,
        _targetAlpha,
        _fadeSpeed * Time.deltaTime
    );

    // Clear the text only after the fade-out finishes.
    if (_targetAlpha == 0f && _canvasGroup.alpha == 0f)
    {
      _promptText.text = string.Empty;
    }
  }

  private void HandlePromptChanged(string message)
  {
    if (string.IsNullOrEmpty(message))
    {
      Hide();
      return;
    }

    Show(message);
  }

  private void Show(string message)
  {
    _promptText.text = $"<color=#FFD27F>[E]</color> {message}";
    _targetAlpha = 1f;
  }

  private void Hide()
  {
    _targetAlpha = 0f;
  }
}
