using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private BossHealth bossHealth; // Reference to BossHealth script
    public BossHealthBarUI healthBarUI; // Reference to the health bar UI script

    private bool isTeleporting = false; // Flag to check if the teleportation is in progress
    private bool canTeleport = true; // Flag to prevent teleportation spam
    public AudioSource audioSource; // Audio source to play the sound
    public AudioClip aggroClip; // Aggro sound clip

    private bool hasDamagedPlayerDuringTeleport = false; // Flag to check if damage has been dealt during teleportation

    void Start()
    {
        centipedeCollider = GetComponent<Collider2D>();
        bossHealth = GetComponent<BossHealth>(); // Get the BossHealth component

        if (player == null)
        {
            Debug.LogError("Player reference is not assigned!");
        }

        if (projectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogError("Projectile prefab or spawn point is not assigned!");
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

        if (bossHealth == null)
        {
            Debug.LogError("BossHealth script is not assigned!");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned!");
        }
        if (aggroClip == null)
        {
            Debug.LogError("AggroClip is not assigned!");
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

        // Handle projectile firing only if the boss is aggroed and not teleporting
        if (isAggroed && !isTeleporting)
        {
            HandleProjectileFiring();
        }

        // Handle teleportation only if the boss has taken damage
        if (!canTeleport)
        {
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

        // Play aggro sound
        if (audioSource != null && aggroClip != null)
        {
            audioSource.clip = aggroClip;
            audioSource.loop = true;
            audioSource.Play();
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
        // Allow teleportation only if the boss has taken damage (canTeleport is false after damage)
        if (canTeleport)
        {
            teleportCooldownTimer += Time.deltaTime;

            // Allow teleportation after cooldown
            if (teleportCooldownTimer >= teleportCooldown)
            {
                TeleportToRandomLocation();
                teleportCooldownTimer = 0f; // Reset teleport cooldown timer
            }
        }
    }

    private void LockZPosition()
    {
        Vector3 lockedPosition = transform.position;
        lockedPosition.z = 0f; // Lock Z to 0 or keep it fixed as needed
        transform.position = lockedPosition;
    }

    // Add TakeDamage method here to handle damage to the boss
    public void TakeDamage(float damageAmount)
    {
        if (bossHealth != null)
        {
            bossHealth.TakeDamage(damageAmount); // Use BossHealth script to handle damage
        }
    }

    private void Die()
    {
        Debug.Log("Centipede has been defeated!");

        // Stop aggro sound if it's playing
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        // Optional: Play death animation or effects before destroying
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die"); // Assumes there's a "Die" trigger
                                        // Optionally delay destroy to allow animation to play
            Destroy(gameObject, 2f); // Delay destroy by 2 seconds
        }
        else
        {
            Destroy(gameObject); // No animation, destroy immediately
        }
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

    public GameObject burrowEffectPrefab; // Prefab for the burrow animation effect
    public float burrowEffectDelay = 0.3f; // Delay between each burrow animation

    private IEnumerator TeleportSequence(Transform newTeleportPoint)
    {
        // Disable projectile firing during teleportation
        isTeleporting = true;
        hasDamagedPlayerDuringTeleport = false; // Reset the damage flag

        // Only teleport if the new teleport point is provided
        if (newTeleportPoint == null)
        {
            // Trigger a random teleport if no specific teleport point is provided
            TeleportToRandomLocation();
        }

        // Spawn burrow effects between old and new teleport points
        Vector3 startPoint = transform.position;
        Vector3 endPoint = newTeleportPoint.position;

        // Spawn 4 burrow animations with a delay between each
        for (int i = 0; i < 4; i++)
        {
            GameObject burrowEffect = Instantiate(burrowEffectPrefab, startPoint, Quaternion.identity);
            StartCoroutine(MoveBurrowEffect(burrowEffect, startPoint, endPoint, i * burrowEffectDelay));
            yield return new WaitForSeconds(burrowEffectDelay); // Add delay between each burrow
        }

        // Wait for the final burrow to complete before teleporting
        yield return new WaitForSeconds(burrowEffectDelay * 4); // Wait for all burrow effects to finish

        // Teleport the boss to the new location after the burrow effects
        transform.position = endPoint;
        lastTeleportPoint = newTeleportPoint;

        // Allow the centipede to teleport again
        canTeleport = true;
        isTeleporting = false;

        Debug.Log("Centipede teleported to: " + newTeleportPoint.position);
    }

    private IEnumerator MoveBurrowEffect(GameObject burrowEffect, Vector3 start, Vector3 end, float delay)
    {
        // Wait for the delay before starting the animation
        yield return new WaitForSeconds(delay);

        float duration = 1f; // Adjust this to control the speed of the burrow effect
        float elapsedTime = 0f;

        // Move the burrow effect from start to end over the duration
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            burrowEffect.transform.position = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends at the final position
        burrowEffect.transform.position = end;
    }

    private bool IsPlayerIntersectingLine()
    {
        // Check if the player is intersecting the line
        Vector3 lineStart = lineRenderer.GetPosition(0);
        Vector3 lineEnd = lineRenderer.GetPosition(1);

        // Use a simple distance check to see if the player is close to the line
        float distanceToLine = Mathf.Abs((lineEnd.y - lineStart.y) * player.position.x - (lineEnd.x - lineStart.x) * player.position.y + lineEnd.x * lineStart.y - lineEnd.y * lineStart.x) / Mathf.Sqrt(Mathf.Pow(lineEnd.y - lineStart.y, 2) + Mathf.Pow(lineEnd.x - lineStart.x, 2));

        return distanceToLine <= 0.5f; // Adjust threshold for how close the player needs to be
    }

    private void TakeDamageOnTeleport()
    {
        // Deal damage to the player here
        Debug.Log("Player damaged by teleportation!");
        player.GetComponent<PlayerHealth>().TakeDamage(damageOnTeleport);
    }
    public void OnProjectileDestroyed(GameObject projectile)
    {
        activeProjectiles.Remove(projectile); // Remove the destroyed projectile from the active list
    }

    public bool IsAggroed()
    {
        return isAggroed;
    }
}
