using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CandleController : MonoBehaviour, IInteractable
{
    [Header("Candle Objects")]
    [SerializeField] private GameObject _flame;
    [SerializeField] private Light _flameLight;

    [Header("Flicker")]
    [SerializeField, Min(0f)] private float _flickerSpeed = 3f;
    [SerializeField, Range(0f, 1f)] private float _minBrightness = 0.7f;
    [SerializeField, Range(1f, 2f)] private float _maxBrightness = 1.1f;

    [Header("Events")]
    // FormerlySerializedAs keeps your old Inspector connections
    [SerializeField, FormerlySerializedAs("OnLit")]
    private UnityEvent _onLit;

    [SerializeField, FormerlySerializedAs("OnExtinguished")]
    private UnityEvent _onExtinguished;

    private bool _isLit;
    private float _baseIntensity;

    private bool HasValidSetup => _flame != null && _flameLight != null;

    // Empty text means "can't use right now"
    public string InteractText =>
        !HasValidSetup
            ? string.Empty
            : _isLit ? "Blow out the candle" : "Light the candle";

    private void Awake()
    {
        if (_flame == null)
            Debug.LogError("CandleController: Flame is not assigned.", this);

        if (_flameLight == null)
            Debug.LogError("CandleController: Flame Light is not assigned.", this);

        if (_flameLight != null)
            _baseIntensity = _flameLight.intensity;

        SetFlameVisible(false);
    }

    private void Update()
    {
        if (!_isLit || !HasValidSetup)
            return;

        // Smooth random value between 0 and 1
        float noise = Mathf.PerlinNoise(Time.time * _flickerSpeed, 0f);

        _flameLight.intensity = _baseIntensity *
            Mathf.Lerp(_minBrightness, _maxBrightness, noise);
    }

    public void Interact()
    {
        if (_isLit)
            Extinguish();
        else
            Light();
    }

    // Public so the Inspector (or another object) can call it
    public void Light()
    {
        if (!HasValidSetup || _isLit)
            return;

        _isLit = true;
        SetFlameVisible(true);
        _onLit?.Invoke();
    }

    // Public so the Inspector (or another object) can call it
    public void Extinguish()
    {
        if (!HasValidSetup || !_isLit)
            return;

        _isLit = false;
        SetFlameVisible(false);
        _onExtinguished?.Invoke();
    }

    private void SetFlameVisible(bool visible)
    {
        if (_flame != null)
            _flame.SetActive(visible);

        if (_flameLight != null)
            _flameLight.enabled = visible;
    }
}