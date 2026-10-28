using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MirrorController : MonoBehaviour, IInteractable
{
  private enum MirrorState
  {
    Idle,
    FadingIn,
    Visible,
    FadingOut,
    Cooldown
  }

  [Header("Ghost")]
  [SerializeField] private GameObject _ghost;
  [SerializeField, Min(0.01f)] private float _fadeDuration = 1f;
  [SerializeField, Min(0.01f)] private float _visibleDuration = 2f;
  [SerializeField, Min(0.01f)] private float _cooldownDuration = 10f;

  [Header("Events")]
  // FormerlySerializedAs keeps your old Inspector connections
  [SerializeField, FormerlySerializedAs("OnHaunted")]
  private UnityEvent _onHaunted;

  private MirrorState _state = MirrorState.Idle;
  private Material _ghostMaterial;
  private Coroutine _hauntingRoutine;

  private bool HasValidSetup => _ghost != null && _ghostMaterial != null;

  // Empty text means "can't use right now"
  public string InteractText =>
      HasValidSetup && _state == MirrorState.Idle
          ? "Look into the mirror"
          : string.Empty;

  private void Awake()
  {
    if (_ghost != null)
    {
      Renderer ghostRenderer = _ghost.GetComponent<Renderer>();

      if (ghostRenderer != null)
      {
        // An instance, so we don't change the shared material
        _ghostMaterial = ghostRenderer.material;
        SetGhostAlpha(0f);
      }

      _ghost.SetActive(false);
    }

    if (!HasValidSetup)
    {
      Debug.LogError(
          "MirrorController: Assign a Ghost object that has a Renderer.", this);
    }
  }

  public void Interact()
  {
    if (!HasValidSetup || _state != MirrorState.Idle)
      return;

    _hauntingRoutine = StartCoroutine(HauntingSequence());
  }

  private IEnumerator HauntingSequence()
  {
    _state = MirrorState.FadingIn;
    _ghost.SetActive(true);

    // The ghost appears and triggers whatever is connected (the candle)
    _onHaunted?.Invoke();

    yield return FadeGhost(0f, 1f, _fadeDuration);

    _state = MirrorState.Visible;
    yield return new WaitForSeconds(_visibleDuration);

    _state = MirrorState.FadingOut;
    yield return FadeGhost(1f, 0f, _fadeDuration);

    _ghost.SetActive(false);
    _state = MirrorState.Cooldown;

    yield return new WaitForSeconds(_cooldownDuration);

    _state = MirrorState.Idle;
    _hauntingRoutine = null;
  }

  private IEnumerator FadeGhost(float startAlpha, float endAlpha, float duration)
  {
    float elapsed = 0f;

    while (elapsed < duration)
    {
      elapsed += Time.deltaTime;
      SetGhostAlpha(Mathf.Lerp(startAlpha, endAlpha, elapsed / duration));
      yield return null;
    }

    SetGhostAlpha(endAlpha);
  }

  private void SetGhostAlpha(float alpha)
  {
    if (_ghostMaterial == null)
      return;

    Color color = _ghostMaterial.color;
    color.a = alpha;
    _ghostMaterial.color = color;
  }

  private void OnDisable()
  {
    // If we are turned off in the middle of a haunting, reset everything,
    // or the mirror would stay stuck in a non-Idle state forever.
    if (_hauntingRoutine != null)
      StopCoroutine(_hauntingRoutine);

    _hauntingRoutine = null;
    _state = MirrorState.Idle;

    if (_ghostMaterial != null)
      SetGhostAlpha(0f);

    if (_ghost != null)
      _ghost.SetActive(false);
  }

  private void OnDestroy()
  {
    // Renderer.material made a copy, so we clean it up
    if (_ghostMaterial != null)
      Destroy(_ghostMaterial);
  }
}