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
        cameras[season].SetAsLastSibling();
        if (time <= 0)
        {
            cameras[season].rect.Set(0, 0, canvasSize.x, canvasSize.y);
        }
        else
        {
            StartCoroutine(Interpolation(cameras[season].rect, new Rect(cameras[season].rect), new Rect(0, 0, canvasSize.x, canvasSize.y), time));
        }
    }

    public void SetGrid(float time = 0f, Season[] newOrder = null)
    {
        if (newOrder != null)
        {
            cameraOrder = newOrder;
        }

        for (int i = 0; i < cameraOrder.Length; i++)
        {
            Rect cam = cameras[cameraOrder[i]].rect;

            float x = i % 2 * canvasSize.x / 2;
            float y = i / 2 * canvasSize.y / 2;

            if (time <= 0)
            {
                cam.Set(x, y, canvasSize.x / 2, canvasSize.y / 2);
            }
            else
            {
                StartCoroutine(Interpolation(cam, new Rect(cam), new Rect(x, y, canvasSize.x / 2, canvasSize.y / 2), time));
            }
        }
    }

    IEnumerator Interpolation(Rect target, Rect from, Rect to, float time)
    {
        float progress = 0f;
        do
        {
            target.Set(
                Mathf.Lerp(from.x, to.x, progress),
                Mathf.Lerp(from.y, to.y, progress),
                Mathf.Lerp(from.width, to.width, progress),
                Mathf.Lerp(from.height, to.height, progress)
            );

            progress += 1 / time * Time.deltaTime;
            yield return 0;
        } while (progress < 1);
    }

    [Serializable]
    class IdentifiedCamera
    {
        public RectTransform camera;
        public Season season;
    }
}

