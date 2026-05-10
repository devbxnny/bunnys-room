using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HairBrushingMinigame : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject minigameCanvas;
    [SerializeField] private RectTransform bunnyBackImage;
    [SerializeField] private RectTransform hairBrushArea;
    [SerializeField] private RectTransform brushCursor;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text goodJobText;

    [Header("Settings")]
    [SerializeField] private float brushFillSpeed = 0.4f;
    [SerializeField] private float slideDuration = 0.45f;
    [SerializeField] private Vector2 hiddenPosition = new Vector2(0f, -800f);
    [SerializeField] private Vector2 shownPosition = new Vector2(0f, 0f);

    private bool isMinigameActive;
    private bool isComplete;

    private void Start()
    {
        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(false);
        }

        if (goodJobText != null)
        {
            goodJobText.gameObject.SetActive(false);
        }

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }
    }

    private void Update()
    {
        if (!isMinigameActive || isComplete || Mouse.current == null)
            return;

        UpdateBrushCursor();

        if (Mouse.current.leftButton.isPressed && IsMouseOverHairArea())
        {
            progressBar.value += brushFillSpeed * Time.deltaTime;

            if (progressBar.value >= 1f)
            {
                StartCoroutine(CompleteMinigame());
            }
        }
    }

    public void StartMinigame()
    {
        if (isMinigameActive)
            return;

        StartCoroutine(StartMinigameRoutine());
    }

    private IEnumerator StartMinigameRoutine()
    {
        isMinigameActive = true;
        isComplete = false;

        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(true);
        }

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }

        if (goodJobText != null)
        {
            goodJobText.gameObject.SetActive(false);
        }

        if (bunnyBackImage != null)
        {
            bunnyBackImage.anchoredPosition = hiddenPosition;
            yield return SlideBunny(hiddenPosition, shownPosition);
        }
    }

    private void UpdateBrushCursor()
    {
        if (brushCursor == null)
            return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        brushCursor.position = mouseScreenPosition;
    }

    private bool IsMouseOverHairArea()
    {
        if (hairBrushArea == null)
            return false;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        return RectTransformUtility.RectangleContainsScreenPoint(
            hairBrushArea,
            mouseScreenPosition
        );
    }

    private IEnumerator CompleteMinigame()
    {
        isComplete = true;

        if (progressBar != null)
        {
            progressBar.value = 1f;
        }

        if (goodJobText != null)
        {
            goodJobText.gameObject.SetActive(true);
            goodJobText.text = "Good job!";
        }

        yield return new WaitForSeconds(1.2f);

        if (bunnyBackImage != null)
        {
            yield return SlideBunny(shownPosition, hiddenPosition);
        }

        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(false);
        }

        isMinigameActive = false;
        isComplete = false;
    }

    private IEnumerator SlideBunny(Vector2 startPosition, Vector2 endPosition)
    {
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / slideDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            bunnyBackImage.anchoredPosition = Vector2.Lerp(
                startPosition,
                endPosition,
                t
            );

            yield return null;
        }

        bunnyBackImage.anchoredPosition = endPosition;
    }
}