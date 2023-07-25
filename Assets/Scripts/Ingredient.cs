using UnityEngine;

public class Ingredient : MonoBehaviour, IInteractive
{
    public string ingredientName;

    public void Interact()
    {
        gameObject.SetActive(false);
    }

    public void ToggleCollision(bool value)
    {
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = value;
        }
    }
}
