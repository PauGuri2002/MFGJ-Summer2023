using System;
using TMPro;
using UnityEngine;

public class NoticeDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject noticeParent;
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private float noticeTimer = 3f;

    private bool dialogueShown = false;

    public static NoticeDisplayer Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        noticeParent.LeanScale(Vector3.zero, 0);
    }

    public void ShowNotice(string text, Action _callback = null)
    {
        if (dialogueShown) { return; }
        dialogueShown = true;

        noticeText.text = text;

        noticeParent.SetActive(true);
        noticeParent.LeanScale(Vector3.one, 0.5f).setEaseInOutCubic();
        noticeParent.LeanScale(Vector3.zero, 0.5f).setDelay(noticeTimer).setEaseInOutCubic().setOnComplete(() =>
        {
            noticeParent.SetActive(false);
            dialogueShown = false;
        });
    }
}
