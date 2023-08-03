using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExteriorManager : MonoBehaviour
{
    [SerializeField] private MulticamManager multicamManager;
    [SerializeField] private GameObject[] seasonContainers;

    [Header("Spawner Properties")]
    [SerializeField] private Vector2 spawnerSize;
    [SerializeField] private float spawnerHeight = 0.5f;
    [SerializeField] private float spawnPadding = 5f;

    [Header("Spawned Objects")]
    [SerializeField] private int minExtraIngredients = 6, maxExtraIngredients = 10;
    [SerializeField] private int minBeeHives = 10, maxBeeHives = 20;
    [SerializeField] private GameObject beeHivePrefab;

    [Header("UI")]
    [SerializeField] private GameObject ingredientListObject;
    [SerializeField] private GameObject ingredientItemPrefab;

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
        // Temporally disable Ingredient List
        ingredientListObject.LeanScale(Vector3.zero, 0);
        ingredientListObject.LeanRotateZ(45, 0);

        // Multicam
        Color semitransparent = new Color(1, 1, 1, 0.5f);
        multicamManager.SetFullscreenAll((GameManager.Instance != null) ? GameManager.Instance.gameSeason.seasonName : Season.Spring, 0, 0);
        multicamManager.SetTintAll(Color.white, semitransparent, 1f, 2f);
        multicamManager.SetTintAll(semitransparent, Color.white, 2f, 3f);
        multicamManager.SetGrid(3f, 2f);

        // Spawn Bee Hives
        int beeHiveCount = Random.Range(minBeeHives, maxBeeHives + 1);
        for (int i = 0; i < beeHiveCount; i++)
        {
            SpawnObject(beeHivePrefab);
        }

        // Load ingredient list
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

        Invoke(nameof(StartSearch), 5);
    }

    void StartSearch()
    {
        // animate in Ingredient List
        LeanTween.scale(ingredientListObject, Vector3.one, 1f).setEaseInOutCubic();
        LeanTween.rotateZ(ingredientListObject, 0, 1f).setEaseInOutCubic();

        currentPhase = GamePhase.Search;
        onPhaseChange?.Invoke(currentPhase);
    }

    void SpawnIngredient(IngredientInfo ingredient = null)
    {
        ingredient ??= GameManager.ingredients[Random.Range(0, GameManager.ingredients.Length)];

        GameObject instance = SpawnObject(ingredient.prefab);
        instance.layer = (int)ingredient.season;

        // PROVISIONAL!!!! Fins que cada ingredient tingui el seu model
        //instance.GetComponentInChildren<TextMeshProUGUI>().text = ingredient.name;
        //instance.GetComponentInChildren<TextMeshProUGUI>().gameObject.layer = (int)ingredient.season;
        //instance.GetComponent<Ingredient>().ingredientName = ingredient.name;
    }

    GameObject SpawnObject(GameObject prefab)
    {
        Vector3 spawnPosition;

        bool positionFound;
        Collider[] colliders = new Collider[10];
        do
        {
            spawnPosition = new Vector3(Random.Range(0, spawnerSize.x) - spawnerSize.x / 2, spawnerHeight, Random.Range(0, spawnerSize.y) - spawnerSize.y / 2);
            positionFound = true;

            int colliderCount = Physics.OverlapSphereNonAlloc(spawnPosition, spawnPadding, colliders);
            for (int i = 0; i < colliderCount; i++)
            {
                if (colliders[i].CompareTag("Impenetrable") || colliders[i].CompareTag("Player"))
                {
                    positionFound = false;
                    break;
                }
            }
        } while (!positionFound);

        GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity);
        GameObject seasonParent = Array.Find(seasonContainers, c => c.layer == instance.layer);
        if (seasonParent != null)
        {
            instance.transform.SetParent(seasonParent.transform, true);
        }

        return instance;
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

    public IngredientInfo RemoveCollectedIngredient(IngredientInfo ingredient = null)
    {
        if (collectedIngredients.Count <= 0) { return null; }

        if (ingredient == null)
        {
            ingredient = collectedIngredients[Random.Range(0, collectedIngredients.Count)];
        }
        else if (!collectedIngredients.Contains(ingredient))
        {
            print("Ingredient has not been collected");
            return null;
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

        // spawn the ingredient back again
        SpawnIngredient(ingredient);

        return ingredient;
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
        // Populate ingredient list
        foreach (var item in GameManager.Instance.ingredientList)
        {

            if (ingredientsToCollect.ContainsKey(item.Key))
            {
                ingredientItems[item.Key].ChangeAmount(ingredientsToCollect[item.Key]);
            }
            else
            {
                ingredientItems[item.Key].ChangeAmount(0);
            }
        }
    }
}
