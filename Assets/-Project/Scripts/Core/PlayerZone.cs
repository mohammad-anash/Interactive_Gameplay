using UnityEngine;

// Reports whether the player is inside this box area.
// It checks directly every time it is asked, so it does not depend on
// trigger events (which are unreliable on moving objects).
// It only REPORTS. The moving object decides what to do.
[RequireComponent(typeof(BoxCollider))]
public class PlayerZone : MonoBehaviour
{
  [Tooltip("Put the Player on this layer, and select only that layer here.")]
  [SerializeField] private LayerMask _playerLayer;

  private BoxCollider _box;

  public bool IsPlayerInside
  {
    get
    {
      if (_box == null)
        _box = GetComponent<BoxCollider>();

      // Convert the box collider's shape into world space
      Vector3 center = transform.TransformPoint(_box.center);
      Vector3 halfExtents =
          Vector3.Scale(_box.size, transform.lossyScale) * 0.5f;

      return Physics.CheckBox(
          center,
          halfExtents,
          transform.rotation,
          _playerLayer,
          QueryTriggerInteraction.Ignore);
    }
  }

  private void Reset()
  {
    // The zone is only a shape, so it must never push anything
    GetComponent<BoxCollider>().isTrigger = true;
  }

  private void OnValidate()
  {
    if (_playerLayer.value == 0)
      Debug.LogWarning("PlayerZone: Select the Player layer.", this);
  }

  // Green = player is outside, red = player is inside (visible in Scene view)
  private void OnDrawGizmos()
  {
    BoxCollider box = GetComponent<BoxCollider>();
    if (box == null)
      return;

    Gizmos.color = Application.isPlaying && IsPlayerInside
        ? Color.red
        : Color.green;

    Gizmos.matrix = transform.localToWorldMatrix;
    Gizmos.DrawWireCube(box.center, box.size);
  }
}