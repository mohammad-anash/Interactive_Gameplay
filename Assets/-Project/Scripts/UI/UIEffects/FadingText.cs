using TMPro;
using UnityEngine;

public class FadingText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _fadeSpeed = 2f;

    private float _targetAlpha;

    private void Awake()
    {
        if (_text == null)
            _text = GetComponent<TMP_Text>();

        _targetAlpha = 0f;
        _text.alpha = 0f;
    }

    private void Update()
    {
        _text.alpha = Mathf.MoveTowards(
            _text.alpha,
            _targetAlpha,
            _fadeSpeed * Time.deltaTime
        );
    }

    public void FadeIn()
    {
        _targetAlpha = 1f;
    }

    public void FadeOut()
    {
        _targetAlpha = 0f;
    }
}
