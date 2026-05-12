using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image portraitImage;

    [Header("Bunny Mood Portraits")]
    [SerializeField] private Sprite neutralPortrait;
    [SerializeField] private Sprite happyPortrait;
    [SerializeField] private Sprite joyfulPortrait;
    [SerializeField] private Sprite sadPortrait;
    [SerializeField] private Sprite annoyedPortrait;
    [SerializeField] private Sprite blushingPortrait;
    [SerializeField] private Sprite embarrassedPortrait;
    [SerializeField] private Sprite noFacePortrait;

    [Header("Typewriter Settings")]
    [SerializeField] private float letterDelay = 0.035f;

    [Header("Dialogue Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip blipSound;
    [SerializeField] private int lettersPerBlip = 2;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.08f;

    private DialogueLine[] currentLines;
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

    public void StartDialogue(DialogueLine[] lines, System.Action onFinished = null)
    {
        if (lines == null || lines.Length == 0)
            return;

        currentLines = lines;
        currentLineIndex = 0;
        onDialogueFinished = onFinished;

        dialoguePanel.SetActive(true);
        ShowLine(currentLines[currentLineIndex]);
    }

    private void ShowLine(DialogueLine line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        SetPortrait(line.mood);
        typingCoroutine = StartCoroutine(TypeLine(line.text));
    }

    private void SetPortrait(BunnyMood mood)
    {
        if (portraitImage == null)
            return;

        switch (mood)
        {
            case BunnyMood.Happy:
                portraitImage.sprite = happyPortrait;
                break;

            case BunnyMood.Joyful:
                portraitImage.sprite = joyfulPortrait;
                break;

            case BunnyMood.Sad:
                portraitImage.sprite = sadPortrait;
                break;

            case BunnyMood.Annoyed:
                portraitImage.sprite = annoyedPortrait;
                break;

            case BunnyMood.Blushing:
                portraitImage.sprite = blushingPortrait;
                break;

            case BunnyMood.Embarrassed:
                portraitImage.sprite = embarrassedPortrait;
                break;

            case BunnyMood.NoFace:
                portraitImage.sprite = noFacePortrait;
                break;

            default:
                portraitImage.sprite = neutralPortrait;
                break;
        }
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

        dialogueText.text = currentLines[currentLineIndex].text;
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