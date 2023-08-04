using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MulticamManager : MonoBehaviour
{
    [SerializeField] private IdentifiedCamera[] identifiedCameras;
    [SerializeField] private RectTransform parentCanvas;
    [SerializeField] private AudioSource audioSource;

    private Dictionary<Season, RectTransform> panels = new();

    [HideInInspector] public Season[] CameraOrder { get; private set; }
    private Vector2 canvasSize;
    private bool isShaking = false;

    void Start()
    {
        foreach (IdentifiedCamera cam in identifiedCameras)
        {
            panels.Add(cam.season, cam.panel);
        }

        CameraOrder = panels.Keys.ToArray();
        canvasSize = new Vector2(parentCanvas.rect.width, parentCanvas.rect.height);
    }

    public void SetFullscreenAll(Season firstSeason, float time = 0f, float delay = 0f, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        panels[firstSeason].SetAsLastSibling();
        ResizeRenderTexture(panels[firstSeason], canvasSize);

        foreach (RectTransform panel in panels.Values)
        {
            SetFullscreenSingle(panel, time, delay, easing);
        }
    }

    public void SetFullscreen(Season season, float time = 0f, float delay = 0f, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        panels[season].SetAsLastSibling();
        ResizeRenderTexture(panels[season], canvasSize);
        SetFullscreenSingle(panels[season], time, delay, easing);
    }

    void SetFullscreenSingle(RectTransform panel, float time = 0f, float delay = 0f, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        if (time >= 0.5f)
        {
            Invoke(nameof(PlayWhoosh), delay);
        }
        LeanTween.move(panel, Vector2.zero, time).setDelay(delay).setEase(easing);
        LeanTween.size(panel, canvasSize, time).setDelay(delay).setEase(easing);
    }

    public void SetGrid(float time = 0f, float delay = 0f, Season[] newOrder = null, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        if (newOrder != null)
        {
            CameraOrder = newOrder;
        }

        if (time >= 0.5f)
        {
            Invoke(nameof(PlayWhoosh), delay);
        }

        for (int i = 0; i < CameraOrder.Length; i++)
        {
            RectTransform panel = panels[CameraOrder[i]];

            Vector2 newPosition = new(i % 2 * canvasSize.x / 2, -i / 2 * canvasSize.y / 2);

            LeanTween.move(panel, newPosition, time).setDelay(delay).setEase(easing);
            LeanTween.size(panel, canvasSize / 2, time).setDelay(delay).setEase(easing).setOnComplete(() => ResizeRenderTexture(panel, canvasSize / 2));
        }
    }

    public void SetTintAll(Color from, Color to, float time = 0f, float delay = 0f, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        foreach (RectTransform panel in panels.Values)
        {
            if (panel.TryGetComponent<RawImage>(out var image))
            {
                LeanTween.value(image.gameObject, (c) => image.color = c, from, to, time).setDelay(delay).setEase(easing);
            }
        }
    }

    public void ShakeAll(float duration, float intensity, float frequency = 100f)
    {
        if (isShaking)
        {
            Debug.LogWarning("Already shaking");
            return;
        }

        isShaking = true;
        foreach (RectTransform panel in panels.Values)
        {
            StartCoroutine(ShakeSingle(panel, duration, intensity, frequency));
        }
    }

    public void Shake(Season season, float duration, float intensity, float frequency = 100f)
    {
        if (isShaking)
        {
            Debug.LogWarning("Already shaking");
            return;
        }

        isShaking = true;
        StartCoroutine(ShakeSingle(panels[season], duration, intensity, frequency));
    }

    IEnumerator ShakeSingle(RectTransform panel, float duration, float intensity, float frequency)
    {
        float timer = 0;
        Vector3 originalPosition = panel.localPosition;

        while (timer < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            panel.localPosition = originalPosition + new Vector3(x, y, 0f);

            timer += Time.deltaTime;
            yield return new WaitForSeconds(1 / frequency);
        }

        panel.localPosition = originalPosition;
        isShaking = false;
    }

    void ResizeRenderTexture(RectTransform panel, Vector2 newSize)
    {
        if (panel.TryGetComponent<RawImage>(out var image))
        {
            if (image.texture is RenderTexture rt)
            {
                rt.Release();
                rt.width = Mathf.RoundToInt(newSize.x);
                rt.height = Mathf.RoundToInt(newSize.y);
                rt.Create();
            }
        }
    }

    void PlayWhoosh()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.Play();
        }
    }

    [Serializable]
    class IdentifiedCamera
    {
        public RectTransform panel;
        public Season season;
    }
}

