using UnityEngine;

public class FishingZone : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E; // Key to press for interaction
    private bool isPlayerInZone = false; // Track if the player is inside the zone

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the zone
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            Debug.Log("Player entered the fishing zone.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player exits the zone
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            Debug.Log("Player exited the fishing zone.");
        }
    }

    void Update()
    {
        // Only allow interaction if the player is in the zone and presses the interaction key
        if (isPlayerInZone && Input.GetKeyDown(interactKey))
        {
            Debug.Log("Player pressed E to start fishing.");
            FishingManager.Instance.StartFishing();
        }
    }
}
