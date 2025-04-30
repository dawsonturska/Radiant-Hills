using UnityEngine;

public class CentipedeAnimationManager : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component
    private CentipedeBehavior centipedeBehavior; // Reference to the CentipedeBehavior component

    // Animation parameters
    private static readonly int IsAggroed = Animator.StringToHash("IsAggroed");
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int IsFiring = Animator.StringToHash("IsFiring");

    void Start()
    {
        // Get the Animator and CentipedeBehavior components
        animator = GetComponent<Animator>();
        centipedeBehavior = GetComponent<CentipedeBehavior>();

        if (animator == null)
        {
            Debug.LogError("Animator is missing on the centipede!");
        }

        if (centipedeBehavior == null)
        {
            Debug.LogError("CentipedeBehavior is missing!");
        }
    }

    void Update()
    {
        // Check if the centipede is aggroed and update the animator
        if (centipedeBehavior != null)
        {
            // Update aggro state
            animator.SetBool(IsAggroed, centipedeBehavior.IsAggroed());

            // Update firing state
            animator.SetBool(IsFiring, centipedeBehavior.IsAggroed() && centipedeBehavior.CanFire());

            // Calculate movement direction
            Vector2 direction = centipedeBehavior.GetDirectionToPlayer();
            animator.SetFloat(MoveX, direction.x);
            animator.SetFloat(MoveY, direction.y);
        }
    }
}
