using UnityEngine;
using System.Collections.Generic;

public class CentipedeBehavior : MonoBehaviour
{
    public Transform player; // Reference to the player's position
    public float aggroRange = 20f; // Aggro range for the boss
    public float fireRate = 1f; // Time between projectile shots
    public float teleportCooldown = 5f; // Time between teleports
    public float teleportDistance = 10f; // Distance for teleportation (if desired)
    public GameObject projectilePrefab; // The projectile prefab
    public Transform projectileSpawnPoint; // The point where the projectile will spawn

    public List<Transform> teleportPoints; // List of teleport points for the boss to choose from
    public Transform teleportPoint1; // Reference to TeleportPoint1 (for initial teleport)

    private float fireRateTimer = 0f; // Timer for managing fire rate
    private bool isAggroed = false; // Track if the player is within aggro range
    private List<GameObject> activeProjectiles = new List<GameObject>(); // List of active projectiles
    private Collider2D centipedeCollider; // Reference to the centipede's collider
    private float teleportCooldownTimer = 0f; // Timer for managing teleport cooldowns
    private Transform lastTeleportPoint; // Track the last teleport point

    // Reference to the boss's health
    private BossHealth bossHealth;

    // Reference to the health bar UI script
    public BossHealthBarUI healthBarUI;

    void Start()
    {
        centipedeCollider = GetComponent<Collider2D>();

        if (player == null)
        {
            Debug.LogError("Player reference is not assigned!");
        }

        if (projectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogError("Projectile prefab or spawn point is not assigned!");
        }

        // Get the BossHealth component
        bossHealth = GetComponent<BossHealth>();
        if (bossHealth == null)
        {
            Debug.LogError("BossHealth component is missing on this boss!");
        }

        // Initial teleport to TeleportPoint1 at the start of the game
        if (teleportPoint1 != null)
        {
            transform.position = teleportPoint1.position;
            lastTeleportPoint = teleportPoint1; // Set last teleport point to TeleportPoint1
            Debug.Log("Centipede teleported to initial point: " + teleportPoint1.position);
        }
        else
        {
            Debug.LogError("TeleportPoint1 is not assigned!");
        }
    }

    void Update()
    {
        if (player == null) return; // Do nothing if the player is missing

        // Check if the player is within aggro range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= aggroRange)
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
            HandleProjectileFiring();
            HandleTeleportation();
        }

        // Ensure the Z position is locked to its current value during movement
        LockZPosition();
    }

    private void OnAggroPlayer()
    {
        Debug.Log("Player entered aggro range. Centipede is now alerted!");

        // Show the health bar when aggroed
        if (healthBarUI != null)
        {
            healthBarUI.ToggleHealthBar(true);
        }
    }

    private void OnLoseAggro()
    {
        Debug.Log("Player left aggro range. Centipede is no longer alerted.");

        // Hide the health bar when no longer aggroed
        if (healthBarUI != null)
        {
            healthBarUI.ToggleHealthBar(false);
        }
    }

    private void HandleProjectileFiring()
    {
        fireRateTimer += Time.deltaTime;
        if (fireRateTimer >= fireRate)
        {
            FireProjectileAtPlayer();
            fireRateTimer = 0f; // Reset the fire rate timer
        }
    }

    private void HandleTeleportation()
    {
        teleportCooldownTimer += Time.deltaTime;
        if (teleportCooldownTimer >= teleportCooldown && activeProjectiles.Count >= 3)
        {
            TeleportToRandomLocation();
            teleportCooldownTimer = 0f; // Reset teleport cooldown timer
        }
    }

    private void LockZPosition()
    {
        Vector3 lockedPosition = transform.position;
        lockedPosition.z = 0f; // Lock Z to 0 or keep it fixed as needed
        transform.position = lockedPosition;
    }

    void FireProjectileAtPlayer()
    {
        if (projectilePrefab == null || projectileSpawnPoint == null) return;

        Vector3 directionToPlayer = (player.position - projectileSpawnPoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        // Assign references to the projectile
        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.boss = this.transform;
            projScript.centipedeBehavior = this;
            projScript.IgnoreBossCollider(centipedeCollider, true); // Ignore boss collider initially
        }

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = directionToPlayer * 5f; // Adjust speed as needed
        }

        activeProjectiles.Add(projectile); // Keep track of active projectiles
    }

    public void TeleportToRandomLocation()
    {
        if (teleportPoints.Count == 0)
        {
            Debug.LogWarning("No teleport points assigned!");
            return;
        }

        // Exclude the last teleport point to avoid teleporting to the same spot twice
        List<Transform> availablePoints = new List<Transform>(teleportPoints);
        availablePoints.Remove(lastTeleportPoint);

        // Choose a random teleport point from the remaining points
        int randomIndex = Random.Range(0, availablePoints.Count);
        Transform selectedTeleportPoint = availablePoints[randomIndex];

        if (selectedTeleportPoint != null)
        {
            transform.position = selectedTeleportPoint.position;
            lastTeleportPoint = selectedTeleportPoint; // Update the last teleport point
            Debug.Log("Centipede teleported to: " + selectedTeleportPoint.position);
        }
        else
        {
            Debug.LogWarning("Selected teleport point is null!");
        }
    }

    public void OnProjectileDestroyed(GameObject projectile)
    {
        activeProjectiles.Remove(projectile); // Remove the destroyed projectile from the active list
    }

    // Add this method to allow external damage interaction
    public void TakeDamage(float damage)
    {
        if (bossHealth != null)
        {
            bossHealth.TakeDamage(damage); // Apply damage to the boss's health
        }
        else
        {
            Debug.LogError("BossHealth is not assigned.");
        }
    }
}
