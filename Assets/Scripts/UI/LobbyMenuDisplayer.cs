using UnityEngine;

public class LobbyMenuDisplayer : MonoBehaviour
{
    [SerializeField] private RectTransform menuParent;
    [SerializeField] private AudioSource audioSource;

    public void Show(float time)
    {
        menuParent.gameObject.SetActive(true);
        LeanTween.cancel(menuParent);
        menuParent.LeanMove(Vector3.zero, time).setEaseInOutCubic();
    }

    public void Hide(float time)
    {
        LeanTween.cancel(menuParent);
        menuParent.LeanMove(Vector3.down * 275, time).setEaseInOutCubic().setOnComplete(() => menuParent.gameObject.SetActive(false));
    }

    public void QuitGame()
    {
        PlayClickSound();
        Application.Quit();
    }

    public void StartMission()
    {
        if (GameManager.Instance != null)
        {
            PlayClickSound();
            GameManager.Instance.StartMission();
        }
    }

    void PlayClickSound()
    {
        audioSource.Stop();
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
    }
}
