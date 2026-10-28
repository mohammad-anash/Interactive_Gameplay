using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonDebugger : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    private void Awake()
    {
        Debug.Log($"[UI Debug] {name}: Debugger initialized.", this);

        if (GetComponent<Button>() == null)
        {
            Debug.LogWarning(
                $"[UI Debug] {name}: No Button component found.", this);
        }

        if (GetComponent<Graphic>() == null)
        {
            Debug.LogWarning(
                $"[UI Debug] {name}: No Image or Graphic component found.", this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"[UI Debug] Mouse ENTERED {name}.", this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"[UI Debug] Mouse EXITED {name}.", this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[UI Debug] Mouse CLICKED {name}.", this);
    }
}
