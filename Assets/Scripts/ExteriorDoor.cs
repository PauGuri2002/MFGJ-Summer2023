using UnityEngine;

public class ExteriorDoor : MonoBehaviour, IInteractive
{
    [SerializeField] private ExteriorManager exteriorManager;
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
            exteriorManager.timer.StopTimer();
            float time = exteriorManager.timer.elapsedTime;
            GameManager.Instance.CompleteMission(time);
        }
        else
        {
            print("DIALOGUE: I still have to gather some ingredients.");
        }
    }
}
