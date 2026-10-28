using System;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _crosshair;
    [SerializeField] private Image _crosshairImage;
    [SerializeField] private InteractionManager _interactionManager;

    [Header("Size Settings")]
    [SerializeField, Min(1f)] private float _normalSize = 7f;
    [SerializeField, Min(1f)] private float _targetSize = 11f;
    [SerializeField, Min(0.01f)] private float _transitionSpeed = 10f;

    [Header("Alpha Settings")]
    [SerializeField, Range(0f, 1f)] private float _normalAlpha = 0.65f;
    [SerializeField, Range(0f, 1f)] private float _targetAlpha = 1f;

    private bool _canInteract;

    private void Awake()
    {
        if (_crosshair == null)
            _crosshair = GetComponent<RectTransform>();

        if (_crosshairImage == null)
            _crosshairImage = GetComponent<Image>();

        if (_crosshair == null || _crosshairImage == null)
        {
            Debug.LogError(
                "CrosshairController: Crosshair RectTransform or Image is missing.",
                this
            );

            enabled = false;
            return;
        }

        SetCrosshairAppearance(_normalSize, _normalAlpha);
    }

    private void OnEnable()
    {
        if (_interactionManager != null)
            _interactionManager.PromptChanged += OnPromptChanged;
    }

    private void OnDisable()
    {
        if (_interactionManager != null)
            _interactionManager.PromptChanged -= OnPromptChanged;
    }

    private void Update()
    {
        bool cursorLocked = Cursor.lockState == CursorLockMode.Locked;
        _crosshairImage.enabled = cursorLocked;

        if (!cursorLocked)
            return;

        float targetSize = _canInteract ? _targetSize : _normalSize;
        float targetAlpha = _canInteract ? _targetAlpha : _normalAlpha;

        _crosshair.sizeDelta = Vector2.Lerp(
            _crosshair.sizeDelta,
            new Vector2(targetSize, targetSize),
            Time.deltaTime * _transitionSpeed
        );

        Color color = _crosshairImage.color;

        color.a = Mathf.MoveTowards(
            color.a,
            targetAlpha,
            Time.deltaTime * _transitionSpeed
        );

        _crosshairImage.color = color;
    }

    private void OnPromptChanged(string prompt)
    {
        _canInteract = !string.IsNullOrEmpty(prompt);
    }

    private void SetCrosshairAppearance(float size, float alpha)
    {
        _crosshair.sizeDelta = new Vector2(size, size);

        Color color = _crosshairImage.color;
        color.a = alpha;
        _crosshairImage.color = color;
    }
}
