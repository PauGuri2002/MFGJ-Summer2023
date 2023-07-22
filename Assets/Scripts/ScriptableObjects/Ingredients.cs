using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Ingredient List")]
public class Ingredients : ScriptableObject
{
    public Ingredient[] ingredients;
}

[Serializable]
public class Ingredient
{
    public string name;
    public Sprite icon;
    public GameObject prefab;
    public Season season;
}
