using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float waitTimeAtCounter = 2f;
    public float alignmentTolerance = 0.1f;

    private Vector2 moveDirection;
    private DisplayShelf targetShelf;
    private bool hasItem = false;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private Counter counter;
    private MaterialType itemToDeliver;

    private Animator animator;

    private enum State { Wandering, MovingToItem, PickingUpItem, MovingToCounter, Waiting }
    private State currentState = State.Wandering;

    void Start()
    {
        lastPosition = transform.position;
        counter = counterLocation.GetComponent<Counter>();
        animator = GetComponent<Animator>();
        StartCoroutine(StateMachine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            counter.DisplayStoredItems();
        }

        bool isCurrentlyMoving = Mathf.Abs(animator.GetFloat("MoveX")) > 0.01f || Mathf.Abs(animator.GetFloat("MoveY")) > 0.01f;
        animator.SetBool("IsMoving", isCurrentlyMoving);
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
                    ResetToWander();
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
                SetAnimatorDirection(moveDirection);
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
        return Physics2D.BoxCast(transform.position, Vector2.one * 0.9f, 0, direction, 0.5f, shelfLayer);
    }

    private IEnumerator SmoothMove(Vector2 direction)
    {
        Vector3 targetPos = new Vector3(Mathf.Round(transform.position.x + direction.x), Mathf.Round(transform.position.y + direction.y), transform.position.z);

        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);

        while (Vector2.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;

        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", 0);
    }

    private void SetAnimatorDirection(Vector2 dir)
    {
        if (animator != null)
        {
            animator.SetFloat("MoveX", dir.x);
            animator.SetFloat("MoveY", dir.y);
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
        List<Vector2> path = FindPath((Vector2)transform.position, (Vector2)target.position);

        foreach (Vector2 step in path)
        {
            Vector2 direction = step - (Vector2)transform.position;
            SetAnimatorDirection(direction);
            yield return SmoothMove(direction);
        }
    }

    private void PickUpItem()
    {
        if (targetShelf?.HasItem() == true)
        {
            animator?.SetTrigger("Ursa_Grab");
            itemToDeliver = targetShelf.GetItem();
            targetShelf.ClearItem();
            hasItem = true;
            currentState = State.MovingToCounter;
        }
    }

    private void DeliverItem()
    {
        if (Vector2.Distance(transform.position, counterLocation.position) <= pickupRange)
        {
            if (hasItem && counter != null && itemToDeliver != null)
            {
                counter.ReceiveItem(itemToDeliver);
            }
            hasItem = false;
            currentState = State.Waiting;
        }
    }

    private IEnumerator WaitAtCounterRoutine()
    {
        yield return new WaitForSeconds(waitTimeAtCounter);
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

    public void ResetToWander()
    {
        StopAllCoroutines();
        currentState = State.Wandering;
        StartCoroutine(StateMachine());
        Debug.Log("CustomerAI reset to wandering.");
    }

    // ---------- PATHFINDING HELPERS ----------

    private bool IsWalkable(Vector2 pos)
    {
        return !Physics2D.OverlapBox(pos, Vector2.one * 0.9f, 0f, shelfLayer);
    }

    private List<Vector2> FindPath(Vector2 start, Vector2 goal)
    {
        Queue<Vector2> frontier = new Queue<Vector2>();
        Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
        frontier.Enqueue(start);
        cameFrom[start] = start;

        Vector2[] directions = new[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        while (frontier.Count > 0)
        {
            Vector2 current = frontier.Dequeue();

            if (Vector2.Distance(current, goal) < 0.5f)
                break;

            foreach (var dir in directions)
            {
                Vector2 next = current + dir;
                if (!cameFrom.ContainsKey(next) && IsWalkable(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        List<Vector2> path = new List<Vector2>();
        Vector2 curr = cameFrom.ContainsKey(goal) ? goal : cameFrom.Keys.OrderBy(p => Vector2.Distance(p, goal)).First();

        while (curr != start)
        {
            path.Insert(0, curr);
            curr = cameFrom[curr];
        }

        return path;
    }
}
