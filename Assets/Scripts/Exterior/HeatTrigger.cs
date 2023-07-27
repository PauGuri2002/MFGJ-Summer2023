using UnityEngine;

public class HeatTrigger : MonoBehaviour
{
    private HeatMechanic mechanic;

    private void Start()
    {
        mechanic = FindObjectOfType<HeatMechanic>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mechanic.CoolDown();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mechanic.HeatUp();
        }
    }
}
