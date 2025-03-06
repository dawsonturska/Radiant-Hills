using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f; // Time until the projectile is destroyed
    public float damage = 10f; // Damage dealt by the projectile
    public float moveSpeed = 5f; // Speed of the projectile
    public Transform boss; // Reference to the boss (could also be set via Inspector)
    public CentipedeBehavior centipedeBehavior; // Reference to CentipedeBehavior

    private Transform player;
    private bool isLaunched = false; // If the projectile is moving
    private bool isReflected = false; // If the projectile is reflected back to the boss
    private Rigidbody2D rb;
    private Collider2D projectileCollider;
    public TurretLogic turretLogic; // Reference to TurretLogic for the callback
    private bool isTurretProjectile = false; // Flag to check if the projectile was fired by the turret

    void Start()
    {
        if (boss == null)
        {
            // Attempt to get the CentipedeBehavior if not manually assigned
            boss = transform.root; // Assuming the boss is the parent of the projectile
            centipedeBehavior = boss.GetComponent<CentipedeBehavior>();

            if (centipedeBehavior == null)
            {
                Debug.LogError("CentipedeBehavior not found on the boss.");
                return;
            }
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene!");
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();

        // Ignore collision with the boss collider immediately
        if (boss != null)
        {
            Collider2D bossCollider = boss.GetComponent<Collider2D>();
            if (bossCollider != null)
            {
                IgnoreBossCollider(bossCollider, true);
            }
        }

        // Schedule destruction after the projectile's lifetime
        Invoke(nameof(DestroyProjectile), lifetime);

        if (turretLogic != null)
        {
            // Use the fire direction defined by the turret logic
            isTurretProjectile = true; // Set the flag to indicate turret launch
            LaunchProjectile(turretLogic.fireDirection);
        }
    }

    void Update()
    {
        // No need for homing logic if fired from the turret
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Only trigger the reflection if not fired by a turret
        if (!isTurretProjectile && collision.gameObject.CompareTag("PlayerWeapon") && !isReflected)
        {
            ReflectProjectileBackToBoss();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply damage to the player
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            DestroyProjectile();
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            if (isReflected)
            {
                // Logic for hitting the boss after reflection
                Debug.Log("Reflected projectile hit the boss!");

                // Apply the damage to the boss
                CentipedeBehavior centipedeBehavior = collision.gameObject.GetComponent<CentipedeBehavior>();
                if (centipedeBehavior != null)
                {
                    // Apply the damage to the boss
                    centipedeBehavior.TakeDamage((int)damage); // Assuming the damage is castable to int
                }

                // Notify the CentipedeBehavior to teleport
                centipedeBehavior.TeleportToRandomLocation();

                DestroyProjectile();
            }
            else
            {
                // If the projectile hasn't been reflected yet, destroy it without damaging the boss
                DestroyProjectile();
            }
        }
        else
        {
            DestroyProjectile();
        }
    }

    private void LaunchProjectile(Vector2 fireDirection)
    {
        // Set the projectile's velocity based on the fire direction from the turret
        rb.velocity = fireDirection * moveSpeed;
        isLaunched = true; // Mark the projectile as launched
    }

    private void ReflectProjectileBackToBoss()
    {
        isReflected = true;

        // Change direction towards the boss
        Vector3 directionToBoss = (boss.position - transform.position).normalized;
        if (rb != null)
        {
            rb.velocity = directionToBoss * moveSpeed * 2f; // Reflected speed is doubled
        }

        // Allow collision with the boss during reflection
        if (boss != null)
        {
            Collider2D bossCollider = boss.GetComponent<Collider2D>();
            if (bossCollider != null)
            {
                IgnoreBossCollider(bossCollider, false);
            }
        }

        Debug.Log("Projectile reflected back to the boss!");
    }

    public void IgnoreBossCollider(Collider2D bossCollider, bool ignore)
    {
        if (projectileCollider != null && bossCollider != null)
        {
            Physics2D.IgnoreCollision(projectileCollider, bossCollider, ignore);
        }
    }

    private void DestroyProjectile()
    {
        // Notify the turret that the projectile was destroyed
        if (turretLogic != null)
        {
            turretLogic.OnProjectileDestroyed(gameObject);
        }

        // Notify the boss that the projectile was destroyed
        if (centipedeBehavior != null)
        {
            centipedeBehavior.OnProjectileDestroyed(gameObject);
        }

        Destroy(gameObject);
    }
}