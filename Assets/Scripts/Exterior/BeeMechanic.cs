using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeMechanic : MonoBehaviour
{
    [SerializeField] private ExteriorManager exteriorManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private float beeFollowTime = 3;
    [SerializeField] private float beeTravelTime = 0.5f;
    [SerializeField] private float beesMaxSpeed = 2;

    private float originalMaxSpeed;
    private static List<Coroutine> coroutines = new();

    private void Start()
    {
        originalMaxSpeed = player.horizontalMaxSpeed;
    }

    public void StartBees(Transform beeParticles)
    {
        coroutines.Add(StartCoroutine(BeesCoroutine(beeParticles.parent, beeParticles)));
    }

    IEnumerator BeesCoroutine(Transform originalParent, Transform beeParticles)
    {
        player.horizontalMaxSpeed = beesMaxSpeed;

        float progress = 0f;
        while (progress < 1)
        {
            beeParticles.position = Vector3.Lerp(originalParent.position, player.transform.position, progress);
            progress += 1 / beeTravelTime * Time.deltaTime;
            yield return 0;
        }

        progress = 0f;
        while (progress < beeFollowTime)
        {
            beeParticles.position = player.transform.position;
            progress += Time.deltaTime;
            yield return 0;
        }

        IngredientInfo removedIngredient = exteriorManager.RemoveCollectedIngredient();
        GameObject ingredientInstance = null;
        if (removedIngredient != null)
        {
            ingredientInstance = Instantiate(removedIngredient.prefab, beeParticles, false);
            ingredientInstance.layer = LayerMask.NameToLayer("Spring");
            if (ingredientInstance.TryGetComponent<Ingredient>(out var ingredientScript))
            {
                ingredientScript.ToggleCollision(false);
            }
        }

        if (coroutines.Count <= 1)
        {
            player.horizontalMaxSpeed = originalMaxSpeed;
        }
        coroutines.RemoveAt(coroutines.Count - 1);

        progress = 0f;
        while (progress < 1)
        {
            beeParticles.position = Vector3.Lerp(player.transform.position, originalParent.position, progress);
            progress += 1 / beeTravelTime * Time.deltaTime;
            yield return 0;
        }

        if (ingredientInstance != null)
        {
            Destroy(ingredientInstance);
        }
    }
}
