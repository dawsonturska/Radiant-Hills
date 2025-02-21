using UnityEngine;

public class NormalMapManager : MonoBehaviour
{
    public Material characterMaterial;  // Material with the normal map shader
    public Texture2D[] runNormalMaps;   // Normal maps for the "Run" animation
    public Texture2D[] jumpNormalMaps;  // Normal maps for the "Jump" animation
    // Add more arrays for other animations (e.g. idleNormalMaps, walkNormalMaps, etc.)

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get current animation clip name
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        string animationName = currentState.IsName("Run") ? "Run" :
                               currentState.IsName("Jump") ? "Jump" : "Idle"; // etc.

        // Get the current frame based on normalized time
        int frameIndex = Mathf.FloorToInt(currentState.normalizedTime * 8) % 8;  // Assuming 8 frames per animation

        // Assign normal map based on animation and frame
        if (animationName == "Run")
        {
            characterMaterial.SetTexture("_NormalMap", runNormalMaps[frameIndex]);
        }
        else if (animationName == "Jump")
        {
            characterMaterial.SetTexture("_NormalMap", jumpNormalMaps[frameIndex]);
        }
        // Add more conditions for other animations
    }
}
