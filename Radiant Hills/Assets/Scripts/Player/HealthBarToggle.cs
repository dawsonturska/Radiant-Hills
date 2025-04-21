using UnityEngine;
using UnityEngine.SceneManagement; // Needed to check the scene name
using UnityEngine.UI;              // Required if you're using UI.Image

public class HealthBarVisibility : MonoBehaviour
{
    public GameObject healthBar;       // Reference to the health bar GameObject
    public GameObject extraImage;      // Reference to the extra image GameObject

    void Start()
    {
        if (healthBar == null)
        {
            Debug.LogError("HealthBar reference not set!");
        }

        if (extraImage == null)
        {
            Debug.LogError("ExtraImage reference not set!");
        }

        // Initially hide both in the editor
#if UNITY_EDITOR
        if (healthBar != null) healthBar.SetActive(false);
        if (extraImage != null) extraImage.SetActive(false);
#endif
    }

    void Update()
    {
        bool isGatherScene = SceneManager.GetActiveScene().name == "Gather";

        if (Application.isPlaying)
        {
            if (healthBar != null) healthBar.SetActive(isGatherScene);
            if (extraImage != null) extraImage.SetActive(isGatherScene);
        }
    }
}
