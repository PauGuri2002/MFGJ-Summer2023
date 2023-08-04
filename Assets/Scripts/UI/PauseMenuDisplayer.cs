using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PauseMenuDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuParent;
    [SerializeField] private RectTransform pauseMenuBackground;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixer audioMixer;
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
        PlayClickSound();
        isPaused = !isPaused;
        SetPauseActive(isPaused);
    }

    void SetPauseActive(bool value)
    {
        if (value)
        {
            pauseMenuParent.SetActive(true);
            Time.timeScale = 0f;
            audioMixer.SetFloat("MasterVolume", -80f);
            Cursor.lockState = CursorLockMode.None;
            playerInput.SwitchCurrentActionMap("UI");
            pauseMenuBackground.LeanScale(Vector3.one * 0.9f, 5f).setLoopPingPong().setEaseInOutCubic();
        }
        else
        {
            pauseMenuParent.SetActive(false);
            Time.timeScale = 1f;
            audioMixer.SetFloat("MasterVolume", 0f);
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.SwitchCurrentActionMap("Player");
            LeanTween.cancel(pauseMenuBackground);
        }
    }

    public void CancelMission()
    {
        if (GameManager.Instance != null)
        {
            PlayClickSound();
            isPaused = false;
            SetPauseActive(false);
            GameManager.Instance.ReturnToLobby();
        }
    }

    void PlayClickSound()
    {
        audioSource.Stop();
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
    }
}
