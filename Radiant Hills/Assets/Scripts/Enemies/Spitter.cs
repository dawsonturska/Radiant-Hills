using UnityEngine;
using System.Collections;

public class Spitter : Enemy
{
    public float runAwayDistance = 7f; // Distance at which the spitter starts running away
    public float spittingCooldown = 2f; // Time between each spit
    public GameObject projectilePrefab; // The projectile prefab for spitting
    private bool isSpitting = false; // Flag for controlling the spitting behavior
    public Vector2 projectileOffset = new Vector2(0.5f, 0f); // Adjust as needed

    // Audio stuff
    public AudioClip spitSound; // Sound for spitting
    public AudioClip runAwaySound; // Sound for running away
    public AudioClip hitSound; // Sound when the spitter gets hit
    private AudioSource audioSource; // Audio source to play sounds

    void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>(); // Get the audio source component
    }

    void Update()
    {
        base.Update();

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Start spitting projectiles if far enough from the player
            if (!isSpitting && distanceToPlayer > runAwayDistance)
            {
                StartCoroutine(SpitProjectiles());
            }
        }
    }

    private void RunAwayFromPlayer()
    {
        Vector3 direction = (transform.position - player.transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Play the run-away sound when moving away from the player
        if (audioSource != null && runAwaySound != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(runAwaySound);
        }
    }

    private IEnumerator SpitProjectiles()
    {
        isSpitting = true;

        while (Vector3.Distance(transform.position, player.transform.position) > runAwayDistance)
        {
            FireSpitProjectile();
            yield return new WaitForSeconds(spittingCooldown);
        }

        isSpitting = false;
    }

    private void FireSpitProjectile()
    {
        if (projectilePrefab != null)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            Vector3 spawnPos = transform.position + (Vector3)(directionToPlayer * projectileOffset.magnitude);
            GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = directionToPlayer * 5f; // Adjust speed as needed
            }

            // Play the spit sound
            if (audioSource != null && spitSound != null)
            {
                audioSource.PlayOneShot(spitSound);
            }
        }
    }

    // Override TakeDamage to handle damage and play hit sound
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        // Play the hit sound when the spitter is damaged
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Add any additional logic for Spitter damage handling
    }

    protected override void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < runAwayDistance)
        {
            // Run away
            Vector3 direction = (transform.position - player.transform.position).normalized;

            // Prioritize cardinal direction movement like base class
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                direction = direction.x > 0 ? Vector3.right : Vector3.left;
            }
            else
            {
                direction = direction.y > 0 ? Vector3.up : Vector3.down;
            }

            bool isMoving = direction.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);

            if (isMoving)
            {
                animator.SetFloat("MoveX", direction.x);
                animator.SetFloat("MoveY", direction.y);
            }

            transform.position += direction * moveSpeed * Time.deltaTime;

            // Play the run-away sound when the spitter starts moving away
            if (audioSource != null && runAwaySound != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(runAwaySound);
            }
        }
        else
        {
            // Too far to flee, stand still or prepare to spit
            animator.SetBool("IsMoving", false);
        }
    }
}
