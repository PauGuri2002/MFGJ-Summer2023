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

    private Dictionary<Season, RectTransform> cameras = new();
    [HideInInspector] public Season[] cameraOrder { get; private set; }
    private Vector2 canvasSize;
    private bool isShaking = false;

    void Start()
    {
        foreach (IdentifiedCamera cam in identifiedCameras)
        {
            cameras.Add(cam.season, cam.camera);
        }

        cameraOrder = cameras.Keys.ToArray();
        canvasSize = new Vector2(parentCanvas.rect.width, parentCanvas.rect.height);
    }

    public void SetFullscreenAll(Season firstSeason, float time = 0f, float delay = 0f, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        cameras[firstSeason].SetAsLastSibling();

        foreach (RectTransform cam in cameras.Values)
        {
            SetFullscreenSingle(cam, time, delay, easing);
        }
    }

    public void SetFullscreen(Season season, float time = 0f, float delay = 0f, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        cameras[season].SetAsLastSibling();
        SetFullscreenSingle(cameras[season], time, delay, easing);
    }

    void SetFullscreenSingle(RectTransform cam, float time = 0f, float delay = 0f, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        LeanTween.move(cam, Vector2.zero, time).setDelay(delay).setEase(easing);
        LeanTween.size(cam, canvasSize, time).setDelay(delay).setEase(easing);
    }

    public void SetGrid(float time = 0f, float delay = 0f, Season[] newOrder = null, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        if (newOrder != null)
        {
            cameraOrder = newOrder;
        }

        for (int i = 0; i < cameraOrder.Length; i++)
        {
            RectTransform cam = cameras[cameraOrder[i]];

            Vector2 newPosition = new(i % 2 * canvasSize.x / 2, -i / 2 * canvasSize.y / 2);

            LeanTween.move(cam, newPosition, time).setDelay(delay).setEase(easing);
            LeanTween.size(cam, canvasSize / 2, time).setDelay(delay).setEase(easing);
        }
    }

    public void SetTintAll(Color from, Color to, float time = 0f, float delay = 0f, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        foreach (RectTransform cam in cameras.Values)
        {
            if (cam.TryGetComponent<RawImage>(out var image))
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
        foreach (RectTransform cam in cameras.Values)
        {
            StartCoroutine(ShakeSingle(cam, duration, intensity, frequency));
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
        StartCoroutine(ShakeSingle(cameras[season], duration, intensity, frequency));
    }

    IEnumerator ShakeSingle(RectTransform cam, float duration, float intensity, float frequency)
    {
        float timer = 0;
        Vector3 originalPosition = cam.localPosition;

        while (timer < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            cam.localPosition = originalPosition + new Vector3(x, y, 0f);

            timer += Time.deltaTime;
            yield return new WaitForSeconds(1 / frequency);
        }

        cam.localPosition = originalPosition;
        isShaking = false;
    }

    [Serializable]
    class IdentifiedCamera
    {
        public RectTransform camera;
        public Season season;
    }
}

