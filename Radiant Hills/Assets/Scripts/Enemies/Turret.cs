using UnityEngine;
using System.Collections.Generic;

public class TurretLogic : MonoBehaviour
{
    [Header("Targeting & Firing")]
    public Transform player;
    public Transform focalPoint;
    public float aggroRange = 20f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float spawnOffset = 1f;
    public bool invertDirection = false;

    [Header("Animation")]
    public Animator animator;

    [Header("Audio")]
    public AudioClip fireSound; // Sound for firing a projectile
    private AudioSource audioSource;

    private float fireRateTimer = 0f;
    private bool isAggroed = false;
    private bool hasFiredInitially = false;
    private List<GameObject> activeProjectiles = new List<GameObject>();
    private Collider2D turretCollider;

    void Start()
    {
        turretCollider = GetComponent<Collider2D>();

        if (focalPoint == null) Debug.LogError($"{gameObject.name}: Focal point is not assigned!");
        if (projectilePrefab == null || projectileSpawnPoint == null) Debug.LogError($"{gameObject.name}: Projectile prefab or spawn point is not assigned!");
        if (animator == null) Debug.LogError($"{gameObject.name}: Animator reference is not assigned!");
        if (fireSound == null) Debug.LogError($"{gameObject.name}: Fire sound not assigned!");

        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>(); // Add an AudioSource if not found

        UpdatePlayerReference(); // Try to assign player on start
    }

    void Update()
    {
        if (player == null)
        {
            UpdatePlayerReference(); // Reacquire player if missing
            return;
        }

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

        if (isAggroed)
        {
            if (!hasFiredInitially)
            {
                FireProjectile();
                hasFiredInitially = true;
            }
            HandleProjectileFiring();
        }

        UpdateBlendTreeValues();
    }

    private void UpdatePlayerReference()
    {
        var playerObj = FindObjectOfType<Player>();
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log($"{gameObject.name}: Player reference re-assigned at runtime.");
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

        TurretProjectile projScript = projectile.GetComponent<TurretProjectile>();
        if (projScript != null)
        {
            projScript.Initialize(this, direction);
        }
        else
        {
            Debug.LogError("Projectile script not found on instantiated projectile.");
        }

        activeProjectiles.Add(projectile);
        TriggerFireAnimation(direction);

        // Play the fire sound when a projectile is fired
        PlayFireSound();
    }

    private void TriggerFireAnimation(Vector3 direction)
    {
        animator.SetTrigger("FireTrigger");
    }

    private void UpdateBlendTreeValues()
    {
        Vector3 direction = (focalPoint.position - transform.position).normalized;
        animator.SetFloat("DirectionX", direction.x);
        animator.SetFloat("DirectionY", direction.y);
    }

    public void OnProjectileDestroyed(GameObject projectile)
    {
        if (activeProjectiles.Contains(projectile))
        {
            activeProjectiles.Remove(projectile);
        }
    }

    private void PlayFireSound()
    {
        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
    }
}
