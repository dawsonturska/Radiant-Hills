using UnityEngine;

public class CentipedeAnimationManager : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component
    private CentipedeBehavior centipedeBehavior; // Reference to the CentipedeBehavior component

    // Animation parameters
    private static readonly int IsAggroed = Animator.StringToHash("IsAggroed");

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
            animator.SetBool(IsAggroed, centipedeBehavior.IsAggroed());
        }


    }
}
