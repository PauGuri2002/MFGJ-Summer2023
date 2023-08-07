using UnityEngine;

public class ExteriorDoor : MonoBehaviour, IInteractive
{
    [SerializeField] private ExteriorManager exteriorManager;
    [SerializeField] private Timer timer;
    [SerializeField] private Animator doorAnimator;
    private bool isOpen;

    void Start()
    {
        ToggleDoor(ExteriorManager.GamePhase.Search);
        ExteriorManager.OnPhaseChange += ToggleDoor;
    }

    private void OnDisable()
    {
        ExteriorManager.OnPhaseChange -= ToggleDoor;
    }

    void ToggleDoor(ExteriorManager.GamePhase gamePhase)
    {
        isOpen = gamePhase == ExteriorManager.GamePhase.ReturnHome;

        if (isOpen)
        {
            doorAnimator.SetBool("Locked", false);
        }
        else
        {
            doorAnimator.SetBool("Locked", true);
        }
    }

    public void Interact()
    {
        if (isOpen)
        {
            timer.StopTimer();
            float time = timer.elapsedTime;
            GameManager.Instance.CompleteMission(time);
        }
        else
        {
            doorAnimator.SetTrigger("TryOpen");
            NoticeDisplayer.Instance.ShowNotice("I haven't gathered all the ingredients I need yet.");
        }
    }
}
