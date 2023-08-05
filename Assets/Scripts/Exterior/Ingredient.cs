using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ingredient : MonoBehaviour, IInteractive
{
    public string ingredientName;
    private float specialAnimationDelay;
    private readonly float specialAnimationTime = 1f;
    private bool specialAnimationEnabled;

    private readonly float rotationTime = 6f;

    private void Awake()
    {
        ExteriorManager.OnIngredientListUpdate += CheckIfCanPickup;
    }

    private void Start()
    {
        gameObject.LeanRotateAround(Vector3.up, 360f, rotationTime).setRepeat(-1);
    }

    void CheckIfCanPickup(Dictionary<IngredientInfo, int> ingredientsToCollect)
    {
        foreach (IngredientInfo ingredient in ingredientsToCollect.Keys)
        {
            if (ingredient.name == ingredientName)
            {
                StartSpecialAnimation();
                return;
            }
        }
        StopSpecialAnimation();
    }

    void StartSpecialAnimation()
    {
        if (specialAnimationEnabled) { return; }
        specialAnimationEnabled = true;
        specialAnimationDelay = Random.Range(2.0f, 3.0f);
        StartCoroutine(AnimateScale());
    }

    void StopSpecialAnimation()
    {
        if (!specialAnimationEnabled) { return; }
        specialAnimationEnabled = false;
    }

    IEnumerator AnimateScale()
    {
        while (specialAnimationEnabled)
        {
            gameObject.LeanScale(Vector3.one * 1.5f, specialAnimationTime / 3).setEaseOutCubic();
            gameObject.LeanScale(Vector3.one, specialAnimationTime / 3 * 2).setEaseOutBounce().setDelay(specialAnimationTime / 3);

            yield return new WaitForSeconds(specialAnimationDelay + specialAnimationTime);
        }
    }

    public void Interact()
    {
        StopSpecialAnimation();
        LeanTween.cancel(gameObject);

        gameObject.LeanRotateAround(Vector3.up, 360f, 0.5f).setRepeat(-1);
        gameObject.LeanScale(Vector3.zero, 0.5f).setEaseInBack().setOnComplete(() =>
        {
            gameObject.SetActive(false);
            ExteriorManager.OnIngredientListUpdate -= CheckIfCanPickup;
        });
    }

    private void OnDestroy()
    {
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
