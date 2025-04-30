using UnityEngine;

public class CentipedeAnimationHandler : MonoBehaviour
{
    public Animator animator; // Reference to the Centipede's Animator
    public Transform player; // Reference to the player
    public float aggroRange = 20f; // Aggro range to check if player is within range
    private CentipedeBehavior centipedeBehavior; // Reference to the CentipedeBehavior script

    private bool isAggroed = false; // Track if the centipede is aggroed

    void Start()
    {
        centipedeBehavior = GetComponent<CentipedeBehavior>(); // Get the CentipedeBehavior script

        if (animator == null)
        {
            Debug.LogError("Animator reference is not assigned!");
        }
        if (player == null)
        {
            Debug.LogError("Player reference is not assigned!");
        }
        if (centipedeBehavior == null)
        {
            Debug.LogError("CentipedeBehavior reference is not assigned!");
        }
    }

    void Update()
    {
        if (player == null || animator == null || centipedeBehavior == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isAggroed = distanceToPlayer <= aggroRange;

        if (isAggroed && centipedeBehavior.IsAggroed())
        {
            // Trigger spit animation if the cooldown allows and it has not been triggered already
            animator.SetTrigger("Spit");
        }

        // Fire projectile when animation reaches the desired point (e.g., halfway through the "Spit" animation)
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Spit") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
        {
            centipedeBehavior.FireProjectileAtPlayer(); // Delegate firing to CentipedeBehavior
        }

        UpdateFacingDirection();
    }

    private void UpdateFacingDirection()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }
}
