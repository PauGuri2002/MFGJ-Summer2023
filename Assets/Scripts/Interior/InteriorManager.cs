using UnityEngine;

public class InteriorManager : MonoBehaviour
{
    [SerializeField] private Dialogue[] lobbyDialogues;
    [SerializeField] private LowerMenuDisplayer lobbyMenu;
    [SerializeField] private Camera cam;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private float startMissionDelay = 1f;

    [Header("Title")]
    [SerializeField] private RectTransform titleBackground;
    [SerializeField] private RectTransform titleText;
    [SerializeField] private float titleAppearDelay = 0.5f;
    [SerializeField] private float titleDuration = 8f;
    [SerializeField] private LowerMenuDisplayer titleMenu;
    static int timesLoaded = 0;

    void Start()
    {
        cam.cullingMask |= 1 << LayerMask.NameToLayer(GameManager.Instance.gameSeason.displayName);

        if (timesLoaded == 0)
        {
            // Display game title
            titleBackground.gameObject.SetActive(true);
            titleMenu.Hide(0);

            titleText.LeanScale(Vector3.zero, 0);
            titleBackground.LeanAlpha(0, 0);

            LTSeq textSeq = LeanTween.sequence();
            textSeq.append(titleAppearDelay);
            textSeq.append(titleText.LeanScale(Vector3.one * 0.9f, 2f).setEaseInOutCubic().setOnComplete(() => titleMenu.Show(0.5f)));
            textSeq.append(titleText.LeanScale(Vector3.one, titleDuration).setEaseInOutSine().setLoopPingPong());

            titleBackground.LeanAlpha(1, 1f).setEaseInOutCubic().setDelay(titleAppearDelay);
        }
        else
        {
            titleBackground.gameObject.SetActive(false);
            titleMenu.Hide(0);
            TryShowDialogue();
        }

        MusicPlayer.Instance.Play("INTERIOR");
        doorAnimator.SetBool("Open", false);
    }

    public void HideTitleScreen()
    {
        titleMenu.PlayClickSound();

        //Hide title screen
        if (titleBackground.gameObject.activeSelf)
        {
            LeanTween.cancel(titleText);
            LeanTween.cancel(titleBackground);
            titleText.LeanAlpha(0, 1f).setEaseInOutCubic();
            titleBackground.LeanAlpha(0, 1f).setEaseInOutCubic().setOnComplete(() =>
            {
                if (titleBackground != null)
                {
                    titleBackground.gameObject.SetActive(false);
                }
                TryShowDialogue();
            });
            titleMenu.Hide(0.5f);
        }
        else
        {
            TryShowDialogue();
        }
    }

    void TryShowDialogue()
    {
        if (lobbyDialogues.Length > timesLoaded)
        {
            // Show dialogue
            lobbyMenu.Hide(0);
            DialogueDisplayer.Instance.ShowDialogue(lobbyDialogues[timesLoaded], () => lobbyMenu.Show(0.5f));
        }
        else
        {
            lobbyMenu.Show(0.5f);
        }
        timesLoaded++;
    }

    public void StartMission()
    {
        doorAnimator.SetBool("Open", true);
        Invoke(nameof(StartAfterDelay), startMissionDelay);
    }

    void StartAfterDelay()
    {
        GameManager.Instance.StartMission();
    }
}
