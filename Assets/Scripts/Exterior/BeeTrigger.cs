using UnityEngine;

public class BeeTrigger : MonoBehaviour
{
    private BeeMechanic mechanic;

    private void Start()
    {
        mechanic = FindObjectOfType<BeeMechanic>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
            if (particles != null)
            {
                mechanic.StartBees(particles.transform);
            }
        }
    }
}
