using TMPro;
using UnityEngine;

public class ExteriorManager : MonoBehaviour
{
    [SerializeField] private Vector2 spawnerSize;

    [SerializeField] private TextMeshProUGUI ingredientListText;

    public void Start()
    {
        if (GameManager.Instance == null || GameManager.Instance.ingredientList == null) { return; }

        foreach (var item in GameManager.Instance.ingredientList)
        {
            ingredientListText.text += item.Key.name + " x" + item.Value.ToString() + "\n";

            for (int i = 0; i < item.Value; i++)
            {
                Vector3 spawnPosition;
                do
                {
                    spawnPosition = new Vector3(Random.Range(0, spawnerSize.x) - spawnerSize.x / 2, 0.5f, Random.Range(0, spawnerSize.y) - spawnerSize.y / 2);
                } while (Physics.CheckBox(spawnPosition, new Vector3(5, 0.1f, 5)));

                GameObject instance = Instantiate(item.Key.prefab, spawnPosition, Quaternion.identity);
                instance.layer = (int)item.Key.season;
                instance.GetComponentInChildren<TextMeshProUGUI>().text = item.Key.name;
            }
        }
    }
}
