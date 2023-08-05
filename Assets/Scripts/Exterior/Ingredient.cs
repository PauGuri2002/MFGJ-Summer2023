using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ingredient : MonoBehaviour, IInteractive
{
    public string ingredientName;
    private float animationDelay;
    private float animationTime = 1f;
    private bool animationEnabled;

    private void Awake()
    {
        ExteriorManager.OnIngredientListUpdate += CheckIfCanPickup;
    }

    void CheckIfCanPickup(Dictionary<IngredientInfo, int> ingredientsToCollect)
    {
        foreach (IngredientInfo ingredient in ingredientsToCollect.Keys)
        {
            if (ingredient.name == ingredientName)
            {
                StartAnimation();
                return;
            }
        }
        StopAnimation();
    }

    void StartAnimation()
    {
        if (animationEnabled) { return; }
        animationEnabled = true;
        animationDelay = Random.Range(2.0f, 3.0f);
        StartCoroutine(AnimateScale());
    }

    void StopAnimation()
    {
        if (!animationEnabled) { return; }
        animationEnabled = false;
    }

    IEnumerator AnimateScale()
    {
        while (animationEnabled)
        {
            gameObject.LeanScale(Vector3.one * 1.5f, animationTime / 3).setEaseOutCubic();
            gameObject.LeanScale(Vector3.one, animationTime / 3 * 2).setEaseOutBounce().setDelay(animationTime / 3);

            yield return new WaitForSeconds(animationDelay + animationTime);
        }
    }

    public void Interact()
    {
        StopAnimation();
        gameObject.SetActive(false);
        ExteriorManager.OnIngredientListUpdate -= CheckIfCanPickup;
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
