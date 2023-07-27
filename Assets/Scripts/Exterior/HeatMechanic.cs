using System.Collections;
using UnityEngine;

public class HeatMechanic : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float startDelay = 5f;
    [SerializeField] private float heatingTime = 10;

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
            timer += Time.deltaTime;
            print("Heating: " + timer);
            yield return 0;
        }

        print("OVERHEATING!!!");
        coroutine = null;
    }

    IEnumerator DecreaseHeat()
    {
        float currentAmount = shader.GetFloat(amountProperty);
        Vector2 currentSpeed = shader.GetVector(speedProperty);
        float currentSize = shader.GetFloat(sizeProperty);

        float timer = 0f;
        while (timer < 1f)
        {
            shader.SetFloat(amountProperty, Mathf.Lerp(currentAmount, startAmount, timer));
            shader.SetVector(speedProperty, Vector2.Lerp(currentSpeed, startSpeed, timer));
            shader.SetFloat(sizeProperty, Mathf.Lerp(currentSize, startSize, timer));
            timer += Time.deltaTime;
            print("Deheating: " + timer);
            yield return 0;
        }

        shader.SetFloat(amountProperty, startAmount);
        shader.SetVector(speedProperty, startSpeed);
        shader.SetFloat(sizeProperty, startSize);

        coroutine = StartCoroutine(IncreaseHeat());
    }

    private void OnApplicationQuit()
    {
        shader.SetFloat(amountProperty, startAmount);
        shader.SetVector(speedProperty, startSpeed);
        shader.SetFloat(sizeProperty, startSize);
    }
}
