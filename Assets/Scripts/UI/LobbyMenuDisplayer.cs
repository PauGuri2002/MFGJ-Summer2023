using UnityEngine;

public class LobbyMenuDisplayer : MonoBehaviour
{
    [SerializeField] private RectTransform menuParent;

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
        Application.Quit();
    }

    public void StartMission()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartMission();
        }
    }
}
