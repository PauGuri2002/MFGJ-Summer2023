using UnityEngine;

public class Ingredient : MonoBehaviour, IInteractive
{
    public string ingredientName;

    public void Interact()
    {
        gameObject.SetActive(false);
    }
}
