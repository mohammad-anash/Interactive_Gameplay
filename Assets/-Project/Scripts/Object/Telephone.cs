using UnityEngine;

public class Telephone : MonoBehaviour, IInteractable
{
    private enum TelephoneStates
    {
        Idle,
        Ringing,
        Talking
    }

    private TelephoneStates _currentState = TelephoneStates.Idle;

    [Header("Timing Settings")]
    [SerializeField, Min(0f)] private float _minWait = 20f;
    [SerializeField, Min(0f)] private float _maxWait = 40f;
    [SerializeField, Min(0f)] private float _ringDuration = 15f;

    [Header("Receiver Settings")]
    [SerializeField] private Transform _receiver;
    [SerializeField, Min(0f)] private float _shakeAngle = 8f;
    [SerializeField, Min(0f)] private float _shakeSpeed = 40f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _answerClip;
    [SerializeField] private AudioClip _ringClip;

    [Header("Light Settings")]
    [SerializeField] private Light _light;

    private float _timer;
    private float _ringTimer;
    private float _randomWaitDuration;

    private Quaternion _originalReceiverRotation;

    public string InteractText =>
        _currentState == TelephoneStates.Ringing
            ? "Answer the phone"
            : string.Empty;

    private void Awake()
    {
        if (_receiver != null)
            _originalReceiverRotation = _receiver.localRotation;

        if (_light != null)
            _light.enabled = false;

        if (_minWait > _maxWait)
        {
            Debug.LogWarning(
                "Min Wait was greater than Max Wait. Swapping values.",
                this
            );

            float temp = _minWait;
            _minWait = _maxWait;
            _maxWait = temp;
        }
    }

    private void Start()
    {
        StartWaiting();
    }

    private void Update()
    {
        switch (_currentState)
        {
            case TelephoneStates.Idle:
                UpdateIdle();
                break;

            case TelephoneStates.Ringing:
                UpdateRinging();
                break;

            case TelephoneStates.Talking:
                UpdateTalking();
                break;
        }
    }

    private void StartWaiting()
    {
        _currentState = TelephoneStates.Idle;
        _timer = 0f;

        _randomWaitDuration = Random.Range(_minWait, _maxWait);
    }

    private void UpdateIdle()
    {
        _timer += Time.deltaTime;

        if (_timer >= _randomWaitDuration)
            StartRinging();
    }

    private void StartRinging()
    {
        _currentState = TelephoneStates.Ringing;
        _ringTimer = 0f;

        if (_audioSource != null)
        {
            _audioSource.Stop();

            if (_ringClip != null)
            {
                _audioSource.loop = true;
                _audioSource.clip = _ringClip;
                _audioSource.Play();
            }
        }

        if (_light != null)
            _light.enabled = true;
    }

    private void UpdateRinging()
    {
        _ringTimer += Time.deltaTime;

        ShakeReceiver();
        FlashLight();

        if (_ringTimer >= _ringDuration)
        {

            StopRinging();
            StartWaiting();
        }
    }

    private void FlashLight()
    {
        if (_light == null)
            return;

        float flash = Mathf.Sin(_ringTimer * _shakeSpeed);

        _light.enabled = flash > 0f;
    }

    private void ShakeReceiver()
    {
        if (_receiver == null)
            return;

        float angle = Mathf.Sin(_ringTimer * _shakeSpeed) * _shakeAngle;

        _receiver.localRotation =
            _originalReceiverRotation *
            Quaternion.Euler(0f, 0f, angle);
    }

    private void StopRinging()
    {
        if (_audioSource != null)
            _audioSource.Stop();

        if (_light != null)
            _light.enabled = false;

        ResetReceiver();
    }

    private void ResetReceiver()
    {
        if (_receiver != null)
            _receiver.localRotation = _originalReceiverRotation;
    }

    public void Interact()
    {
        if (_currentState != TelephoneStates.Ringing)
            return;

        StartTalking();
    }

    private void StartTalking()
    {
        StopRinging();

        _currentState = TelephoneStates.Talking;

        if (_audioSource != null && _answerClip != null)
        {
            _audioSource.loop = false;
            _audioSource.clip = _answerClip;
            _audioSource.Play();
        }
    }

    private void UpdateTalking()
    {
        // If there is no answer clip or AudioSource,
        // return to Idle immediately.
        if (_audioSource == null || _answerClip == null)
        {
            StartWaiting();
            return;
        }

        // Wait until the laughter audio finishes.
        if (!_audioSource.isPlaying)
            StartWaiting();
    }
}
