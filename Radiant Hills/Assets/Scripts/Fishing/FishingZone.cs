using UnityEngine;

public class FishingZone : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E; // Key to press for interaction
    public GameObject indicatorPrefab; // The prefab to display when the player enters the fishing zone

    private bool isPlayerInZone = false; // Track if the player is inside the zone
    private GameObject activeIndicator; // Reference to the active indicator instance

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the zone
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            ShowIndicator();
            Debug.Log("Player entered the fishing zone.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player exits the zone
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            HideIndicator();
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

    private void ShowIndicator()
    {
        if (indicatorPrefab != null && activeIndicator == null) // Ensure no duplicate indicators
        {
            activeIndicator = Instantiate(indicatorPrefab, transform.position + Vector3.up, Quaternion.identity);
            activeIndicator.transform.SetParent(transform); // Make the indicator a child of the fishing zone
        }
    }

    private void HideIndicator()
    {
        if (activeIndicator != null)
        {
            Destroy(activeIndicator);
        }
    }
}
