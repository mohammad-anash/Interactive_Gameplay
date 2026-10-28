using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractable
{
  [Header("References")]
  [SerializeField] private Light _light;

  // Empty text means "can't use right now" (no light assigned)
  public string InteractText
  {
    get
    {
      if (_light == null)
        return string.Empty;

      return _light.enabled
       ? "Turn off the light"
       : "Turn on the light";
    }
  }

  private void Awake()
  {
    if (_light == null)
      Debug.LogError("LightSwitch: Light reference is missing!", this);
  }

  public void Interact()
  {
    if (_light == null)
      return;

    _light.enabled = !_light.enabled;
  }
}