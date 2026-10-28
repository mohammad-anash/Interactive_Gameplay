using UnityEngine;

public class TestTap : MonoBehaviour, IInteractable
{
  [SerializeField] private string _interactText = "Change color";
  private Renderer _renderer;
  private int _colorIndex;

  private readonly Color[] _colors =
  {
    Color.red,
    Color.green,
    Color.blue,
    Color.yellow,
    Color.white
};

  public string InteractText => _interactText;

  private void Awake()
  {
    _renderer = GetComponent<Renderer>();
  }

  public void Interact()
  {
    if (_renderer == null)
      return;

    _colorIndex = (_colorIndex + 1) % _colors.Length;
    _renderer.material.color = _colors[_colorIndex];
  }
}
