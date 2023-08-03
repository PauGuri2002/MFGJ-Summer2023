using UnityEngine;

public class InteriorManager : MonoBehaviour
{
    [SerializeField] private Dialogue lobbyDialogue;
    [SerializeField] private RectTransform lobbyMenu;
    [SerializeField] private Camera cam;
    static bool firstTime = true;

    void Start()
    {
        cam.cullingMask |= 1 << LayerMask.NameToLayer(GameManager.Instance.gameSeason.displayName);

        if (firstTime)
        {
            // Initial dialogue
            lobbyMenu.LeanMove(Vector3.down * 275, 0);
            DialogueDisplayer.Instance.ShowDialogue(lobbyDialogue, () => lobbyMenu.LeanMove(Vector3.zero, 0.5f).setEaseInOutCubic());

            firstTime = false;
        }

    }

    void Update()
    {

    }
}
