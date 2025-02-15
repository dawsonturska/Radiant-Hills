using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveDirection;

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

        // Prioritize the most recent input direction
        if (moveY != 0)
        {
            moveDirection = new Vector2(0, moveY); // Vertical input overrides horizontal
        }
        else if (moveX != 0)
        {
            moveDirection = new Vector2(moveX, 0);
        }
        else
        {
            moveDirection = Vector2.zero; // Stop movement when no keys are pressed
        }
    }

    private void MovePlayer()
    {
        rb.velocity = moveDirection * moveSpeed;
    }

    private void UpdateAnimation()
    {
        bool isMoving = moveDirection.sqrMagnitude > 0;

        animator.SetFloat("MoveX", moveDirection.x);
        animator.SetFloat("MoveY", moveDirection.y);
        animator.SetBool("IsMoving", isMoving);
    }
}
