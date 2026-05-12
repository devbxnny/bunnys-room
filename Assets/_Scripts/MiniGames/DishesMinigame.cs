using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DishesMinigame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dishesPanel;
    [SerializeField] private RectTransform dishContainer;
    [SerializeField] private Image plateImage;
    [SerializeField] private Image dirtImage;
    [SerializeField] private RectTransform spongeImage;
    [SerializeField] private TMP_Text goodJobText;

    [Header("Scrub Settings")]
    [SerializeField] private float scrubAmountNeeded = 100f;
    [SerializeField] private float scrubGainPerSecond = 45f;
    [SerializeField] private float requiredMouseMoveDistance = 4f;

    [Header("Slide Settings")]
    [SerializeField] private Vector2 hiddenPosition = new Vector2(0, -700);
    [SerializeField] private Vector2 shownPosition = new Vector2(0, 0);
    [SerializeField] private float slideDuration = 0.35f;

    private float currentScrubAmount;
    private Vector2 lastMousePosition;
    private bool minigameActive;
    private bool isFinished;

    private void Start()
    {
        if (dishesPanel != null)
            dishesPanel.SetActive(false);

        if (goodJobText != null)
            goodJobText.gameObject.SetActive(false);

        if (dishContainer != null)
            dishContainer.anchoredPosition = hiddenPosition;
    }

    private void Update()
    {
        if (!minigameActive || isFinished)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (spongeImage != null)
        {
            spongeImage.position = mousePosition;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            float mouseMoveDistance = Vector2.Distance(mousePosition, lastMousePosition);

            if (mouseMoveDistance >= requiredMouseMoveDistance && IsMouseOverDish(mousePosition))
            {
                AddScrubProgress();
            }
        }

        lastMousePosition = mousePosition;
    }

    public void StartMinigame()
    {
        currentScrubAmount = 0f;
        minigameActive = true;
        isFinished = false;

        if (dishesPanel != null)
            dishesPanel.SetActive(true);

        if (goodJobText != null)
            goodJobText.gameObject.SetActive(false);

        SetDirtAlpha(1f);

        if (Mouse.current != null)
            lastMousePosition = Mouse.current.position.ReadValue();

        StartCoroutine(SlideDish(shownPosition));
    }

    private void AddScrubProgress()
    {
        currentScrubAmount += scrubGainPerSecond * Time.deltaTime;

        float cleanPercent = currentScrubAmount / scrubAmountNeeded;
        float dirtAlpha = 1f - cleanPercent;

        SetDirtAlpha(dirtAlpha);

        if (currentScrubAmount >= scrubAmountNeeded)
        {
            FinishDish();
        }
    }

    private bool IsMouseOverDish(Vector2 screenPosition)
    {
        if (plateImage == null)
            return false;

        return RectTransformUtility.RectangleContainsScreenPoint(
            plateImage.rectTransform,
            screenPosition,
            null
        );
    }

    private void SetDirtAlpha(float alpha)
    {
        if (dirtImage == null)
            return;

        Color color = dirtImage.color;
        color.a = Mathf.Clamp01(alpha);
        dirtImage.color = color;
    }

    private void FinishDish()
    {
        isFinished = true;
        minigameActive = false;

        SetDirtAlpha(0f);

        if (goodJobText != null)
            goodJobText.gameObject.SetActive(true);

        StartCoroutine(EndAfterDelay());
    }

    private IEnumerator EndAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        yield return SlideDish(hiddenPosition);

        if (dishesPanel != null)
            dishesPanel.SetActive(false);
    }

    private IEnumerator SlideDish(Vector2 targetPosition)
    {
        if (dishContainer == null)
            yield break;

        Vector2 startPosition = dishContainer.anchoredPosition;
        float timer = 0f;

        while (timer < slideDuration)
        {
            timer += Time.deltaTime;
            float t = timer / slideDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            dishContainer.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        dishContainer.anchoredPosition = targetPosition;
    }
}