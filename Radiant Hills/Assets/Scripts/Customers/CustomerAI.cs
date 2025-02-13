using System.Collections;
using UnityEngine;

public class CustomerAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveStepDuration = 0.5f;
    public Transform counterLocation;
    public LayerMask shelfLayer;
    public float stuckTimeout = 0.3f;
    public float detectionRange = 10f;
    public float pickupRange = 1f;
    public float waitTimeAtCounter = 2f; // Time customer will wait at the counter after delivery
    public float alignmentTolerance = 0.1f; // Tolerance range for the alignment check

    private Vector2 moveDirection;
    private DisplayShelf targetShelf;
    private bool hasItem = false;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private Counter counter;

    private MaterialType itemToDeliver; // Store the material type of the item the customer picks up

    private enum State { Wandering, MovingToItem, PickingUpItem, MovingToCounter, Waiting }
    private State currentState = State.Wandering;

    void Start()
    {
        lastPosition = transform.position;
        counter = counterLocation.GetComponent<Counter>();
        StartCoroutine(StateMachine());
    }

    void Update()
    {
        // Check for "P" key press to display counter's stored items
        if (Input.GetKeyDown(KeyCode.P))
        {
            counter.DisplayStoredItems();
        }
    }

    private IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case State.Wandering:
                    yield return WanderRoutine();
                    break;
                case State.MovingToItem:
                    yield return MoveToTargetRoutine(targetShelf?.transform);
                    currentState = State.PickingUpItem;
                    break;
                case State.PickingUpItem:
                    PickUpItem();
                    break;
                case State.MovingToCounter:
                    yield return MoveToTargetRoutine(counterLocation);
                    DeliverItem();
                    break;
                case State.Waiting:
                    yield return WaitAtCounterRoutine();
                    break;
            }
            yield return null;
        }
    }

    private IEnumerator WanderRoutine()
    {
        while (currentState == State.Wandering)
        {
            yield return ChooseRandomDirection();
            yield return new WaitForSeconds(moveStepDuration);
            CheckForNearbyShelf();
            HandleStuckDetection();
        }
    }

    private IEnumerator ChooseRandomDirection()
    {
        for (int i = 0; i < 10; i++)
        {
            moveDirection = GetRandomDirection();
            if (!IsPathBlocked(moveDirection))
            {
                yield return SmoothMove(moveDirection);
                yield break;
            }
        }
    }

    private Vector2 GetRandomDirection() =>
        Random.Range(0, 4) switch
        {
            0 => Vector2.up,
            1 => Vector2.down,
            2 => Vector2.left,
            _ => Vector2.right
        };

    private bool IsPathBlocked(Vector2 direction)
    {
        return Physics2D.BoxCast(transform.position, Vector2.one * 0.9f, 0, direction, 1f, shelfLayer);
    }

    private IEnumerator SmoothMove(Vector2 direction)
    {
        Vector3 targetPos = (Vector2)transform.position + direction;
        while (Vector2.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void CheckForNearbyShelf()
    {
        foreach (var shelf in FindObjectsOfType<DisplayShelf>())
        {
            if (shelf.HasItem() && Vector2.Distance(transform.position, shelf.transform.position) <= detectionRange)
            {
                targetShelf = shelf;
                currentState = State.MovingToItem;
                return;
            }
        }
    }

    private IEnumerator MoveToTargetRoutine(Transform target)
    {
        while (target != null && Vector2.Distance(transform.position, target.position) > pickupRange)
        {
            Vector3 direction = GetMovementDirection(target.position);
            if (IsPathBlocked(direction))
            {
                direction = FindValidDirection();
            }
            yield return SmoothMove(direction);
        }
    }

    private Vector3 GetMovementDirection(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        return Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? new Vector3(Mathf.Sign(dir.x), 0, 0) : new Vector3(0, Mathf.Sign(dir.y), 0);
    }

    private Vector3 FindValidDirection()
    {
        foreach (var dir in new[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right })
        {
            if (!IsPathBlocked((Vector2)dir)) return dir;
        }
        return Vector3.zero;
    }

    private void PickUpItem()
    {
        if (targetShelf?.HasItem() == true)
        {
            // Assuming we have a way to get the material type from the shelf
            itemToDeliver = targetShelf.GetItem(); // Get the material type of the item
            targetShelf.ClearItem();
            hasItem = true;
            currentState = State.MovingToCounter;
        }
    }

    private void DeliverItem()
    {
        // Check if the customer is aligned with the counter
        if (Mathf.Abs(transform.position.x - counterLocation.position.x) <= alignmentTolerance &&
            Mathf.Abs(transform.position.y - counterLocation.position.y) <= alignmentTolerance)
        {
            if (hasItem && counter != null && itemToDeliver != null)
            {
                counter.ReceiveItem(itemToDeliver); // Pass the MaterialType object
            }
            hasItem = false;
            currentState = State.Waiting; // Move to the waiting state after delivery
        }
        else
        {
            // If not aligned, move the customer toward the counter
            currentState = State.MovingToCounter;
        }
    }

    private IEnumerator WaitAtCounterRoutine()
    {
        yield return new WaitForSeconds(waitTimeAtCounter); // Wait for the specified time
        currentState = State.Wandering; // After waiting, return to wandering
    }

    private void HandleStuckDetection()
    {
        if (Vector3.Distance(transform.position, lastPosition) < 0.1f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckTimeout)
            {
                StartCoroutine(ChooseRandomDirection());
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
        lastPosition = transform.position;
    }
}

