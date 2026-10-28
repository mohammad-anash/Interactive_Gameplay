using System;
using UnityEngine;
using UnityEngine.InputSystem;


// InteractionManager It job only to interact with GameObect.
public class InteractionManager : MonoBehaviour
{
  [Header("Interaction Settings")]
  [SerializeField, Min(0f)] private float _maxDistance = 3f;
  [SerializeField] private LayerMask _layer;
  [SerializeField] private Camera _mainCamera;

  public event Action<string> PromptChanged;

  private IInteractable _currentInteractable;
  private string _lastPrompt = string.Empty;

  private void Update()
  {
    if (_mainCamera == null)
      return;

    UpdateInteractable();

    if (Keyboard.current != null &&
        Keyboard.current.eKey.wasPressedThisFrame &&
        _currentInteractable != null)
    {
      _currentInteractable.Interact();
    }
  }

  private void UpdateInteractable()
  {
    IInteractable interactable = null;

    if (Physics.Raycast(
            _mainCamera.transform.position,
            _mainCamera.transform.forward,
            out RaycastHit hit,
            _maxDistance,
            _layer,
            QueryTriggerInteraction.Ignore))
    {
      interactable = hit.collider.GetComponentInParent<IInteractable>();
    }

    _currentInteractable = interactable;

    string prompt = _currentInteractable != null
        ? _currentInteractable.InteractText
        : string.Empty;

    if (prompt != _lastPrompt)
    {
      _lastPrompt = prompt;
      PromptChanged?.Invoke(prompt);
    }
  }
}
