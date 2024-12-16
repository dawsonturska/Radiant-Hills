using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f; // Time until the projectile is destroyed if it doesn't hit anything
    public float damage = 10f; // Amount of damage the projectile does
    public float moveSpeed = 5f; // Speed of the projectile
    public float hoverDuration = 1f; // Time before projectile starts moving towards player
    public Transform boss; // The boss that the projectile will return to

    private Transform player; // Reference to the player's transform
    private bool isLaunched = false; // Check if the projectile has been launched
    private bool isReflected = false; // Check if the projectile has been reflected
    private float hoverTimer = 0f; // Timer to keep track of hover time
    private Rigidbody2D rb; // Rigidbody of the projectile for movement control

    void Start()
    {
        // Find the player transform (assuming it is tagged as "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene!");
            return;
        }

        // Destroy the projectile after a certain lifetime
        Destroy(gameObject, lifetime);

        // Get the Rigidbody2D for later velocity adjustments
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Hovering for the first 1 second before the projectile launches
        if (!isLaunched)
        {
            hoverTimer += Time.deltaTime;

            if (hoverTimer >= hoverDuration)
            {
                isLaunched = true; // Start moving the projectile
            }
        }
        else if (!isReflected)
        {
            // Move the projectile toward the player initially
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    // Trigger for player's attack (weapon is a trigger collider)
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerWeapon") && !isReflected)
        {
            // If the player hits the projectile with their weapon, send it back to the boss
            if (boss != null)
            {
                ReflectProjectileBackToBoss();
            }
        }
    }

    // Handle collisions with the player or other objects (non-trigger collider)
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle collision logic with non-trigger projectiles
        if (collision.gameObject.CompareTag("Player"))
        {
            // Deal damage to the player (optional)
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject); // Destroy the projectile after it hits the player
        }
        else
        {
            Destroy(gameObject); // Destroy the projectile if it hits anything else
        }
    }

    // Reflect the projectile back to the boss
    private void ReflectProjectileBackToBoss()
    {
        // Set the reflected flag
        isReflected = true;

        // Calculate the direction to the boss (projectile moves directly towards the boss)
        Vector3 directionToBoss = (boss.position - transform.position).normalized;

        // Speed multiplier for reflection (adjust this to your desired speed)
        float reflectionSpeedMultiplier = 2f; // 2x the original speed, for example

        // Set velocity to reflect the projectile back to the boss, with increased speed
        if (rb != null)
        {
            rb.velocity = directionToBoss * moveSpeed * reflectionSpeedMultiplier; // Reflect directly towards the boss
        }

        Debug.Log("Projectile reflected back to the boss!");
    }
}
