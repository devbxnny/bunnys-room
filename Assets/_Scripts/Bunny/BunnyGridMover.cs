using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyGridMover : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float moveSpeed = 4f;

    [Header("Sprites")]
    [SerializeField] private Sprite idleFront;
    [SerializeField] private Sprite idleBack;
    [SerializeField] private Sprite idleLeft;
    [SerializeField] private Sprite idleRight;

    [Header("Movement State")]
    [SerializeField] private bool isMoving;

    private SpriteRenderer spriteRenderer;
    private Vector2Int currentGridPosition;
    private Queue<Vector2Int> pathQueue = new Queue<Vector2Int>();

    private System.Action onArrivedCallback;

    public bool IsMoving => isMoving;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentGridPosition = WorldToGrid(transform.position);
        transform.position = GridToWorld(currentGridPosition);

        SetIdleSprite(Vector2.down);
    }

    public void MoveToWorldPosition(Vector3 worldTarget, System.Action onArrived = null)
    {
        Vector2Int targetGridPosition = WorldToGrid(worldTarget);
        MoveToGridPosition(targetGridPosition, onArrived);
    }

    public void MoveToGridPosition(Vector2Int targetGridPosition, System.Action onArrived = null)
    {
        pathQueue.Clear();
        onArrivedCallback = onArrived;

        Vector2Int tempPosition = currentGridPosition;

        while (tempPosition.x != targetGridPosition.x)
        {
            tempPosition.x += targetGridPosition.x > tempPosition.x ? 1 : -1;
            pathQueue.Enqueue(tempPosition);
        }

        while (tempPosition.y != targetGridPosition.y)
        {
            tempPosition.y += targetGridPosition.y > tempPosition.y ? 1 : -1;
            pathQueue.Enqueue(tempPosition);
        }

        if (!isMoving)
        {
            StartCoroutine(FollowPath());
        }
    }

    private IEnumerator FollowPath()
    {
        isMoving = true;

        while (pathQueue.Count > 0)
        {
            Vector2Int nextGridPosition = pathQueue.Dequeue();
            Vector3 nextWorldPosition = GridToWorld(nextGridPosition);

            Vector2 direction = nextGridPosition - currentGridPosition;
            SetIdleSprite(direction);

            yield return MoveOneTile(nextWorldPosition);

            currentGridPosition = nextGridPosition;
        }

        isMoving = false;

        onArrivedCallback?.Invoke();
        onArrivedCallback = null;
    }

    private IEnumerator MoveOneTile(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
    }

    private void SetIdleSprite(Vector2 direction)
    {
        if (spriteRenderer == null) return;

        if (direction.x > 0 && idleRight != null)
        {
            spriteRenderer.sprite = idleRight;
        }
        else if (direction.x < 0 && idleLeft != null)
        {
            spriteRenderer.sprite = idleLeft;
        }
        else if (direction.y > 0 && idleBack != null)
        {
            spriteRenderer.sprite = idleBack;
        }
        else if (direction.y < 0 && idleFront != null)
        {
            spriteRenderer.sprite = idleFront;
        }
    }

    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / tileSize);
        int y = Mathf.RoundToInt(worldPosition.y / tileSize);

        return new Vector2Int(x, y);
    }

    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(
            gridPosition.x * tileSize,
            gridPosition.y * tileSize,
            transform.position.z
        );
    }
}