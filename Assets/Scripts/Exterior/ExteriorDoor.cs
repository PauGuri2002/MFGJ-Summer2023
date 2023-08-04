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
        ExteriorManager.onPhaseChange += ToggleDoor;
    }

    void ToggleDoor(ExteriorManager.GamePhase gamePhase)
    {
        isOpen = gamePhase == ExteriorManager.GamePhase.ReturnHome;

        if (isOpen)
        {
            print("Door opened!");
            doorAnimator.SetBool("Locked", false);
        }
        else
        {
            print("Door closed!");
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
            DialogueDisplayer.Instance.ShowNotice("I haven't gathered all the ingredients I need yet.");
        }
    }
}
