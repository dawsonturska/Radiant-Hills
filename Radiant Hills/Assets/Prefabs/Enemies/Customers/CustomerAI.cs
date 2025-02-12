using System.Collections;
using UnityEngine;

public class CustomerAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveStepDuration = 0.5f;
    public Transform counterLocation;
    public LayerMask shelfLayer;
    public float stuckTimeout = 0.3f;  // Time before AI finds an opposite direction if stuck
    public float detectionRange = 10f; // Distance to detect an item on a shelf
    public float pickupRange = 1f; // Range to pick up an item from the shelf

    private Vector2 moveDirection;
    private DisplayShelf targetShelf;
    private bool hasItem = false;
    private bool isMoving = false;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;

    private enum State { Wandering, MovingToItem, PickingUpItem, MovingToCounter }
    private State currentState = State.Wandering;

    void Start()
    {
        lastPosition = transform.position;
        StartCoroutine(StateMachine());  // Start the state machine
    }

    private IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case State.Wandering:
                    yield return StartCoroutine(WanderRoutine());
                    break;
                case State.MovingToItem:
                    yield return StartCoroutine(MoveToItemRoutine());
                    break;
                case State.PickingUpItem:
                    yield return StartCoroutine(PickUpItemRoutine());
                    break;
                case State.MovingToCounter:
                    yield return StartCoroutine(MoveToCounterRoutine());
                    break;
            }

            yield return null;
        }
    }

    private IEnumerator WanderRoutine()
    {
        Debug.Log("Customer is wandering.");

        while (currentState == State.Wandering)
        {
            if (!isMoving)
            {
                ChooseRandomDirection();  // Choose a random direction to move in
                yield return new WaitForSeconds(moveStepDuration);
                CheckForNearbyShelf();  // Check for nearby items on shelves
            }

            // Stuck detection: if position hasn't changed significantly, it's stuck
            if (Vector3.Distance(transform.position, lastPosition) < 0.1f)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer >= stuckTimeout)
                {
                    Debug.Log("Customer is stuck. Finding a new direction.");
                    ChooseRandomDirection();  // Find a new direction if stuck
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;  // Reset stuck timer if the AI moved
            }

            lastPosition = transform.position;  // Update the last position for stuck detection
            yield return null;
        }
    }

    private void ChooseRandomDirection()
    {
        if (isMoving) return;

        int attempts = 0;
        bool validDirection = false;

        while (!validDirection && attempts < 10)
        {
            attempts++;
            moveDirection = GetRandomDirection();

            if (!IsPathBlocked(moveDirection))  // Check if path is clear
            {
                validDirection = true;
                StartCoroutine(SmoothMove(moveDirection));  // Move in the selected direction
            }
        }

        if (!validDirection)
        {
            Debug.Log("Could not find a valid path after several attempts, trying again.");
            moveDirection = GetRandomDirection();  // Force a new direction
            StartCoroutine(SmoothMove(moveDirection));
        }
    }

    private Vector2 GetRandomDirection()
    {
        int direction = Random.Range(0, 4);
        switch (direction)
        {
            case 0: return Vector2.up;        // Positive Y direction
            case 1: return Vector2.down;      // Negative Y direction
            case 2: return Vector2.left;      // Negative X direction
            case 3: return Vector2.right;     // Positive X direction
            default: return Vector2.zero;
        }
    }

    private bool IsPathBlocked(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(0.9f, 0.9f), 0, direction, 3f, shelfLayer);

        return hit.collider != null; // Check if the boxcast hits anything that is not the customer
    }

    private IEnumerator SmoothMove(Vector2 direction)
    {
        isMoving = true;

        Vector3 targetPos = (Vector2)transform.position + direction;
        Vector3 initialPosition = transform.position;

        while (Vector2.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (Vector3.Distance(initialPosition, transform.position) < 0.1f)
        {
            Debug.Log("Movement blocked, trying a new direction.");
            ChooseRandomDirection();  // Force a new direction if blocked
        }
        else
        {
            transform.position = targetPos;
        }

        isMoving = false;
    }

    private void CheckForNearbyShelf()
    {
        DisplayShelf[] shelves = FindObjectsOfType<DisplayShelf>();

        foreach (var shelf in shelves)
        {
            if (shelf.HasItem() && Vector2.Distance(transform.position, shelf.transform.position) <= detectionRange)
            {
                targetShelf = shelf;
                currentState = State.MovingToItem;
                Debug.Log($"Found an item on shelf {shelf.shelfID}, moving towards it.");
                StopCoroutine(WanderRoutine());  // Stop wandering
                return;
            }
        }
    }

    private IEnumerator MoveToItemRoutine()
    {
        if (targetShelf == null) yield break;

        while (Vector2.Distance(transform.position, targetShelf.transform.position) > pickupRange)
        {
            Vector3 direction = (targetShelf.transform.position - transform.position).normalized;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                direction = new Vector3(Mathf.Sign(direction.x), 0, 0);
            else
                direction = new Vector3(0, Mathf.Sign(direction.y), 0);

            if (!IsPathBlocked(direction))
            {
                yield return StartCoroutine(SmoothMove((Vector2)direction));
            }
            else
            {
                Debug.Log("Path to item blocked, trying alternate path.");
                Vector3 alternateDirection = FindValidDirectionTowards(targetShelf.transform.position);
                if (alternateDirection != Vector3.zero)
                {
                    yield return StartCoroutine(SmoothMove((Vector2)alternateDirection));
                }
                else
                {
                    ChooseRandomDirection();  // Fallback if no path is available
                }
            }
        }

        currentState = State.PickingUpItem;
        StartCoroutine(PickUpItemRoutine());
    }

    private Vector3 FindValidDirectionTowards(Vector3 target)
    {
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

        foreach (var dir in directions)
        {
            if (!IsPathBlocked((Vector2)dir))
            {
                return dir;
            }
        }

        return Vector3.zero;  // No valid direction found
    }

    private IEnumerator PickUpItemRoutine()
    {
        if (targetShelf != null && targetShelf.HasItem())
        {
            targetShelf.ClearItem();
            hasItem = true;
            Debug.Log("Item picked up.");

            currentState = State.MovingToCounter;
            StartCoroutine(MoveToCounterRoutine());
        }

        yield return null;
    }

    private IEnumerator MoveToCounterRoutine()
    {
        while (Vector2.Distance(transform.position, counterLocation.position) > pickupRange)
        {
            Vector3 direction = (counterLocation.position - transform.position).normalized;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                direction = new Vector3(Mathf.Sign(direction.x), 0, 0);
            else
                direction = new Vector3(0, Mathf.Sign(direction.y), 0);

            if (!IsPathBlocked(direction))
            {
                yield return StartCoroutine(SmoothMove((Vector2)direction));
            }
            else
            {
                Debug.Log("Path to counter blocked, trying alternate path.");
                Vector3 alternateDirection = FindValidDirectionTowards(counterLocation.position);
                if (alternateDirection != Vector3.zero)
                {
                    yield return StartCoroutine(SmoothMove((Vector2)alternateDirection));
                }
                else
                {
                    ChooseRandomDirection();  // Fallback if no path is available
                }
            }
        }

        DeliverItem();  // Deliver item to counter
    }

    private void DeliverItem()
    {
        hasItem = false;
        Debug.Log("Item delivered to counter.");
        currentState = State.Wandering;
        StartCoroutine(WanderRoutine());  // Resume wandering after delivery
    }
}
