using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Ingredient List")]
public class Ingredients : ScriptableObject
{
    public IngredientInfo[] ingredients;
}

[Serializable]
public class IngredientInfo
{
    public string name;
    public Sprite icon;
    public GameObject prefab;
    public Season season;
}
