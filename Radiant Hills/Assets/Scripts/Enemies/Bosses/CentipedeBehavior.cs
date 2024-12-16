using UnityEngine;

public class CentipedeBehavior : MonoBehaviour
{
    public Transform player; // Reference to the player's position
    public float aggroRange = 20f; // Aggro range for the boss
    public float teleportCooldown = 5f; // Time between teleports (if desired)
    public float teleportDistance = 10f; // Maximum distance for teleportation
    public GameObject projectilePrefab; // The projectile prefab
    public Transform projectileSpawnPoint; // The point where the projectile will spawn
    public float fireRate = 1f; // Time between projectile shots

    // Trapezoidal room boundaries
    public float minY = -5f; // Minimum Y position (bottom of the room)
    public float maxY = 5f;  // Maximum Y position (top of the room)
    public float minWidth = 2f; // Minimum width at the bottom of the room
    public float maxWidth = 8f; // Maximum width at the top of the room

    private float teleportTimer = 0f;
    private float fireRateTimer = 0f;
    private bool isAggroed = false; // Track if the player is within aggro range
    private int projectileCount = 0; // Count of projectiles fired

    void Update()
    {
        // Check if the player is within aggro range
        if (Vector3.Distance(transform.position, player.position) <= aggroRange)
        {
            if (!isAggroed)
            {
                isAggroed = true;
                OnAggroPlayer();
            }
        }
        else
        {
            if (isAggroed)
            {
                isAggroed = false;
                OnLoseAggro();
            }
        }

        // Handle teleportation and projectile firing only if the boss is aggroed
        if (isAggroed)
        {
            fireRateTimer += Time.deltaTime;
            if (fireRateTimer >= fireRate)
            {
                FireProjectileAtPlayer();
                fireRateTimer = 0f; // Reset the fire rate timer
            }
        }

        // Ensure the Z position is locked to its current value during movement
        Vector3 lockedPosition = transform.position;
        lockedPosition.z = 0f; // Lock Z to 0 or keep it fixed as needed
        transform.position = lockedPosition;
    }

    // Handle boss behavior when the player is in aggro range
    private void OnAggroPlayer()
    {
        Debug.Log("Player entered aggro range. Centipede is now alerted!");
    }

    // Handle boss behavior when the player is no longer in aggro range
    private void OnLoseAggro()
    {
        Debug.Log("Player left aggro range. Centipede is no longer alerted.");
    }

    // Teleport to a random location within the room boundaries
    void TeleportToRandomLocation()
    {
        // Get a random position within the specified teleport distance
        Vector3 randomDirection = Random.insideUnitSphere * teleportDistance;
        randomDirection.y = Mathf.Clamp(randomDirection.y, minY, maxY); // Ensure Y is within the room's height

        // Calculate the X position based on Y to ensure it's within the trapezoidal boundaries
        float widthAtY = GetWidthAtY(randomDirection.y);
        randomDirection.x = Mathf.Clamp(randomDirection.x, -widthAtY / 2f, widthAtY / 2f); // Ensure X is within the trapezoidal bounds

        // Lock the Z position to its current value (you can change this to a fixed value like 0f if desired)
        randomDirection.z = transform.position.z;

        // Set the new position
        transform.position += randomDirection;

        Debug.Log("Centipede Teleported to: " + transform.position);
    }

    // Calculate the width of the room at a given Y position
    float GetWidthAtY(float y)
    {
        // Linear interpolation between min and max width based on Y position
        float t = Mathf.InverseLerp(minY, maxY, y); // Normalize Y to a 0-1 range
        return Mathf.Lerp(minWidth, maxWidth, t); // Linearly interpolate between minWidth and maxWidth
    }

    // Fire projectiles at the player
    void FireProjectileAtPlayer()
    {
        if (player != null && projectilePrefab != null && projectileSpawnPoint != null)
        {
            // Calculate the direction to the player
            Vector3 directionToPlayer = (player.position - projectileSpawnPoint.position).normalized;

            // Instantiate the projectile and set its direction
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = directionToPlayer * 5f; // Adjust speed as needed
            }

            Debug.Log("Centipede Fired a Projectile!");

            // Increment projectile count
            projectileCount++;

            // Check if 3 projectiles have been fired and teleport if so
            if (projectileCount >= 3)
            {
                TeleportToRandomLocation();
                projectileCount = 0; // Reset the projectile counter
            }
        }
    }
}
