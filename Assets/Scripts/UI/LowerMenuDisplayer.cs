using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class LowerMenuDisplayer : MonoBehaviour
{
    [SerializeField] private RectTransform menuParent;
    [SerializeField] private AudioSource audioSource;
    private Button[] buttons;

    public virtual void Show(float time, float delay = 0f)
    {
        ToggleInteractivity(true);
        menuParent.gameObject.SetActive(true);
        LeanTween.cancel(menuParent);
        menuParent.LeanMove(Vector3.zero, time).setEaseInOutCubic().setDelay(delay);
    }

    public virtual void Hide(float time, float delay = 0f)
    {
        ToggleInteractivity(false);
        LeanTween.cancel(menuParent);
        menuParent.LeanMove(Vector3.down * 275, time).setEaseInOutCubic().setDelay(delay).setOnComplete(() => menuParent.gameObject.SetActive(false));
    }

    void ToggleInteractivity(bool value)
    {
        buttons ??= menuParent.GetComponentsInChildren<Button>();

        foreach (Button btn in buttons)
        {
            btn.interactable = value;
        }
    }

    public void PlayClickSound()
    {
        audioSource.Stop();
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
    }
}
