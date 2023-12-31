using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientItem : MonoBehaviour
{
    [HideInInspector] public string ingredientName;
    private int amount;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite[] backgroundSprites;

    public void Customize(string name, int _amount, Color backgroundColor, Sprite sprite)
    {
        ingredientName = name;
        amount = _amount;
        background.color = backgroundColor;
        background.sprite = backgroundSprites[Random.Range(0, backgroundSprites.Length)];
        icon.sprite = sprite;
        UpdateText();
    }

    public void ChangeAmount(int _amount)
    {
        amount = _amount;
        UpdateText();
    }

    public void UpdateText()
    {
        label.text = (amount <= 0) ? "<s>" + ingredientName + "</s>" : ingredientName + " x" + amount.ToString();
    }
}
