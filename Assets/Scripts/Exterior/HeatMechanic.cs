using System;
using UnityEngine;
using UnityEngine.UI;

public class HeatMechanic : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float startDelay = 5f;
    [SerializeField] private float heatingTime = 10;
    [SerializeField] private float coolingTime = 1;
    [SerializeField] private Image overlay;
    [SerializeField] private MulticamManager multicamManager;
    [SerializeField] private GameObject ingredientListObject;

    [Header("Shader Parameters")]
    [SerializeField] private Material shader;
    [SerializeField] private float startSize = 30;
    [SerializeField] private float maxSize = 50;
    [SerializeField] private Vector2 startSpeed = new Vector2(0.2f, 0.2f);
    [SerializeField] private Vector2 maxSpeed = new Vector2(0.3f, 0.3f);
    [SerializeField] private float startAmount = 0.005f;
    [SerializeField] private float maxAmount = 0.1f;

    public enum Status { Idle, Heating, Cooling, Burning, Burned }
    [NonSerialized] public Status currentStatus = Status.Idle;
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
            Burn();

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
        if (currentStatus == Status.Burning) { return; }

        if (currentStatus == Status.Burned)
        {
            multicamManager.SetGrid(1, 0);

            if (ingredientListObject != null)
            {
                ingredientListObject.LeanScale(Vector3.one, 1f).setEaseInOutCubic();
                ingredientListObject.LeanRotateZ(0, 1f).setEaseInOutCubic();
            }
        }

        currentStatus = Status.Cooling;
    }

    void Burn()
    {
        currentStatus = Status.Burned;
        multicamManager.Shake(Season.Summer, 0.1f, 10f);
        multicamManager.SetFullscreen(Season.Summer, 0.8f, 0.2f);

        if (ingredientListObject != null)
        {
            ingredientListObject.LeanScale(Vector3.zero, 0).setEaseInOutCubic();
            ingredientListObject.LeanRotateZ(45, 0).setEaseInOutCubic();
        }
    }

    private void OnApplicationQuit()
    {
        ResetShader();
    }

    private void OnDestroy()
    {
        ResetShader();
    }

    void ResetShader()
    {
        shader.SetFloat(amountProperty, startAmount);
        //shader.SetVector(speedProperty, startSpeed);
        shader.SetFloat(sizeProperty, startSize);
    }
}
