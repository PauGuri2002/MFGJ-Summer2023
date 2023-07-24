using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExteriorManager : MonoBehaviour
{
    [SerializeField] private Vector2 spawnerSize;
    [SerializeField] private float spawnerHeight = 0.5f;
    [SerializeField] private float spawnPadding = 5f;

    [SerializeField] private GameObject ingredientListObject;
    [SerializeField] private GameObject ingredientItemPrefab;
    [SerializeField] private int minExtraIngredients = 6;
    [SerializeField] private int maxExtraIngredients = 10;
    public Timer timer;

    private Dictionary<IngredientInfo, int> ingredientsToCollect;
    private List<IngredientInfo> collectedIngredients = new();

    private Dictionary<IngredientInfo, IngredientItem> ingredientItems = new();

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

        ingredientsToCollect = new(GameManager.Instance.ingredientList);
        List<IngredientInfo> sortedIngredients = ingredientsToCollect.Keys.OrderBy(i => (int)i.season).ToList();

        foreach (IngredientInfo item in sortedIngredients)
        {
            // Spawn ingredient list items (UI)
            GameObject ingredientItem = Instantiate(ingredientItemPrefab, ingredientListObject.transform);
            if (ingredientItem.TryGetComponent<IngredientItem>(out var ingredientScript))
            {
                SeasonInfo seasonInfo = Array.Find(GameManager.seasons, s => s.seasonName == item.season);
                ingredientScript.Customize(
                item.name,
                ingredientsToCollect[item],
                seasonInfo.softColor,
                    item.icon
                );
                ingredientItems.Add(item, ingredientScript);
            }

            // Spawn required ingredients in world
            for (int i = 0; i < ingredientsToCollect[item]; i++)
            {
                SpawnIngredient(item);
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

        bool positionFound;
        do
        {
            spawnPosition = new Vector3(Random.Range(0, spawnerSize.x) - spawnerSize.x / 2, spawnerHeight, Random.Range(0, spawnerSize.y) - spawnerSize.y / 2);
            positionFound = true;

            Collider[] colliders = Physics.OverlapSphere(spawnPosition, spawnPadding);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Impenetrable"))
                {
                    positionFound = false;
                    break;
                }
            }
        } while (!positionFound);

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
            collectedIngredients.Add(ingredient);
            if (ingredientsToCollect[ingredient] <= 0)
            {
                ingredientsToCollect.Remove(ingredient);
                CheckCollectedIngredients();
            }

            UpdateIngredientList();
            return true;
        }
        else { return false; }
    }
    public bool TryCollectIngredient(string ingredientName)
    {
        IngredientInfo ingredient = Array.Find(GameManager.ingredients, i => i.name == ingredientName);
        return TryCollectIngredient(ingredient);
    }

    public bool RemoveCollectedIngredient(IngredientInfo ingredient = null)
    {
        if (collectedIngredients.Count <= 0) { return false; }

        if (ingredient == null)
        {
            ingredient = collectedIngredients[Random.Range(0, collectedIngredients.Count)];
        }
        else if (!collectedIngredients.Contains(ingredient))
        {
            print("Ingredient has not been collected");
            return false;
        }

        collectedIngredients.Remove(ingredient);
        if (ingredientsToCollect.ContainsKey(ingredient))
        {
            ingredientsToCollect[ingredient]++;
        }
        else
        {
            ingredientsToCollect.Add(ingredient, 1);
        }
        CheckCollectedIngredients();
        UpdateIngredientList();
        return true;
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

    void UpdateIngredientList()
    {
        print("updating. Count: " + GameManager.Instance.ingredientList.Count);
        // Populate ingredient list
        foreach (var item in GameManager.Instance.ingredientList)
        {

            if (ingredientsToCollect.ContainsKey(item.Key))
            {
                print("key contained");
                ingredientItems[item.Key].ChangeAmount(ingredientsToCollect[item.Key]);
            }
            else
            {
                print("value is 0");
                ingredientItems[item.Key].ChangeAmount(0);
            }
        }
    }
}
