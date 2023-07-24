using UnityEngine;

public class FreezeTrigger : MonoBehaviour
{
    private FreezeMechanic mechanic;

    private void Start()
    {
        mechanic = FindObjectOfType<FreezeMechanic>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mechanic.StartFreeze();
        }
    }
}
