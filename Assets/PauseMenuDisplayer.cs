using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuParent;
    [SerializeField] private RectTransform pauseMenuBackground;
    [SerializeField] private PlayerInput playerInput;
    private bool isPaused = false;

    void Start()
    {
        pauseMenuParent.SetActive(false);
    }

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            TogglePause();
        }
    }
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            pauseMenuParent.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            playerInput.SwitchCurrentActionMap("UI");
            pauseMenuBackground.LeanScale(Vector3.one * 0.9f, 5f).setLoopPingPong().setEaseInOutCubic();
        }
        else
        {
            pauseMenuParent.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.SwitchCurrentActionMap("Player");
            LeanTween.cancel(pauseMenuBackground);
        }
    }

    public void CancelMission()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToLobby();
        }
    }
}
