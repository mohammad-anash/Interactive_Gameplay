using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingScreen : MonoBehaviour
{
    // ----- Required References ------------------------
    [Header("Ending Screen References")]
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private TextMeshProUGUI _escapeTimeText;
    [SerializeField] private UIPanel _endingPanel;
    [SerializeField] private PlayerAndCameraController _controller;

    // ----- Is Required References Are Assigned ------------------------
    private bool HasMissingReferences =>
        _gameTimer == null ||
        _escapeTimeText == null ||
        _endingPanel == null ||
        _controller == null;

    private bool _hasEnded;

    private void Awake()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (HasMissingReferences)
        {
            Debug.LogError("EndingScreen: Required references are missing!", this);
            return;
        }

        _endingPanel.HideImmediate();
    }

    // ----- Show Ending ------------------------
    public void ShowEnding()
    {
        if (HasMissingReferences || _hasEnded) return;

        _hasEnded = true;

        _escapeTimeText.text = $"Escape Time: {_gameTimer.GetFormattedTime()}";

        _gameTimer.Stop();

        _endingPanel.Show();

        _controller.DisablePlayerAndCameraMovement();

        Time.timeScale = 0f;
    }

    // ----- Restart Game ------------------------
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ----- Quit Game ------------------------
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
