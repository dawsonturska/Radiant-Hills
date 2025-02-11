using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public GameObject displayShelfPrefab; // Prefab for the display shelf
    public Inventory inventory; // Reference to the player's inventory
    public IconGrid iconGrid; // Reference to the icon grid

    void Start()
    {
        // Check if inventory is not assigned and try to find it in the scene
        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>(); // Find the inventory if not set in Inspector
        }

        // Check if iconGrid is not assigned and try to find it in the scene
        if (iconGrid == null)
        {
            iconGrid = FindObjectOfType<IconGrid>(); // Find the icon grid if not set in Inspector
        }
    }

    // Method to create and initialize a display shelf at a specific position
    public DisplayShelf CreateDisplayShelf(Vector3 position)
    {
        // Instantiate the display shelf prefab at the specified position and rotation
        GameObject shelfInstance = Instantiate(displayShelfPrefab, position, Quaternion.identity);

        // Get the DisplayShelf component from the instantiated object
        DisplayShelf shelfScript = shelfInstance.GetComponent<DisplayShelf>();

        // Check if the DisplayShelf script was found and initialize it with inventory and icon grid
        if (shelfScript != null)
        {
            shelfScript.Initialize(); // Initialize the display shelf
        }
        else
        {
            Debug.LogError("Failed to find DisplayShelf component on the instantiated prefab.");
        }

        return shelfScript; // Return the initialized DisplayShelf instance
    }
}
