using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class DialogueDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject noticeParent;
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private float typeDelay = 0.1f;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private AudioSource audioSource;

    private int partIndex = 0;
    private string[] currentDialogue;
    private string parsedDialoguePart;
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

        dialogueParent.LeanScale(Vector3.zero, 0);
        noticeParent.LeanScale(Vector3.zero, 0);
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
        dialogueParent.LeanScale(Vector3.one, 0.5f);

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
        noticeParent.LeanScale(Vector3.one, 0.5f);

        writingCoroutine = StartCoroutine(WriteText());
    }

    IEnumerator WriteText()
    {
        parsedDialoguePart = (GameManager.Instance != null) ? currentDialogue[partIndex].Replace("%SEASON%", "<color=#" + ColorUtility.ToHtmlStringRGB(GameManager.Instance.gameSeason.color) + ">" + GameManager.Instance.gameSeason.displayName + "</color>") : currentDialogue[partIndex];
        char[] textArray = parsedDialoguePart.ToCharArray();
        currentTarget.text = "";

        while (currentTarget.text.Length < parsedDialoguePart.Length)
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
            audioSource.Stop();
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.Play();

            if (writingCoroutine != null)
            {
                StopCoroutine(writingCoroutine);
                writingCoroutine = null;
                currentTarget.text = parsedDialoguePart;
                partIndex++;
            }
            else if (partIndex < currentDialogue.Length)
            {
                writingCoroutine = StartCoroutine(WriteText());
            }
            else
            {
                if (noticeParent.transform.localScale.magnitude > 0)
                {
                    noticeParent.LeanScale(Vector3.zero, 0.5f).setEaseInOutCubic().setOnComplete(() => noticeParent.SetActive(false));
                }
                if (dialogueParent.transform.localScale.magnitude > 0)
                {
                    dialogueParent.LeanScale(Vector3.zero, 0.5f).setEaseInOutCubic().setOnComplete(() => dialogueParent.SetActive(false));
                }

                currentDialogue = null;
                playerInput.SwitchCurrentActionMap(startingActionMap);
                startingActionMap = null;
                callback?.Invoke();
            }
        }
    }
}
