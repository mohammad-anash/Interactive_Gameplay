using UnityEngine;

public class Treasure : MonoBehaviour, IInteractable
{

    // --- Required References ---------------

    [Header("References")]
    [SerializeField] private Door _door;
    [SerializeField] private EndingScreen _endingScreen;


    // --- Is Required References Are Assigned ---------------
    private bool HasMissingReferences => _door == null || _endingScreen == null;


    // --- Interact Text ----------------
    public string InteractText => !HasMissingReferences && !_isCollected && _door.IsOpen ? "Collect Treasure" : string.Empty;


    // == Is Treasure Collected ------------
    private bool _isCollected;



    // --- Unity Life Cycle --------------
    private void Awake()
    {
        if (HasMissingReferences) Debug.LogWarning("Required Reference Are Missing");
    }


    // -- Interact With Object ------------------
    public void Interact()
    {

        if (HasMissingReferences) return;
        if (_isCollected) return;

        _isCollected = true;
        gameObject.SetActive(false);

        _endingScreen.ShowEnding();
        AudioListener.pause = true;
    }
}
