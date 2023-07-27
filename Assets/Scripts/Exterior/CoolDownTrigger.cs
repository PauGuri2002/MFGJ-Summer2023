using UnityEngine;

public class CoolDownTrigger : MonoBehaviour
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
}
