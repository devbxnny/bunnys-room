using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PhotoPreviewController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform photoPanel;

    [Header("Slide Settings")]
    [SerializeField] private Vector2 hiddenPosition = new Vector2(0, -700);
    [SerializeField] private Vector2 shownPosition = new Vector2(0, 0);
    [SerializeField] private float slideDuration = 0.35f;

    private Coroutine slideCoroutine;

    private void Start()
    {
        photoPanel.anchoredPosition = hiddenPosition;
        photoPanel.gameObject.SetActive(false);
    }

    public void ShowPhoto()
    {
        photoPanel.gameObject.SetActive(true);
        SlideTo(shownPosition);
    }

    public void HidePhoto()
    {
        SlideTo(hiddenPosition, true);
    }

    private void SlideTo(Vector2 targetPosition, bool disableAfter = false)
    {
        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideRoutine(targetPosition, disableAfter));
    }

    private IEnumerator SlideRoutine(Vector2 targetPosition, bool disableAfter)
    {
        Vector2 startPosition = photoPanel.anchoredPosition;
        float timer = 0f;

        while (timer < slideDuration)
        {
            timer += Time.deltaTime;
            float t = timer / slideDuration;

            t = Mathf.SmoothStep(0f, 1f, t);

            photoPanel.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        photoPanel.anchoredPosition = targetPosition;

        if (disableAfter)
            photoPanel.gameObject.SetActive(false);
    }

}