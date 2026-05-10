using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Typewriter Settings")]
    [SerializeField] private float letterDelay = 0.035f;

    [Header("Dialogue Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip blipSound;
    [SerializeField] private int lettersPerBlip = 2;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.08f;

    private string[] currentLines;
    private int currentLineIndex;
    private bool isTyping;
    private Coroutine typingCoroutine;

    private System.Action onDialogueFinished;

    private void Start()
    {
        HideDialogue();
    }

    private void Update()
    {
        if (dialoguePanel == null || !dialoguePanel.activeSelf)
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleDialogueClick();
        }
    }

    public void StartDialogue(string[] lines, System.Action onFinished = null)
    {
        if (lines == null || lines.Length == 0)
            return;

        currentLines = lines;
        currentLineIndex = 0;
        onDialogueFinished = onFinished;

        dialoguePanel.SetActive(true);
        ShowLine(currentLines[currentLineIndex]);
    }

    private void ShowLine(string line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        int visibleLetterCount = 0;

        foreach (char letter in line)
        {
            dialogueText.text += letter;

            if (!char.IsWhiteSpace(letter) && !char.IsPunctuation(letter))
            {
                visibleLetterCount++;

                if (visibleLetterCount % lettersPerBlip == 0)
                {
                    PlayBlip();
                }
            }

            yield return new WaitForSeconds(letterDelay);
        }

        isTyping = false;
    }

    private void PlayBlip()
    {
        if (audioSource == null || blipSound == null)
            return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(blipSound);
    }

    private void HandleDialogueClick()
    {
        if (isTyping)
        {
            FinishCurrentLineInstantly();
            return;
        }

        currentLineIndex++;

        if (currentLineIndex < currentLines.Length)
        {
            ShowLine(currentLines[currentLineIndex]);
        }
        else
        {
            System.Action finishedCallback = onDialogueFinished;
            HideDialogue();
            finishedCallback?.Invoke();
        }
    }

    private void FinishCurrentLineInstantly()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = currentLines[currentLineIndex];
        isTyping = false;
    }

    private void HideDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        onDialogueFinished = null;
        isTyping = false;
        currentLines = null;
        currentLineIndex = 0;
    }
}