using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public List<string> recipeNames = new();
    public List<float> recipeTimes = new();
    public SerializableDictionary<int, int> completedRecipeIds = new();

    public void Add(string name, float time)
    {
        recipeNames.Add(name);
        recipeTimes.Add(time);
        completedRecipeIds.Add(recipeNames.Count - 1, recipeTimes.Count - 1);
    }

    public void Clear()
    {
        recipeNames.Clear();
        recipeTimes.Clear();
        completedRecipeIds.Clear();
    }
}