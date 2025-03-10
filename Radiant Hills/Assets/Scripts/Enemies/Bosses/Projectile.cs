using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Properties")]
    public float lifetime = 5f;
    public float damage = 10f;
    public float moveSpeed = 5f;

    [Header("References")]
    public Transform boss;
    public CentipedeBehavior centipedeBehavior;

    private Transform player;
    private bool isLaunched = false;
    private bool isReflected = false;
    private bool hasHitBoss = false; // Flag to prevent multiple hits on the boss
    private Rigidbody2D rb;
    private Collider2D projectileCollider;
    private Vector3 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();

        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}: Rigidbody2D component is missing!");
        }
    }

    void Start()
    {
        if (boss != null)
        {
            centipedeBehavior = boss.GetComponent<CentipedeBehavior>();
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError($"{gameObject.name}: Player not found in the scene!");
        }

        if (boss != null)
        {
            Collider2D bossCollider = boss.GetComponent<Collider2D>();
            if (bossCollider != null)
            {
                IgnoreBossCollider(bossCollider, true); // Ignore collisions with the boss initially
            }
        }

        Invoke(nameof(DestroyProjectile), lifetime);
    }

    public void SetDirection(Vector3 direction)
    {
        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}: Rigidbody2D is null when setting direction!");
            return;
        }

        moveDirection = direction.normalized;
        rb.velocity = moveDirection * moveSpeed;
        isLaunched = true;
    }

    void Update()
    {
        if (isLaunched && rb != null && rb.velocity.magnitude < moveSpeed)
        {
            rb.velocity = moveDirection * moveSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerWeapon") && !isReflected)
        {
            ReflectProjectileBackToBoss();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Apply damage to player
            }
            DestroyProjectile(); // Destroy the projectile after collision with player
        }
        else if (collision.gameObject.CompareTag("Boss") && isReflected && !hasHitBoss)
        {
            // When the reflected projectile hits the boss
            Debug.Log("Reflected projectile hit the boss!");

            if (centipedeBehavior != null)
            {
                Debug.Log($"Applying {damage} damage to the centipede.");
                centipedeBehavior.TakeDamage(damage); // Apply damage to the boss
                centipedeBehavior.TeleportToRandomLocation(); // Teleport the boss after damage
            }
            hasHitBoss = true; // Prevent further collisions with the boss
            DestroyProjectile(); // Destroy the projectile after hitting the boss
        }
        else
        {
            // Destroy the projectile on other collisions (non-reflected or non-boss collision)
            DestroyProjectile();
        }
    }

    private void ReflectProjectileBackToBoss()
    {
        isReflected = true;
        Vector3 directionToBoss = (boss.position - transform.position).normalized;
        rb.velocity = directionToBoss * moveSpeed * 2f; // Double speed for reflected projectile

        if (boss != null)
        {
            Collider2D bossCollider = boss.GetComponent<Collider2D>();
            if (bossCollider != null)
            {
                IgnoreBossCollider(bossCollider, false); // Stop ignoring collisions with the boss after reflection
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
        if (centipedeBehavior != null)
        {
            centipedeBehavior.OnProjectileDestroyed(gameObject); // Notify the centipede behavior
        }

        Destroy(gameObject); // Destroy the projectile
    }
}
