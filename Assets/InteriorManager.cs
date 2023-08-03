using UnityEngine;

public class InteriorManager : MonoBehaviour
{
    [SerializeField] private Dialogue[] lobbyDialogues;
    [SerializeField] private RectTransform lobbyMenu;
    [SerializeField] private RectTransform titleBackground;
    [SerializeField] private RectTransform titleText;
    [SerializeField] private Camera cam;
    static int timesLoaded = 0;

    void Start()
    {
        cam.cullingMask |= 1 << LayerMask.NameToLayer(GameManager.Instance.gameSeason.displayName);

        if (timesLoaded == 0)
        {
            // Initial dialogue
            if (lobbyDialogues.Length > 0)
            {
                lobbyMenu.LeanMove(Vector3.down * 275, 0);
                DialogueDisplayer.Instance.ShowDialogue(lobbyDialogues[0], () => lobbyMenu.LeanMove(Vector3.zero, 0.5f).setEaseInOutCubic());
            }

            // Display game title
            titleBackground.gameObject.SetActive(true);

            titleText.LeanScale(Vector3.zero, 0);
            titleBackground.LeanAlpha(0, 0);

            LTSeq textSeq = LeanTween.sequence();
            textSeq.append(2f);
            textSeq.append(titleText.LeanScale(Vector3.one * 0.9f, 2f).setEaseInOutCubic());
            textSeq.append(titleText.LeanScale(Vector3.one, 10f).setEaseOutCubic());
            textSeq.append(titleText.LeanAlpha(0, 1f).setEaseInOutCubic());

            LTSeq bgSeq = LeanTween.sequence();
            bgSeq.append(2f);
            bgSeq.append(titleBackground.LeanAlpha(1, 1f).setEaseInOutCubic());
            bgSeq.append(11f);
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
                lobbyMenu.LeanMove(Vector3.down * 275, 0);
                DialogueDisplayer.Instance.ShowDialogue(lobbyDialogues[timesLoaded], () => lobbyMenu.LeanMove(Vector3.zero, 0.5f).setEaseInOutCubic());
            }
        }

    }

    //private void OnDestroy()
    //{
    //    LeanTween.cancel(titleText);
    //    LeanTween.cancel(titleBackground);
    //}
}
