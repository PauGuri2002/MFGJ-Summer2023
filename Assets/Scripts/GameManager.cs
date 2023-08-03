using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Ingredient List Parameters")]
    [SerializeField] private int minUniqueCount = 3;
    [SerializeField] private int maxUniqueCount = 6;
    [SerializeField] private int minTotalAmount = 6;
    [SerializeField] private int maxTotalAmount = 8;

    [NonSerialized] public static IngredientInfo[] ingredients;
    [NonSerialized] public static SeasonInfo[] seasons;

    [NonSerialized] public static GameManager Instance;
    [HideInInspector] public Dictionary<IngredientInfo, int> ingredientList = new();
    [HideInInspector] public string recipeName;
    [HideInInspector] public SeasonInfo gameSeason;

    public static event Action<string, float> OnCompleteMission;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ingredients = (Resources.Load("Ingredients") as Ingredients).ingredients;
        seasons = (Resources.Load("Seasons") as Seasons).seasons;

        gameSeason = GetCurrentSeason();
        if (gameSeason == null)
        {
            gameSeason = seasons[Random.Range(0, seasons.Length)];
        }

        print("Today is " + gameSeason.displayName);
    }

    SeasonInfo GetCurrentSeason()
    {
        return null;
    }

    /* GAME FLOW */

    public void StartMission()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GenerateRecipe();
        SceneManager.LoadScene("ExteriorScene");
    }

    void GenerateRecipe()
    {
        ingredientList.Clear();
        List<IngredientInfo> availableIngredients = new(ingredients);
        int uniqueCount = Random.Range(minUniqueCount, maxUniqueCount + 1);
        int totalAmount = Random.Range(minTotalAmount, maxTotalAmount + 1);

        int[] amountPerIngredient = new int[uniqueCount];
        for (int i = 0, j = 0; i < totalAmount; i++, j++)
        {
            if (j >= amountPerIngredient.Length)
            {
                j = 0;
            }

            amountPerIngredient[j] += 1;
        }

        for (int i = 0; i < uniqueCount; i++)
        {
            int ingredientIndex = Random.Range(0, availableIngredients.Count);
            ingredientList.Add(availableIngredients[ingredientIndex], amountPerIngredient[i]);
            availableIngredients.RemoveAt(ingredientIndex);
        }

        //TODO: Actually program name generation
        recipeName = "Pumpkinized Grapes With Cherry and Pear";
    }

    public void CompleteMission(float missionTime)
    {
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("InteriorScene");

        StartCoroutine(SaveHighScore(missionTime));

        OnCompleteMission?.Invoke(recipeName, missionTime);
    }

    IEnumerator SaveHighScore(float missionTime)
    {
        yield return new WaitForSeconds(Time.deltaTime);

        HighScoreDisplayer highScoreDisplayer = FindObjectOfType<HighScoreDisplayer>();
        if (highScoreDisplayer != null)
        {
            Debug.Log("HSD is not null");
            highScoreDisplayer.AddRecipe(recipeName, missionTime);
        }
    }
}
