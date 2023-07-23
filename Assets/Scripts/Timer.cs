using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [HideInInspector] public float elapsedTime = 0;
    private bool isActive = false;

    void Start()
    {
        label.text = "00:00";
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            elapsedTime += Time.deltaTime;
            UpdateLabel();
        }
    }

    public void StartTimer()
    {
        isActive = true;
    }

    public void StopTimer()
    {
        isActive = false;
    }

    public void ResetTimer()
    {
        if (isActive)
        {
            StopTimer();
        }
        elapsedTime = 0f;
        UpdateLabel();
    }

    public void SetTime(float time)
    {
        elapsedTime = time;
    }

    void UpdateLabel()
    {
        string minutes = Mathf.Floor(elapsedTime / 60).ToString();
        string seconds = Mathf.Floor(elapsedTime % 60).ToString();
        string time = (minutes.Length == 1 ? "0" : "") + minutes + ":" + (seconds.Length == 1 ? "0" : "") + seconds;
        label.text = time;
    }
}
