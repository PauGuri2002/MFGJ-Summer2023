using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractive interactiveInReach;
    [SerializeField] private ExteriorManager exteriorManager;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    public void OnAction(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            if (interactiveInReach == null) { return; }

            animator.SetTrigger("Grab");
            audioSource.Stop();
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.Play();

            if (interactiveInReach is Ingredient ingredient)
            {
                if (exteriorManager.TryCollectIngredient(ingredient.ingredientName))
                {
                    interactiveInReach.Interact();
                    interactiveInReach = null;
                }

                return;
            }

            interactiveInReach.Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<IInteractive>(out var interactiveScript))
        {
            print(other.name + " entered");
            interactiveInReach = interactiveScript;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<IInteractive>(out var interactiveScript))
        {
            if (interactiveInReach == interactiveScript)
            {
                print(other.name + " exited");
                interactiveInReach = null;
            }
        }
    }
}
