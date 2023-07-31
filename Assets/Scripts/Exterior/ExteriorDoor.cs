using UnityEngine;

public class ExteriorDoor : MonoBehaviour, IInteractive
{
    [SerializeField] private ExteriorManager exteriorManager;
    [SerializeField] private Timer timer;
    private bool isOpen;

    void Start()
    {
        ExteriorManager.onPhaseChange += ToggleDoor;
    }

    void ToggleDoor(ExteriorManager.GamePhase gamePhase)
    {
        isOpen = gamePhase == ExteriorManager.GamePhase.ReturnHome;

        if (isOpen)
        {
            print("Door opened!");
        }
        else
        {
            print("Door closed!");
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
            DialogueDisplayer.Instance.ShowNotice("I haven't gathered all the ingredients I need yet.");
        }
    }
}
