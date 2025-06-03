using System.Collections;
using UnityEngine;



public class CustomerAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveStepDuration = 0.5f;
    public Transform registerLocation;
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
    private Register register;
    private MaterialType itemToDeliver;

    private Animator animator;

    private enum State { Wandering, MovingToItem, PickingUpItem, MovingToCounter, Waiting }
    private State currentState = State.Wandering;

    void Start()
    {
        lastPosition = transform.position;
        register = registerLocation.GetComponent<Register>();
        animator = GetComponent<Animator>();
        StartCoroutine(StateMachine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            register.DisplayStoredItems();
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
                    yield return MoveToTargetRoutine(registerLocation);
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
        Vector3 targetPos = (Vector2)transform.position + direction;

        // Set movement direction and enable movement animation
        SetAnimatorDirection(direction);
        animator.SetBool("IsMoving", true);

        while (Vector2.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Snap to final position and stop movement animation
        transform.position = targetPos;
        animator.SetBool("IsMoving", false);
    }

    private void SetAnimatorDirection(Vector2 dir)
    {
        if (animator != null)
        {
            if (!Mathf.Approximately(animator.GetFloat("MoveX"), dir.x))
                animator.SetFloat("MoveX", dir.x);

            if (!Mathf.Approximately(animator.GetFloat("MoveY"), dir.y))
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
        while (target != null && Vector2.Distance(transform.position, target.position) > pickupRange)
        {
            Vector3 direction = GetMovementDirection(target.position);
            if (IsPathBlocked(direction))
            {
                direction = FindValidDirection();
            }
            SetAnimatorDirection(direction);
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
            animator?.SetTrigger("Ursa_Grab"); // Trigger grab animation
            itemToDeliver = targetShelf.GetItem();
            targetShelf.ClearItem();
            hasItem = true;
            currentState = State.MovingToCounter;
        }
    }

    private void DeliverItem()
    {
        if (Vector2.Distance(transform.position, registerLocation.position) <= pickupRange)
        {
            if (hasItem && register != null && itemToDeliver != null)
            {
                register.ReceiveItem(itemToDeliver);
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
}
