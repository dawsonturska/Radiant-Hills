using UnityEngine;
using System.Collections.Generic;

public class TurretLogic : MonoBehaviour
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

    private float fireRateTimer = 0f; // Timer for managing fire rate
    private bool isAggroed = false; // Track if the player is within aggro range
    private bool hasFiredInitially = false; // Track if the turret has fired initially
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
    }

    void Update()
    {
        if (player == null || focalPoint == null) return; // Prevent execution if references are missing

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

        // Fire projectiles when aggroed
        if (isAggroed)
        {
            if (!hasFiredInitially)
            {
                FireProjectile();
                hasFiredInitially = true;
            }
            HandleProjectileFiring();
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

        Vector3 direction = (focalPoint.position - projectileSpawnPoint.position).normalized;
        if (invertDirection)
        {
            direction = -direction;
        }

        Vector3 spawnPosition = projectileSpawnPoint.position + direction * spawnOffset;
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Assign references to the projectile
        TurretProjectile projScript = projectile.GetComponent<TurretProjectile>();
        if (projScript != null)
        {
            projScript.Initialize(this, direction); // Call the Initialize method of TurretProjectile
        }
        else
        {
            Debug.LogError("Projectile script not found on instantiated projectile.");
        }

        activeProjectiles.Add(projectile);
    }

    public void OnProjectileDestroyed(GameObject projectile)
    {
        if (activeProjectiles.Contains(projectile))
        {
            activeProjectiles.Remove(projectile);
        }
    }
}
