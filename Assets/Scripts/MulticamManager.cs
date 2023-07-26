using System;
using System.Collections;
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

    public void SetFullscreen(Season season, float time = 0f, float delay = 0f, bool alsoFullscreenOthers = false)
    {
        if (isAnimating)
        {
            Debug.LogWarning("Already animating");
            return;
        }

        cameras[season].SetAsLastSibling();

        if (!alsoFullscreenOthers)
        {
            SetFullscreenSingle(cameras[season], time, delay);
        }
        else
        {
            foreach (RectTransform cam in cameras.Values)
            {
                SetFullscreenSingle(cam, time, delay);
            }
        }
    }

    void SetFullscreenSingle(RectTransform cam, float time = 0f, float delay = 0f)
    {
        if (time <= 0)
        {
            cam.anchoredPosition = Vector2.zero;
            cam.sizeDelta = canvasSize;
            isAnimating = false;
        }
        else
        {
            StartCoroutine(SpatialInterpolation(cam, cam.anchoredPosition, Vector2.zero, cam.sizeDelta, canvasSize, time, delay));
        }
    }

    public void SetGrid(float time = 0f, float delay = 0f, Season[] newOrder = null)
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

            if (time <= 0)
            {
                cam.anchoredPosition = newPosition;
                cam.sizeDelta = canvasSize / 2;
            }
            else
            {
                StartCoroutine(SpatialInterpolation(cam, cam.anchoredPosition, newPosition, cam.sizeDelta, canvasSize / 2, time, delay));
            }
        }
    }

    IEnumerator SpatialInterpolation(RectTransform target, Vector2 fromPosition, Vector2 toPosition, Vector2 fromSize, Vector2 toSize, float time, float delay)
    {
        isAnimating = true;

        if (delay >= 0)
        {
            yield return new WaitForSeconds(delay);
        }

        float progress = 0f;
        do
        {
            target.anchoredPosition = Vector2.Lerp(fromPosition, toPosition, progress);
            target.sizeDelta = Vector2.Lerp(fromSize, toSize, progress);

            progress += 1 / time * Time.deltaTime;
            yield return 0;
        } while (progress < 1);

        target.anchoredPosition = toPosition;
        target.sizeDelta = toSize;
        isAnimating = false;
    }

    public void SetTintAll(Color from, Color to, float time, float delay)
    {
        if (isTintAnimating)
        {
            Debug.LogWarning("Opacity already animating");
            return;
        }

        foreach (RectTransform cam in cameras.Values)
        {
            if (cam.TryGetComponent<RawImage>(out var camImage))
            {
                StartCoroutine(TintInterpolation(camImage, from, to, time, delay));
            }
        }

    }

    IEnumerator TintInterpolation(RawImage target, Color from, Color to, float time, float delay)
    {
        isTintAnimating = true;

        if (delay >= 0)
        {
            yield return new WaitForSeconds(delay);
        }

        float progress = 0f;
        do
        {
            target.color = Color.Lerp(from, to, progress);

            progress += 1 / time * Time.deltaTime;
            yield return 0;
        } while (progress < 1);

        target.color = to;
        isTintAnimating = false;
    }

    [Serializable]
    class IdentifiedCamera
    {
        public RectTransform camera;
        public Season season;
    }
}

