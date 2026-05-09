using UnityEngine;
using UnityEngine.InputSystem;

public class ClickableObject : MonoBehaviour
{
    [Header("Object Info")]
    [SerializeField] private string objectName = "Hairbrush";
    [SerializeField] private string interactionMessage = "Bunny looks at the hairbrush.";

    [Header("Click Settings")]
    [SerializeField] private bool canClick = true;
    [SerializeField] private bool disableAfterClick = false;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject glowObject;

    [Header("Bunny Interaction")]
    [SerializeField] private BunnyGridMover bunnyMover;
    [SerializeField] private Transform bunnyTargetPoint;

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
        Debug.Log(interactionMessage);

        if (bunnyMover != null && bunnyTargetPoint != null)
        {
            bunnyMover.MoveToWorldPosition(bunnyTargetPoint.position);
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
}