using System.Collections;
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

    private Coroutine coroutine;
    int amountProperty, speedProperty, sizeProperty;

    void Start()
    {
        amountProperty = Shader.PropertyToID("_Distortion_Amount");
        speedProperty = Shader.PropertyToID("_Distortion_Speed");
        sizeProperty = Shader.PropertyToID("_Distortion_Scale");
        overlay.fillAmount = 0;

        if (coroutine == null)
        {
            coroutine = StartCoroutine(IncreaseHeat());
        }
    }

    public void CoolDown()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        StartCoroutine(DecreaseHeat());
    }

    IEnumerator IncreaseHeat()
    {
        yield return new WaitForSeconds(startDelay);

        float timer = 0f;
        while (timer < heatingTime)
        {
            shader.SetFloat(amountProperty, Mathf.Lerp(startAmount, maxAmount, timer / heatingTime));
            shader.SetVector(speedProperty, Vector2.Lerp(startSpeed, maxSpeed, timer / heatingTime));
            shader.SetFloat(sizeProperty, Mathf.Lerp(startSize, maxSize, timer / heatingTime));
            overlay.fillAmount = timer / heatingTime;

            timer += Time.deltaTime;
            print("Heating: " + timer);
            yield return 0;
        }

        overlay.fillAmount = 1;
        print("OVERHEATING!!!");
        coroutine = null;
    }

    IEnumerator DecreaseHeat()
    {
        float currentAmount = shader.GetFloat(amountProperty);
        Vector2 currentSpeed = shader.GetVector(speedProperty);
        float currentSize = shader.GetFloat(sizeProperty);

        float timer = 0f;
        while (timer < coolingTime)
        {
            shader.SetFloat(amountProperty, Mathf.Lerp(currentAmount, startAmount, timer / coolingTime));
            shader.SetVector(speedProperty, Vector2.Lerp(currentSpeed, startSpeed, timer / coolingTime));
            shader.SetFloat(sizeProperty, Mathf.Lerp(currentSize, startSize, timer / coolingTime));
            overlay.fillAmount = 1 - timer / coolingTime;

            timer += Time.deltaTime;
            print("Deheating: " + timer);
            yield return 0;
        }

        shader.SetFloat(amountProperty, startAmount);
        shader.SetVector(speedProperty, startSpeed);
        shader.SetFloat(sizeProperty, startSize);
        overlay.fillAmount = 0;

        coroutine = StartCoroutine(IncreaseHeat());
    }

    private void OnApplicationQuit()
    {
        shader.SetFloat(amountProperty, startAmount);
        shader.SetVector(speedProperty, startSpeed);
        shader.SetFloat(sizeProperty, startSize);
    }
}
