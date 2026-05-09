using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyGridMover : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float moveSpeed = 4f;

    [Header("Movement State")]
    [SerializeField] private bool isMoving;

    private Vector2Int currentGridPosition;
    private Queue<Vector2Int> pathQueue = new Queue<Vector2Int>();

    public bool IsMoving => isMoving;

    private void Start()
    {
        currentGridPosition = WorldToGrid(transform.position);
        transform.position = GridToWorld(currentGridPosition);
    }

    public void MoveToWorldPosition(Vector3 worldTarget)
    {
        Vector2Int targetGridPosition = WorldToGrid(worldTarget);
        MoveToGridPosition(targetGridPosition);
    }

    public void MoveToGridPosition(Vector2Int targetGridPosition)
    {
        pathQueue.Clear();

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

            yield return MoveOneTile(nextWorldPosition);

            currentGridPosition = nextGridPosition;
        }

        isMoving = false;
    }

    private IEnumerator MoveOneTile(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;

        Vector2 direction = targetPosition - startPosition;

        // Later this direction can drive animations:
        // direction.x > 0 = walk right
        // direction.x < 0 = walk left
        // direction.y > 0 = walk up
        // direction.y < 0 = walk down

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