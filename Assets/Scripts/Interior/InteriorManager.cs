using UnityEngine;

public class InteriorManager : MonoBehaviour
{
    [SerializeField] private Dialogue[] lobbyDialogues;
    [SerializeField] private LobbyMenuDisplayer lobbyMenu;
    [SerializeField] private Camera cam;

    [Header("Title")]
    [SerializeField] private RectTransform titleBackground;
    [SerializeField] private RectTransform titleText;
    [SerializeField] private float titleAppearDelay = 1f;
    [SerializeField] private float titleDuration = 8f;
    static int timesLoaded = 0;

    void Start()
    {
        cam.cullingMask |= 1 << LayerMask.NameToLayer(GameManager.Instance.gameSeason.displayName);

        if (timesLoaded == 0)
        {
            // Initial dialogue
            if (lobbyDialogues.Length > 0)
            {
                lobbyMenu.Hide(0);
                DialogueDisplayer.Instance.ShowDialogue(lobbyDialogues[0], () => lobbyMenu.Show(0.5f));
            }

            // Display game title
            titleBackground.gameObject.SetActive(true);

            titleText.LeanScale(Vector3.zero, 0);
            titleBackground.LeanAlpha(0, 0);

            LTSeq textSeq = LeanTween.sequence();
            textSeq.append(titleAppearDelay);
            textSeq.append(titleText.LeanScale(Vector3.one * 0.9f, 2f).setEaseInOutCubic());
            textSeq.append(titleText.LeanScale(Vector3.one, titleDuration).setEaseOutCubic());
            textSeq.append(titleText.LeanAlpha(0, 1f).setEaseInOutCubic());

            LTSeq bgSeq = LeanTween.sequence();
            bgSeq.append(titleAppearDelay);
            bgSeq.append(titleBackground.LeanAlpha(1, 1f).setEaseInOutCubic());
            bgSeq.append(titleDuration + 1);
            bgSeq.append(titleBackground.LeanAlpha(0, 1f).setEaseInOutCubic());
            bgSeq.append(() => { if (titleBackground != null) { titleBackground.gameObject.SetActive(false); } });

            timesLoaded++;
        }
        else
        {
            titleBackground.gameObject.SetActive(false);

            if (lobbyDialogues.Length > timesLoaded)
            {
                // Show dialogue
                lobbyMenu.Hide(0);
                DialogueDisplayer.Instance.ShowDialogue(lobbyDialogues[timesLoaded], () => lobbyMenu.Show(0.5f));
            }
        }

        MusicPlayer.Instance.Play("INTERIOR");
    }

    //private void OnDestroy()
    //{
    //    LeanTween.cancel(titleText);
    //    LeanTween.cancel(titleBackground);
    //}
}
