using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveDirection;
    private Vector2 lastInputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        lastInputDirection = Vector2.down; // Default direction (e.g., down)
    }

    void Update()
    {
        HandleInput();
        UpdateAnimation(); // Update animation based on input direction
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveY != 0)
        {
            moveDirection = new Vector2(0, moveY);
            lastInputDirection = moveDirection; // Update immediately
        }
        else if (moveX != 0)
        {
            moveDirection = new Vector2(moveX, 0);
            lastInputDirection = moveDirection; // Update immediately
        }
        else
        {
            moveDirection = Vector2.zero;
        }
    }

    private void MovePlayer()
    {
        rb.velocity = moveDirection * moveSpeed;
    }

    private void UpdateAnimation()
    {
        // Ensure that the correct direction is used based on player input.
        if (moveDirection.sqrMagnitude > 0)
        {
            animator.SetFloat("MoveX", moveDirection.x);
            animator.SetFloat("MoveY", moveDirection.y);
        }
        else
        {
            // Use the last input direction when no movement is happening
            animator.SetFloat("MoveX", lastInputDirection.x);
            animator.SetFloat("MoveY", lastInputDirection.y);
        }

        // Set the "IsMoving" flag for animation transitions.
        bool isMoving = moveDirection.sqrMagnitude > 0;
        animator.SetBool("IsMoving", isMoving);
    }
}
