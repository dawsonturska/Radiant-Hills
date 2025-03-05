using UnityEngine;
using System.Collections.Generic;

public class TurretLogic : MonoBehaviour
{
    public Transform player; // Reference to the player's position
    public float aggroRange = 20f; // Aggro range for the turret
    public float fireRate = 1f; // Time between projectile shots
    public GameObject projectilePrefab; // The projectile prefab
    public Transform projectileSpawnPoint; // The point where the projectile will spawn
    public Vector3 fireDirection = Vector3.right; // Direction in which the turret fires
    public float spawnOffset = 1f; // Offset to prevent self-collision

    private float fireRateTimer = 0f; // Timer for managing fire rate
    private bool isAggroed = false; // Track if the player is within aggro range
    private List<GameObject> activeProjectiles = new List<GameObject>(); // List of active projectiles
    private Collider2D turretCollider; // Reference to the turret's collider


    void Start()
    {
        turretCollider = GetComponent<Collider2D>();

        if (player == null)
        {
            Debug.LogError("Player reference is not assigned!");
        }

        if (projectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogError("Projectile prefab or spawn point is not assigned!");
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

        // Handle projectile firing only if the turret is aggroed
        if (isAggroed)
        {
            HandleProjectileFiring();
        }
    }

    private void OnAggroPlayer()
    {
        Debug.Log("Player entered aggro range. Turret is now alerted!");
    }

    private void OnLoseAggro()
    {
        Debug.Log("Player left aggro range. Turret is no longer alerted.");
    }

    private void HandleProjectileFiring()
    {
        fireRateTimer += Time.deltaTime;
        if (fireRateTimer >= fireRate)
        {
            FireProjectile();
            fireRateTimer = 0f; // Reset the fire rate timer
        }
    }

    void FireProjectile()
    {
        if (projectilePrefab == null || projectileSpawnPoint == null) return;

        Vector3 spawnPosition = projectileSpawnPoint.position + fireDirection.normalized * spawnOffset;
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);


        // Assign references to the projectile
        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.turretLogic = this;
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
