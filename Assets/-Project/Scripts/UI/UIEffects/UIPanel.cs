using System.Collections;
using UnityEngine;

// One shared fade and zoom for every panel (note, keypad, ending).
// It only shows and hides. It knows nothing about notes or safes.
[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField, Min(0f)] private float _animationDuration = 0.3f;
    [SerializeField, Range(0.5f, 1f)] private float _startScale = 0.9f;

    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Vector3 _shownScale = Vector3.one;
    private bool _isCached;

    private Coroutine _animationCoroutine;
    private bool _isVisible;

    public bool IsVisible => _isVisible;

    private void Awake()
    {
        CacheReferences();
    }

    // Safe to call many times. Needed because another script may call
    // Show() or HideImmediate() before this Awake has run.
    private void CacheReferences()
    {
        if (_isCached)
            return;

        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _shownScale = _rectTransform.localScale;
        _isCached = true;
    }

    public void Show()
    {
        CacheReferences();

        // Start from "hidden", so the fade and zoom are visible
        if (!gameObject.activeSelf)
        {
            _canvasGroup.alpha = 0f;
            _rectTransform.localScale = _shownScale * _startScale;
            gameObject.SetActive(true);
        }

        _isVisible = true;
        StartAnimation(1f);
    }

    public void Hide()
    {
        CacheReferences();
        _isVisible = false;

        if (!gameObject.activeSelf)
            return;

        StartAnimation(0f);
    }

    // Hides at once, with no animation. Use it at the start of the game.
    public void HideImmediate()
    {
        CacheReferences();

        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = null;
        _isVisible = false;
        _canvasGroup.alpha = 0f;
        _rectTransform.localScale = _shownScale * _startScale;
        gameObject.SetActive(false);
    }

    private void StartAnimation(float targetAlpha)
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        // A coroutine can't start if this object or a parent is inactive.
        // Jump straight to the final state, so it never stays half-way.
        if (!isActiveAndEnabled)
        {
            Debug.LogWarning(
                "UIPanel: The panel or one of its parents is inactive. Skipping the animation.",
                this);

            ApplyFinalState(targetAlpha);
            return;
        }

        _animationCoroutine = StartCoroutine(AnimatePanel(targetAlpha));
    }

    private IEnumerator AnimatePanel(float targetAlpha)
    {
        float startAlpha = _canvasGroup.alpha;
        Vector3 startScale = _rectTransform.localScale;

        Vector3 targetScale = targetAlpha > 0f
            ? _shownScale
            : _shownScale * _startScale;

        float elapsed = 0f;

        while (elapsed < _animationDuration)
        {
            // Unscaled time, so it also works while the game is paused
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / _animationDuration);
            float eased = Mathf.SmoothStep(0f, 1f, progress);

            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, eased);
            _rectTransform.localScale = Vector3.Lerp(startScale, targetScale, eased);

            yield return null;
        }

        _animationCoroutine = null;
        ApplyFinalState(targetAlpha);
    }

    private void ApplyFinalState(float targetAlpha)
    {
        _canvasGroup.alpha = targetAlpha;

        _rectTransform.localScale = targetAlpha > 0f
            ? _shownScale
            : _shownScale * _startScale;

        if (targetAlpha <= 0f)
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // Unity already stopped the coroutine. Clear our reference to it.
        _animationCoroutine = null;
    }
}