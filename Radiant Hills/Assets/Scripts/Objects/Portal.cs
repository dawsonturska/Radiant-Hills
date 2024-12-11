using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string targetScene; // Name of the scene to load
    public float holdDuration = 3f; // Time in seconds the player needs to hold the key
    private float holdTimer = 0f; // Tracks how long the player has been holding the key
    private bool isPlayerInRange = false; // Tracks if the player is in the portal's range

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            holdTimer = 0f; // Reset the timer when the player leaves the portal
        }
    }

    private void Update()
    {
        if (isPlayerInRange)
        {
            if (Input.GetKey(KeyCode.E))
            {
                holdTimer += Time.deltaTime; // Increment the timer

                if (holdTimer >= holdDuration)
                {
                    TeleportPlayer();
                }
            }
            else
            {
                holdTimer = 0f; // Reset the timer if the key is released
            }
        }
    }

    private void TeleportPlayer()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.LoadScene(targetScene); // Load the target scene
        }
        else
        {
            Debug.LogError("Target scene is not set in the Portal script!");
        }
    }
}
