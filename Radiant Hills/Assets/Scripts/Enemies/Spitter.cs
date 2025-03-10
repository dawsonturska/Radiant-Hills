using UnityEngine;
using System.Collections;

public class Spitter : Enemy
{
    public float runAwayDistance = 7f; // Distance at which the spitter starts running away
    public float spittingCooldown = 2f; // Time between each spit
    public GameObject projectilePrefab; // The projectile prefab for spitting
    private bool isSpitting = false; // Flag for controlling the spitting behavior

    void Start()
    {
        base.Start();
    }

    void Update()
    {
        base.Update();

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Run away from the player when they are too close
            if (distanceToPlayer < runAwayDistance)
            {
                RunAwayFromPlayer();
            }

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
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = directionToPlayer * 5f; // Adjust speed as needed
            }

            // Optionally, you can add logic to reflect the projectile back at the spitter
        }
    }

    // Override TakeDamage to handle damage
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        // Add any additional logic for Spitter damage handling
    }
}
