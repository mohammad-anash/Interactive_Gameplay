public interface IInteractable
{
    // An empty text means the object cannot be used right now.
    string InteractText { get; }

    void Interact();
}
