using UnityEngine;

public class PlayerAndCameraController : MonoBehaviour
{
    [Header("References of Player & Camera Controller")]
    [SerializeField] private CameraController _mainCamera;
    [SerializeField] private MonoBehaviour _playerMovement;


    private void Awake()
    {
        if (_mainCamera == null)
        {
            Debug.LogError("CameraController is not assigned!", this);
        }

        if (_playerMovement == null)
        {
            Debug.LogError("Player movement is not assigned!", this);
        }
    }


    // ----- Disable Camera And Player Movement ---------------
    public void DisablePlayerAndCameraMovement()
    {
        if (_mainCamera == null || _playerMovement == null) return;

        _mainCamera.enabled = false;
        _playerMovement.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ----- Enable Camera And Player Movement ---------------
    public void EnablePlayerAndCameraMovement()
    {
        if (_mainCamera == null || _playerMovement == null) return;

        _mainCamera.enabled = true;
        _playerMovement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
