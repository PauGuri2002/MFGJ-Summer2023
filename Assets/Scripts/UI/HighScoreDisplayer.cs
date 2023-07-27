using System;
using System.Collections.Generic;
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
        foreach (KeyValuePair<string, float> recipe in saveData.completedRecipes)
        {
            string minutes = Mathf.Floor(recipe.Value / 60).ToString();
            string seconds = Mathf.Floor(recipe.Value % 60).ToString();
            text += recipe.Key + " - " + (minutes.Length == 1 ? "0" : "") + minutes + ":" + (seconds.Length == 1 ? "0" : "") + seconds + "\n";
        }
        textField.text = text;
        print(text);
    }

    public void AddRecipe(string name, float time)
    {
        saveData.completedRecipes.Add(name, time);
        SaveData();
    }

    public void RemoveRecipe(string name)
    {
        saveData.completedRecipes.Remove(name);
        SaveData();
    }

    public void ClearData()
    {
        saveData.completedRecipes.Clear();
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