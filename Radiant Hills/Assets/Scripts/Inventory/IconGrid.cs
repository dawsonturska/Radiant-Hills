using UnityEngine;
using UnityEngine.UI;
using TMPro; // If you use TextMeshPro for text components

public class IconGrid : MonoBehaviour
{
    public GameObject itemIconPrefab; // Prefab for the item icon (button with text)
    public GameObject emptySlotPrefab; // Prefab for the empty slot (used for empty slots)
    public Transform gridContainer; // Parent container for the icons
    public Inventory inventory; // Reference to the Inventory script
    public int maxSlots = 20; // Maximum number of slots

    void Start()
    {
        // Ensure the inventory reference is set
        if (inventory == null)
        {
            inventory = GetComponentInParent<Inventory>(); // Find Inventory in parent if not set
        }

        // Initially populate the grid with empty slots
        PopulateGrid();
    }

    // Populates the grid with item icons
    public void PopulateGrid()
    {
        // Debugging: Log the number of materials in the inventory
        Debug.Log("Populating grid with " + inventory.materialQuantities.Count + " materials");

        // Clear any existing icons or empty slots in the grid
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject); // Destroy existing child objects (icons or empty slots)
        }

        // Add new item icons based on the inventory materials
        int slotIndex = 0;
        foreach (var entry in inventory.materialQuantities)
        {
            MaterialType material = entry.Key;
            int quantity = entry.Value;

            // Create a new icon for this material (button prefab)
            GameObject icon = Instantiate(itemIconPrefab, gridContainer);
            TextMeshProUGUI textMeshProComponent = icon.GetComponentInChildren<TextMeshProUGUI>();

            if (textMeshProComponent != null)
            {
                // Update the quantity text inside the button
                textMeshProComponent.text = quantity.ToString();
            }

            // Optionally, update the button's image or other properties based on the material
            Button iconButton = icon.GetComponent<Button>();
            if (iconButton != null)
            {
                // Optionally, set the button's image to the material's icon if you have one
                Image iconImage = icon.GetComponent<Image>();
                if (iconImage != null)
                {
                    if (material.icon != null)
                    {
                        iconImage.sprite = material.icon; // Ensure material.icon is assigned correctly
                    }
                    else
                    {
                        Debug.LogWarning("Material icon is missing for " + material.name);
                    }
                }
            }

            slotIndex++; // Increment the slot index for the next material

            // Stop if we've exceeded the maximum slots allowed
            if (slotIndex >= maxSlots)
            {
                break;
            }
        }

        // If there are still empty slots left, add them to the grid
        while (slotIndex < maxSlots)
        {
            Instantiate(emptySlotPrefab, gridContainer); // Create an empty slot for each remaining slot
            slotIndex++;
        }
    }
}
