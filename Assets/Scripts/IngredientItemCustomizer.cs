using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientItemCustomizer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image background;
    [SerializeField] private Image icon;

    public void Customize(string text, Color backgroundColor, Sprite sprite)
    {
        label.text = text;
        background.color = backgroundColor;
        icon.sprite = sprite;
    }
}
