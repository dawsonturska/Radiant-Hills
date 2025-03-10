using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CentipedeBehavior : MonoBehaviour
{
    public Transform player; // Reference to the player's position
    public float aggroRange = 20f; // Aggro range for the boss
    public float fireRate = 1f; // Time between projectile shots
    public float fireCooldown = 1f; // Cooldown between consecutive fires (can be adjusted)
    public float teleportCooldown = 5f; // Time between teleports
    public GameObject projectilePrefab; // The projectile prefab
    public Transform projectileSpawnPoint; // The point where the projectile will spawn

    public List<Transform> teleportPoints; // List of teleport points for the boss to choose from
    public Transform teleportPoint1; // Reference to TeleportPoint1 (for initial teleport)
    public float damageOnTeleport = 10f; // Damage to player when they intersect the teleport line
    public LineRenderer lineRenderer; // Reference to LineRenderer for drawing the teleportation line

    private float fireRateTimer = 0f; // Timer for managing fire rate
    private float fireCooldownTimer = 0f; // Cooldown timer for preventing rapid firing
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

        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned!");
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

        // Handle projectile firing only if the boss is aggroed
        if (isAggroed)
        {
            HandleProjectileFiring();
        }

        // Handle teleportation independently of projectiles
        HandleTeleportation();

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
        fireCooldownTimer += Time.deltaTime;

        // Fire projectiles only after the cooldown period has passed
        if (fireCooldownTimer >= fireRate && fireCooldownTimer >= fireCooldown)
        {
            FireProjectileAtPlayer();
            fireCooldownTimer = 0f; // Reset the cooldown timer
        }
    }

    private void HandleTeleportation()
    {
        teleportCooldownTimer += Time.deltaTime;

        // Allow teleportation to occur after the cooldown
        if (teleportCooldownTimer >= teleportCooldown)
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
            StartCoroutine(TeleportSequence(selectedTeleportPoint));
        }
        else
        {
            Debug.LogWarning("Selected teleport point is null!");
        }
    }

    private IEnumerator TeleportSequence(Transform newTeleportPoint)
    {
        // Draw a line between the old and new teleport points
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, newTeleportPoint.position);
        lineRenderer.enabled = true;

        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Check for player collision with the line for 1 second
        float damageWindowTime = 1f;
        float damageTime = 0f;

        while (damageTime < damageWindowTime)
        {
            if (IsPlayerIntersectingLine())
            {
                // Damage the player
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageOnTeleport);
                }
            }

            damageTime += Time.deltaTime;
            yield return null;
        }

        // Teleport the boss to the new location after 1 second
        transform.position = newTeleportPoint.position;
        lastTeleportPoint = newTeleportPoint;

        // Disable the line renderer after teleportation
        lineRenderer.enabled = false;

        Debug.Log("Centipede teleported to: " + newTeleportPoint.position);
    }

    private bool IsPlayerIntersectingLine()
    {
        // Check if the player is intersecting the line
        Vector3 lineStart = lineRenderer.GetPosition(0);
        Vector3 lineEnd = lineRenderer.GetPosition(1);

        // Use a simple distance check to see if the player is close to the line
        float distanceToLine = Mathf.Abs((lineEnd.y - lineStart.y) * player.position.x - (lineEnd.x - lineStart.x) * player.position.y + lineEnd.x * lineStart.y - lineEnd.y * lineStart.x) / Vector2.Distance(lineStart, lineEnd);

        return distanceToLine < 1f; // You can adjust the threshold for how close the player needs to be to the line to take damage
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
            Debug.Log("Taking damage: " + damage + " Current Health: " + bossHealth.currentHealth);
            bossHealth.TakeDamage(damage); // Apply damage to the boss's health
        }
        else
        {
            Debug.LogError("BossHealth is not assigned.");
        }
    }
}
