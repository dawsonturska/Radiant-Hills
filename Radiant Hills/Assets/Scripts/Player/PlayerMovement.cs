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
        lastInputDirection = Vector2.down; // Default direction (e.g., facing downward)
    }

    void Update()
    {
        HandleInput();
        UpdateAnimation(); // Ensure animations update as soon as input changes
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Prioritize vertical movement (prevents diagonal movement)
        if (moveY != 0)
        {
            moveDirection = new Vector2(0, moveY);
            lastInputDirection = moveDirection; // Update immediately
            animator.SetTrigger(moveY > 0 ? "MoveUp" : "MoveDown"); // Trigger transition animation
            animator.SetTrigger("StartMoving"); // Start movement after transition
        }
        else if (moveX != 0)
        {
            moveDirection = new Vector2(moveX, 0);
            lastInputDirection = moveDirection; // Update immediately
            animator.SetTrigger(moveX > 0 ? "MoveRight" : "MoveLeft"); // Trigger transition animation
            animator.SetTrigger("StartMoving"); // Start movement after transition
        }
        else
        {
            moveDirection = Vector2.zero;
            animator.ResetTrigger("StartMoving");
        }
    }

    private void MovePlayer()
    {
        rb.velocity = moveDirection * moveSpeed;
    }

    private void UpdateAnimation()
    {
        // Update the movement animation direction immediately
        if (moveDirection.sqrMagnitude > 0)
        {
            animator.SetFloat("MoveX", moveDirection.x);
            animator.SetFloat("MoveY", moveDirection.y);
            lastInputDirection = moveDirection; // Ensure last direction is updated
        }

        // Ensure idle animations still face the last direction
        animator.SetFloat("IdleX", lastInputDirection.x);
        animator.SetFloat("IdleY", lastInputDirection.y);

        // Update the IsMoving parameter
        animator.SetBool("IsMoving", moveDirection.sqrMagnitude > 0);
    }
}
