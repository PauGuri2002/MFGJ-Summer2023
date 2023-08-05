using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RecipeNameGenerator : MonoBehaviour
{
    [SerializeField] private NameVariationCode[] nameVariationCodes;
    [SerializeField] private NameStructureGroup[] nameStructureGroups;
    private Dictionary<string, NameVariation> variationCodesDict = new();

    private void Start()
    {
        foreach (NameVariationCode variationCode in nameVariationCodes)
        {
            variationCodesDict.Add(variationCode.code, variationCode.nameVariation);
        }
    }

    public string GenerateName(IngredientInfo[] ingredients)
    {
        string recipeName = "Delicious Recipe";

        if (ingredients.Length > nameStructureGroups.Length) { Debug.LogWarning("No structure group exists for " + ingredients.Length + " ingredients."); return recipeName; }

        string[] nameStructures = nameStructureGroups[ingredients.Length - 1].nameStructures;

        if (nameStructures.Length <= 0) { Debug.LogWarning("There are no structures for " + ingredients.Length + " ingredients"); return recipeName; }

        string structure = nameStructures[Random.Range(0, nameStructures.Length)];
        string[] splitStructure = structure.Split('%');

        int codeCount = 0;
        for (int i = 0; i < splitStructure.Length; i++)
        {
            if (!variationCodesDict.ContainsKey(splitStructure[i])) { continue; }
            if (codeCount >= ingredients.Length) { Debug.LogWarning("There are more codes than ingredients"); return recipeName; }

            Color color = Array.Find(GameManager.seasons, s => s.seasonName == ingredients[codeCount].season).color;
            string hexColor = ColorUtility.ToHtmlStringRGB(color);

            IngredientNameVariation[] ingredientNameVariations = ingredients[codeCount].nameVariations;
            IngredientNameVariation nameVariation = Array.Find(ingredientNameVariations, n => n.variationType == variationCodesDict[splitStructure[i]]);
            if (nameVariation != null)
            {
                splitStructure[i] = "<color=#" + hexColor + ">" + nameVariation.name + "</color>";
            }
            else
            {
                Debug.LogWarning("Variation " + splitStructure[i] + " could not be found. Using default: " + ingredients[codeCount].name);
                splitStructure[i] = "<color=#" + hexColor + ">" + ingredients[codeCount].name + "</color>";
            }

            codeCount++;
        }
        recipeName = string.Join("", splitStructure);

        return recipeName;
    }
}

public enum NameVariation
{
    Normal,
    Singular,
    Plural,
    Ized,
}

[Serializable]
public class NameVariationCode
{
    public NameVariation nameVariation;
    public string code;
}

[Serializable]
public class NameStructureGroup
{
    [TextArea] public string[] nameStructures;
}