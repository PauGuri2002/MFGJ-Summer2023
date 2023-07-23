using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExteriorManager : MonoBehaviour
{
    [SerializeField] private Vector2 spawnerSize;

    [SerializeField] private GameObject ingredientListObject;
    [SerializeField] private GameObject ingredientItemPrefab;
    [SerializeField] private int minExtraIngredients = 6;
    [SerializeField] private int maxExtraIngredients = 10;
    public Timer timer;

    private Dictionary<IngredientInfo, int> ingredientsToCollect;

    public static GamePhase currentPhase;
    public static event Action<GamePhase> onPhaseChange;
    public enum GamePhase
    {
        Search,
        ReturnHome
    }

    public void Start()
    {
        if (GameManager.Instance == null || GameManager.Instance.ingredientList == null) { return; }

        ingredientsToCollect = GameManager.Instance.ingredientList;

        foreach (var item in GameManager.Instance.ingredientList)
        {
            // Populate ingredient list
            GameObject ingredientItem = Instantiate(ingredientItemPrefab, ingredientListObject.transform);
            if (ingredientItem.TryGetComponent<IngredientItemCustomizer>(out var customizer))
            {
                SeasonInfo seasonInfo = Array.Find(GameManager.seasons, s => s.seasonName == item.Key.season);

                customizer.Customize(
                    item.Key.name + " x" + item.Value.ToString(),
                    seasonInfo.softColor,
                    item.Key.icon
                );

            }

            // Spawn required ingredients in world
            for (int i = 0; i < item.Value; i++)
            {
                SpawnIngredient(item.Key);
            }
        }

        // Spawn extra ingredients in world
        int extraIngredientCount = Random.Range(minExtraIngredients, maxExtraIngredients + 1);
        for (int i = 0; i < extraIngredientCount; i++)
        {
            SpawnIngredient();
        }

        // Start timer
        timer.StartTimer();
        currentPhase = GamePhase.Search;
        onPhaseChange?.Invoke(currentPhase);
    }

    void SpawnIngredient(IngredientInfo ingredient = null)
    {
        Vector3 spawnPosition;
        ingredient ??= GameManager.ingredients[Random.Range(0, GameManager.ingredients.Length)];

        do
        {
            spawnPosition = new Vector3(Random.Range(0, spawnerSize.x) - spawnerSize.x / 2, 0.5f, Random.Range(0, spawnerSize.y) - spawnerSize.y / 2);
        } while (Physics.CheckBox(spawnPosition, new Vector3(5, 0.1f, 5)));

        GameObject instance = Instantiate(ingredient.prefab, spawnPosition, Quaternion.identity);
        instance.layer = (int)ingredient.season;

        // PROVISIONAL!!!! Fins que cada ingredient tingui el seu model
        instance.GetComponentInChildren<TextMeshProUGUI>().text = ingredient.name;
        instance.GetComponentInChildren<TextMeshProUGUI>().gameObject.layer = (int)ingredient.season;
        instance.GetComponent<Ingredient>().ingredientName = ingredient.name;
    }

    public bool TryCollectIngredient(IngredientInfo ingredient)
    {
        if (ingredientsToCollect.ContainsKey(ingredient))
        {
            ingredientsToCollect[ingredient]--;
            if (ingredientsToCollect[ingredient] <= 0)
            {
                ingredientsToCollect.Remove(ingredient);
                CheckCollectedIngredients();
            }

            return true;
        }
        else { return false; }
    }
    public bool TryCollectIngredient(string ingredientName)
    {
        IngredientInfo ingredient = Array.Find(GameManager.ingredients, i => i.name == ingredientName);
        return TryCollectIngredient(ingredient);
    }

    void CheckCollectedIngredients()
    {
        if (ingredientsToCollect.Count <= 0)
        {
            if (currentPhase != GamePhase.ReturnHome)
            {
                currentPhase = GamePhase.ReturnHome;
                onPhaseChange?.Invoke(currentPhase);
            }
        }
        else if (currentPhase != GamePhase.Search)
        {
            currentPhase = GamePhase.Search;
            onPhaseChange?.Invoke(currentPhase);
        }
    }
}
