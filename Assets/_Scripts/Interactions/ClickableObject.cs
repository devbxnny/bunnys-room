using UnityEngine;
using UnityEngine.InputSystem;

public class ClickableObject : MonoBehaviour
{
    [Header("Object Info")]
    [SerializeField] private string objectName = "Hairbrush";

    [SerializeField] private DialogueLine[] dialogueLines;

    [Header("Click Settings")]
    [SerializeField] private bool canClick = true;
    [SerializeField] private bool disableAfterClick = false;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject glowObject;

    [Header("Bunny Interaction")]
    [SerializeField] private BunnyGridMover bunnyMover;
    [SerializeField] private Transform bunnyTargetPoint;

    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;

    [SerializeField] private HairBrushingMinigame hairBrushingMinigame;
    [SerializeField] private bool startsHairBrushingMinigame;

    [SerializeField] private DishesMinigame dishesMinigame;
    [SerializeField] private bool startsDishesMinigame;

    [Header("Photo Preview")]
    [SerializeField] private PhotoPreviewController photoPreviewController;
    [SerializeField] private bool showsPhotoPreview;

    private Collider2D clickCollider;
    private Camera mainCamera;
    private bool isHovering;


    private void Awake()
    {
        clickCollider = GetComponent<Collider2D>();
        mainCamera = Camera.main;

        if (glowObject != null)
        {
            glowObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!canClick || clickCollider == null || mainCamera == null || Mouse.current == null)
            return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        isHovering = clickCollider.OverlapPoint(mouseWorldPosition);

        if (glowObject != null)
        {
            glowObject.SetActive(isHovering);
        }

        if (isHovering && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void Interact()
    {
        Debug.Log("Clicked: " + objectName);

        if (bunnyMover != null && bunnyTargetPoint != null)
        {
            bunnyMover.MoveToWorldPosition(bunnyTargetPoint.position, ShowObjectDialogue);
        }
        else
        {
            ShowObjectDialogue();
        }

        if (disableAfterClick)
        {
            canClick = false;

            if (glowObject != null)
            {
                glowObject.SetActive(false);
            }
        }
    }

    private void ShowObjectDialogue()
    {
        if (dialogueManager == null)
            return;

        if (showsPhotoPreview && photoPreviewController != null)
        {
            photoPreviewController.ShowPhoto();
        }

        if (startsHairBrushingMinigame && hairBrushingMinigame != null)
        {
            dialogueManager.StartDialogue(dialogueLines, hairBrushingMinigame.StartMinigame);
        }
        else if (startsDishesMinigame && dishesMinigame != null)
        {
            dialogueManager.StartDialogue(dialogueLines, dishesMinigame.StartMinigame);
        }
        else if (showsPhotoPreview && photoPreviewController != null)
        {
            dialogueManager.StartDialogue(dialogueLines, photoPreviewController.HidePhoto);
        }
        else
        {
            dialogueManager.StartDialogue(dialogueLines);
        }
    }
}