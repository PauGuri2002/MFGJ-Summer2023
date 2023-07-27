using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MulticamManager : MonoBehaviour
{
    [SerializeField] private IdentifiedCamera[] identifiedCameras;
    [SerializeField] private RectTransform parentCanvas;

    private Dictionary<Season, RectTransform> cameras = new();
    private Season[] cameraOrder;
    private Vector2 canvasSize;
    private bool isAnimating = false;
    private bool isTintAnimating = false;

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
        if (isAnimating)
        {
            Debug.LogWarning("Already animating");
            return;
        }

        cameras[firstSeason].SetAsLastSibling();

        foreach (RectTransform cam in cameras.Values)
        {
            SetFullscreenSingle(cam, time, delay, easing);
        }
    }

    public void SetFullscreen(Season season, float time = 0f, float delay = 0f, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        SetFullscreenSingle(cameras[season], time, delay, easing);
    }

    void SetFullscreenSingle(RectTransform cam, float time = 0f, float delay = 0f, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        LeanTween.move(cam, Vector2.zero, time).setDelay(delay).setEase(easing);
        LeanTween.size(cam, canvasSize, time).setDelay(delay).setEase(easing);
    }

    public void SetGrid(float time = 0f, float delay = 0f, Season[] newOrder = null, LeanTweenType easing = LeanTweenType.easeInOutCubic)
    {
        if (isAnimating)
        {
            Debug.LogWarning("Already animating");
            return;
        }

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
        if (isTintAnimating)
        {
            Debug.LogWarning("Opacity already animating");
            return;
        }

        foreach (RectTransform cam in cameras.Values)
        {
            if (cam.TryGetComponent<RawImage>(out var image))
            {
                LeanTween.value(image.gameObject, (c) => image.color = c, from, to, time).setDelay(delay).setEase(easing);
            }

        }

    }

    [Serializable]
    class IdentifiedCamera
    {
        public RectTransform camera;
        public Season season;
    }
}

