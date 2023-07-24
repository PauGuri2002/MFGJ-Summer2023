using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject noticeParent;
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private float typeDelay = 0.1f;
    [SerializeField] private PlayerInput playerInput;

    private int partIndex = 0;
    private string[] currentDialogue;
    private TextMeshProUGUI currentTarget;
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
    }

    public void ShowDialogue(Dialogue dialogue, Action _callback = null)
    {
        if (currentDialogue != null) { return; }

        startingActionMap = playerInput.currentActionMap.name;
        playerInput.SwitchCurrentActionMap("UI");

        callback = _callback;
        partIndex = 0;
        currentDialogue = dialogue.parts;
        currentTarget = dialogueText;

        dialogueParent.SetActive(true);

        writingCoroutine = StartCoroutine(WriteText());
    }

    public void ShowNotice(string text, Action _callback = null)
    {
        if (currentDialogue != null) { return; }

        startingActionMap = playerInput.currentActionMap.name;
        playerInput.SwitchCurrentActionMap("UI");

        callback = _callback;
        partIndex = 0;
        currentDialogue = new string[] { text };
        currentTarget = noticeText;

        noticeParent.SetActive(true);

        writingCoroutine = StartCoroutine(WriteText());
    }

    IEnumerator WriteText()
    {
        char[] textArray = currentDialogue[partIndex].ToCharArray();
        currentTarget.text = "";

        while (currentTarget.text.Length < currentDialogue[partIndex].Length)
        {
            currentTarget.text += textArray[currentTarget.text.Length];
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
            if (writingCoroutine != null)
            {
                StopCoroutine(writingCoroutine);
                writingCoroutine = null;
                currentTarget.text = currentDialogue[partIndex];
                partIndex++;
            }
            else if (partIndex < currentDialogue.Length)
            {
                writingCoroutine = StartCoroutine(WriteText());
            }
            else
            {
                noticeParent.SetActive(false);
                dialogueParent.SetActive(false);
                currentDialogue = null;
                playerInput.SwitchCurrentActionMap(startingActionMap);
                startingActionMap = null;
                callback?.Invoke();
            }
        }
    }
}