using System;
using TMPro;
using UnityEngine;

public class HighScoreDisplayer : MonoBehaviour
{
    [Header("Save Data")]
    [SerializeField] private string fileName;
    [SerializeField] private bool encryptSaveData = true;
    private SaveDataManager saveDataManager;
    [NonSerialized] public SaveData saveData;

    [Header("Display Data")]
    [SerializeField] private TextMeshProUGUI textField;

    void Start()
    {
        // Load Data
        saveDataManager = new SaveDataManager(Application.persistentDataPath, fileName, encryptSaveData);
        LoadData();
    }

    public void DisplayData()
    {
        string text = "";
        foreach (int recipeKey in saveData.GetSortedKeys())
        {
            string recipeName = saveData.recipeNames[recipeKey];
            float recipeTime = saveData.recipeTimes[saveData.completedRecipeIds[recipeKey]];

            string minutes = Mathf.Floor(recipeTime / 60).ToString();
            string seconds = Mathf.Floor(recipeTime % 60).ToString();
            text += recipeName + " - <font=\"LilitaOne-Regular SDF\">" + (minutes.Length == 1 ? "0" : "") + minutes + ":" + (seconds.Length == 1 ? "0" : "") + seconds + "</font>\n";
        }
        textField.text = text;
    }

    public void AddRecipe(string name, float time)
    {
        saveData.Add(name, time);
        SaveData();
    }

    public void ClearData()
    {
        saveData.Clear();
        SaveData();
    }

    void SaveData()
    {
        saveDataManager.Save(saveData);
        print("Data saved");
        DisplayData();
    }

    void LoadData()
    {
        saveData = saveDataManager.Load();
        if (saveData == null)
        {
            NewData();
        }
        print("Data loaded");
        DisplayData();
    }

    void NewData()
    {
        print("New data created");
        saveData = new SaveData();
    }
}
