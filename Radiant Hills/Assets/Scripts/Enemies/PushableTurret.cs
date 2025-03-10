using UnityEngine;
using System.Collections.Generic;

public class PushTurretLogic : MonoBehaviour
{
    [Header("Targeting & Firing")]
    public Transform player; // Reference to the player's position
    public Transform focalPoint; // The target point to fire at
    public float aggroRange = 20f; // Aggro range for the turret
    public float fireRate = 1f; // Time between projectile shots
    public GameObject projectilePrefab; // The projectile prefab
    public Transform projectileSpawnPoint; // The point where the projectile will spawn
    public float spawnOffset = 1f; // Offset to prevent self-collision
    public bool invertDirection = false; // Toggle to invert firing direction

    [Header("Polygonal Area")]
    public List<PolygonCollider2D> fireAreas; // List of polygon colliders that define valid firing areas

    private float fireRateTimer = 0f; // Timer for managing fire rate
    private bool isAggroed = false; // Track if the player is within aggro range
    private bool hasFiredInitially = false; // Track if the turret has fired initially
    private bool canStartFiring = false; // Flag to indicate if the turret can start firing after the delay
    private float delayTimer = 0f; // Timer for the 5-second delay
    private List<GameObject> activeProjectiles = new List<GameObject>(); // List of active projectiles
    private Collider2D turretCollider; // Reference to the turret's collider

    void Start()
    {
        turretCollider = GetComponent<Collider2D>();

        // Validate required references
        if (player == null)
            Debug.LogError($"{gameObject.name}: Player reference is not assigned!");

        if (focalPoint == null)
            Debug.LogError($"{gameObject.name}: Focal point is not assigned!");

        if (projectilePrefab == null || projectileSpawnPoint == null)
            Debug.LogError($"{gameObject.name}: Projectile prefab or spawn point is not assigned!");

        if (fireAreas == null || fireAreas.Count == 0)
            Debug.LogError($"{gameObject.name}: No fire area colliders are assigned!");
    }

    void Update()
    {
        if (player == null || focalPoint == null || fireAreas == null || fireAreas.Count == 0) return; // Prevent execution if references are missing

        // Check if the player is within aggro range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerInRange = distanceToPlayer <= aggroRange;

        if (playerInRange && !isAggroed)
        {
            isAggroed = true;
            OnAggroPlayer();
        }
        else if (!playerInRange && isAggroed)
        {
            isAggroed = false;
            OnLoseAggro();
        }

        // Fire projectiles only if the turret is inside one of the fire areas, aggroed, and has passed the delay time
        if (isAggroed && IsInsideFireArea())
        {
            if (!canStartFiring)
            {
                delayTimer += Time.deltaTime; // Start counting the delay

                if (delayTimer >= 5f) // 5-second delay
                {
                    canStartFiring = true; // Allow firing after delay
                    Debug.Log($"{gameObject.name}: Turret can start firing after delay.");
                }
            }

            // Only start firing if the turret has passed the delay
            if (canStartFiring)
            {
                if (!hasFiredInitially)
                {
                    FireProjectile();
                    hasFiredInitially = true;
                }
                HandleProjectileFiring();
            }
        }
    }

    private void OnAggroPlayer()
    {
        Debug.Log($"{gameObject.name}: Player entered aggro range!");
    }

    private void OnLoseAggro()
    {
        Debug.Log($"{gameObject.name}: Player left aggro range.");
    }

    private bool IsInsideFireArea()
    {
        // Check if the turret is inside any of the polygon fire areas
        foreach (PolygonCollider2D fireArea in fireAreas)
        {
            if (fireArea.OverlapPoint(transform.position))
            {
                return true; // Turret is inside this fire area
            }
        }
        return false; // Turret is not inside any valid fire area
    }

    private void HandleProjectileFiring()
    {
        fireRateTimer += Time.deltaTime;
        if (fireRateTimer >= fireRate)
        {
            FireProjectile();
            fireRateTimer = 0f;
        }
    }

    void FireProjectile()
    {
        if (projectilePrefab == null || projectileSpawnPoint == null || focalPoint == null) return;

        // Calculate the direction towards the focal point
        Vector3 direction = (focalPoint.position - projectileSpawnPoint.position).normalized;
        if (invertDirection)
        {
            direction = -direction; // Invert direction if needed
        }

        // Calculate spawn position with offset
        Vector3 spawnPosition = projectileSpawnPoint.position + direction * spawnOffset;
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Assign references to the projectile
        TurretProjectile projScript = projectile.GetComponent<TurretProjectile>();
        if (projScript != null)
        {
            projScript.Initialize(this, direction); // Initialize the projectile with the direction and turret reference
        }
        else
        {
            Debug.LogError("Projectile script not found on instantiated projectile.");
        }

        activeProjectiles.Add(projectile); // Track the active projectiles
    }

    public void OnProjectileDestroyed(GameObject projectile)
    {
        // Remove destroyed projectiles from the list of active projectiles
        if (activeProjectiles.Contains(projectile))
        {
            activeProjectiles.Remove(projectile);
        }
    }
}
