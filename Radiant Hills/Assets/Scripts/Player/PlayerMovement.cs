using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveDirection;

    private float lastMoveX = 0f;
    private float lastMoveY = -1f; // Default to facing down
    private float movementThreshold = 0.1f; // Prevents small movements from toggling animations

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(moveX) > movementThreshold)
        {
            moveY = 0;
        }
        else if (Mathf.Abs(moveY) > movementThreshold)
        {
            moveX = 0;
        }

        moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveDirection.sqrMagnitude > 0)
        {
            lastMoveX = moveX;
            lastMoveY = moveY;
        }
    }

    private void MovePlayer()
    {
        rb.velocity = moveDirection * moveSpeed;
    }

    private void UpdateAnimation()
    {
        bool isMoving = moveDirection.sqrMagnitude > movementThreshold;

        if (isMoving)
        {
            animator.SetFloat("MoveX", moveDirection.x);
            animator.SetFloat("MoveY", moveDirection.y);
        }
        else
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);
        }

        animator.SetBool("IsMoving", isMoving);
    }
}
