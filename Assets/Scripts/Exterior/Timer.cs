using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [HideInInspector] public float elapsedTime = 0;
    private bool isActive = false;
    private RectTransform timerRect;

    void Start()
    {
        TryGetComponent(out timerRect);
        if (timerRect != null)
        {
            timerRect.LeanMove(Vector3.up * 100, 0);
        }
        label.text = "00:00";

        ExteriorManager.onPhaseChange += AnimateIn;
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

    void AnimateIn(ExteriorManager.GamePhase phase)
    {
        if (timerRect != null && !isActive && phase == ExteriorManager.GamePhase.Search)
        {
            LeanTween.move(timerRect, Vector3.zero, 1f).setEaseInOutCubic();
            StartTimer();
            ExteriorManager.onPhaseChange -= AnimateIn;
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
