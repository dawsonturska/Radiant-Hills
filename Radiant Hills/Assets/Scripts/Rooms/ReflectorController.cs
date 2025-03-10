using UnityEngine;

public class ProjectileReflectorController : MonoBehaviour
{
    public float pushStrength = 5f; // How strong the push is
    private Rigidbody2D rb;
    private bool isBeingPushed = false;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D component found on " + gameObject.name);
        }

        // Set Rigidbody2D to Kinematic
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Get reference to the player object (assuming player has the "Player" tag)
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player object not found!");
        }

        // Ensure collider is not a trigger (non-trigger collider)
        Collider2D reflectorCollider = GetComponent<Collider2D>();
        reflectorCollider.isTrigger = false;  // No trigger, interact as a solid object

        // Ignore collision with projectiles (assuming all projectiles have a "Projectile" tag)
        Collider2D[] projectiles = FindObjectsOfType<Collider2D>();
        foreach (var projectile in projectiles)
        {
            if (projectile.CompareTag("Projectile")) // Assuming projectiles have the tag "Projectile"
            {
                Physics2D.IgnoreCollision(reflectorCollider, projectile);
            }
        }
    }

    void Update()
    {
        if (isBeingPushed && player != null)
        {
            // Apply force to the reflector based on player's position
            PushReflector();
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // Check if the player collides with the reflector
        if (other.gameObject.CompareTag("Player"))
        {
            isBeingPushed = true;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        // When the player leaves the collider, stop pushing
        if (other.gameObject.CompareTag("Player"))
        {
            isBeingPushed = false;
        }
    }

    void PushReflector()
    {
        // Detect direction from the player to the reflector
        Vector2 direction = (transform.position - player.position).normalized;

        // Ensure movement is only in X or Y direction (no diagonal movement)
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            direction = new Vector2(direction.x, 0f);  // Move only along X
        }
        else
        {
            direction = new Vector2(0f, direction.y);  // Move only along Y
        }

        // Move the reflector by adjusting its position using MovePosition (works for Kinematic)
        rb.MovePosition(rb.position + direction * pushStrength * Time.deltaTime);
    }
}
