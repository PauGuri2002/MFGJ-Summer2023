using System;
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

    private Ingredient[] ingredients;

    [NonSerialized] public static GameManager Instance;
    [HideInInspector] public Dictionary<Ingredient, int> ingredientList = new();

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ingredients = (Resources.Load("Ingredients") as Ingredients).ingredients;
    }

    public void StartGame()
    {
        GenerateRecipe();
        SceneManager.LoadScene("ExteriorScene");
    }

    void GenerateRecipe()
    {
        ingredientList.Clear();
        List<Ingredient> availableIngredients = new(ingredients);
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
    }
}
