using UnityEngine;
using UnityEngine.UI;

public class HeatMechanic : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float startDelay = 5f;
    [SerializeField] private float heatingTime = 10;
    [SerializeField] private float coolingTime = 1;
    [SerializeField] private Image overlay;

    [Header("Shader Parameters")]
    [SerializeField] private Material shader;
    [SerializeField] private float startSize = 30;
    [SerializeField] private float maxSize = 50;
    [SerializeField] private Vector2 startSpeed = new Vector2(0.2f, 0.2f);
    [SerializeField] private Vector2 maxSpeed = new Vector2(0.3f, 0.3f);
    [SerializeField] private float startAmount = 0.005f;
    [SerializeField] private float maxAmount = 0.1f;

    enum Status { Idle, Heating, Cooling, Burning }
    private Status currentStatus = Status.Idle;
    private float heatPercent = 0;

    //private Coroutine coroutine;
    int amountProperty, speedProperty, sizeProperty;

    void Start()
    {
        amountProperty = Shader.PropertyToID("_Distortion_Amount");
        speedProperty = Shader.PropertyToID("_Distortion_Speed");
        sizeProperty = Shader.PropertyToID("_Distortion_Scale");
        overlay.fillAmount = 0;

        ExteriorManager.onPhaseChange += TryEnable;
    }

    void TryEnable(ExteriorManager.GamePhase phase)
    {
        if (currentStatus == Status.Idle && phase == ExteriorManager.GamePhase.Search)
        {
            Invoke(nameof(HeatUp), startDelay);
            ExteriorManager.onPhaseChange -= TryEnable;
        }
    }

    private void Update()
    {
        if (currentStatus == Status.Heating)
        {
            heatPercent += 1 / heatingTime * Time.deltaTime;
            if (heatPercent >= 1)
            {
                heatPercent = 1;
                currentStatus = Status.Burning;
            }
        }
        else if (currentStatus == Status.Cooling)
        {
            heatPercent -= 1 / coolingTime * Time.deltaTime;
            if (heatPercent <= 0)
            {
                heatPercent = 0;
                currentStatus = Status.Idle;
            }
        }
        else if (currentStatus == Status.Burning)
        {
            print("I'M ON FIRE");
        }

        shader.SetFloat(amountProperty, Mathf.Lerp(startAmount, maxAmount, heatPercent));
        //shader.SetVector(speedProperty, Vector2.Lerp(startSpeed, maxSpeed, timer / heatingTime));
        shader.SetFloat(sizeProperty, Mathf.Lerp(startSize, maxSize, heatPercent));
        overlay.fillAmount = heatPercent;
    }

    public void HeatUp()
    {
        currentStatus = Status.Heating;
    }

    public void CoolDown()
    {
        if (currentStatus != Status.Burning)
        {
            currentStatus = Status.Cooling;
        }
    }

    private void OnApplicationQuit()
    {
        shader.SetFloat(amountProperty, startAmount);
        //shader.SetVector(speedProperty, startSpeed);
        shader.SetFloat(sizeProperty, startSize);
    }
}
