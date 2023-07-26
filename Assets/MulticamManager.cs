using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MulticamManager : MonoBehaviour
{
    [SerializeField] private IdentifiedCamera[] identifiedCameras;
    [SerializeField] private RectTransform parentCanvas;

    private Dictionary<Season, RectTransform> cameras = new();
    private Season[] cameraOrder;
    private Vector2 canvasSize;
    private bool isAnimating = false;

    void Start()
    {
        foreach (IdentifiedCamera cam in identifiedCameras)
        {
            cameras.Add(cam.season, cam.camera);
        }

        cameraOrder = cameras.Keys.ToArray();
        canvasSize = new Vector2(parentCanvas.rect.width, parentCanvas.rect.height);
    }

    public void SetFullscreen(Season season, float time = 0f)
    {
        if (isAnimating)
        {
            Debug.LogWarning("Already animating");
            return;
        }

        RectTransform cam = cameras[season];
        cam.SetAsLastSibling();
        if (time <= 0)
        {
            print("set fullscreen instantly");
            cam.anchoredPosition = Vector2.zero;
            cam.sizeDelta = canvasSize;
            isAnimating = false;
        }
        else
        {
            StartCoroutine(Interpolation(cam, new Rect(cam.rect), new Rect(0, 0, canvasSize.x, canvasSize.y), time));
        }
    }

    public void SetGrid(float time = 0f, Season[] newOrder = null)
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

            float x = i % 2 * canvasSize.x / 2;
            float y = i / 2 * canvasSize.y / 2;

            if (time <= 0)
            {
                cam.anchoredPosition = new Vector2(x, y);
                cam.sizeDelta = canvasSize / 2;
            }
            else
            {
                StartCoroutine(Interpolation(cam, new Rect(cam.rect), new Rect(x, y, canvasSize.x / 2, canvasSize.y / 2), time));
            }
        }
    }

    IEnumerator Interpolation(RectTransform target, Rect from, Rect to, float time)
    {
        isAnimating = true;

        float progress = 0f;
        do
        {
            print("interpolating: " + progress);
            target.anchoredPosition = new Vector2(Mathf.Lerp(from.x, to.x, progress), Mathf.Lerp(from.y, to.y, progress));
            target.sizeDelta = new Vector2(Mathf.Lerp(from.width, to.width, progress), Mathf.Lerp(from.height, to.height, progress));

            progress += 1 / time * Time.deltaTime;
            yield return 0;
        } while (progress < 1);

        isAnimating = false;
    }

    [Serializable]
    class IdentifiedCamera
    {
        public RectTransform camera;
        public Season season;
    }
}

