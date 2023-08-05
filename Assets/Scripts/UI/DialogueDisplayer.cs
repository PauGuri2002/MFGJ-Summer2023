using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class DialogueDisplayer : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float typeDelay = 0.1f;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private AudioSource audioSource;

    [Header("Notice")]
    [SerializeField] private GameObject noticeParent;
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private float noticeTimer = 3f;
    //[SerializeField] private UISizeMatcher backgroundSizeMatcher;

    private int partIndex = 0;
    private bool dialogueShown = false;
    private string[] currentDialogue;
    private string parsedDialoguePart;
    private Coroutine writingCoroutine;
    private string startingActionMap;

    private Action callback;

    public static DialogueDisplayer Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        dialogueParent.LeanScale(Vector3.zero, 0);
        noticeParent.LeanScale(Vector3.zero, 0);
    }

    public void ShowDialogue(Dialogue dialogue, Action _callback = null)
    {
        if (dialogueShown) { return; }
        dialogueShown = true;

        startingActionMap = playerInput.currentActionMap.name;
        playerInput.SwitchCurrentActionMap("UI");

        callback = _callback;
        partIndex = 0;
        currentDialogue = dialogue.parts;

        dialogueParent.SetActive(true);
        dialogueParent.LeanScale(Vector3.one, 0.5f).setEaseInOutCubic();

        writingCoroutine = StartCoroutine(WriteText());
    }

    public void ShowNotice(string text, Action _callback = null)
    {
        if (dialogueShown) { return; }
        dialogueShown = true;

        partIndex = 0;
        noticeText.text = text;

        noticeParent.SetActive(true);
        noticeParent.LeanScale(Vector3.one, 0.5f).setEaseInOutCubic();
        noticeParent.LeanScale(Vector3.zero, 0.5f).setDelay(noticeTimer).setEaseInOutCubic().setOnComplete(() =>
        {
            noticeParent.SetActive(false);
            dialogueShown = false;
        });

        //backgroundSizeMatcher.Match();
    }

    IEnumerator WriteText()
    {
        parsedDialoguePart = (GameManager.Instance != null) ? currentDialogue[partIndex].Replace("%SEASON%", "<color=#" + ColorUtility.ToHtmlStringRGB(GameManager.Instance.gameSeason.color) + ">" + GameManager.Instance.gameSeason.displayName + "</color>") : currentDialogue[partIndex];
        char[] textArray = parsedDialoguePart.ToCharArray();
        dialogueText.text = "";

        while (dialogueText.text.Length < parsedDialoguePart.Length)
        {
            dialogueText.text += textArray[dialogueText.text.Length];
            yield return new WaitForSeconds(typeDelay);
        }

        partIndex++;
        writingCoroutine = null;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (currentDialogue == null) { return; }

        if (context.started)
        {
            audioSource.Stop();
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.Play();

            if (writingCoroutine != null)
            {
                StopCoroutine(writingCoroutine);
                writingCoroutine = null;
                dialogueText.text = parsedDialoguePart;
                partIndex++;
            }
            else if (partIndex < currentDialogue.Length)
            {
                writingCoroutine = StartCoroutine(WriteText());
            }
            else
            {
                if (dialogueParent.transform.localScale.magnitude > 0)
                {
                    dialogueParent.LeanScale(Vector3.zero, 0.5f).setEaseInOutCubic().setOnComplete(() => dialogueParent.SetActive(false));
                }

                currentDialogue = null;
                dialogueShown = false;
                playerInput.SwitchCurrentActionMap(startingActionMap);
                startingActionMap = null;
                callback?.Invoke();
            }
        }
    }
}
