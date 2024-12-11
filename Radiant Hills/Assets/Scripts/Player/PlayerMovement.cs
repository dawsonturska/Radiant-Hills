using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Vector2 moveDirection; // Stores movement input

    void Start()
    {
        // Get the Rigidbody2D component attached to the player
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get player input (WASD or Arrow keys)
        float moveX = Input.GetAxisRaw("Horizontal"); // Left/Right movement
        float moveY = Input.GetAxisRaw("Vertical"); // Up/Down movement

        // Create a new vector for the movement direction
        moveDirection = new Vector2(moveX, moveY).normalized; // Normalize to prevent faster diagonal movement
    }

    void FixedUpdate()
    {
        // Apply the movement to the Rigidbody2D
        rb.velocity = moveDirection * moveSpeed;
    }
}