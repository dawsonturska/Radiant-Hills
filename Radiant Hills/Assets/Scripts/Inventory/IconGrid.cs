using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class IconGrid : MonoBehaviour
{
    public GameObject itemIconPrefab; // Prefab for the item icon (button with text)
    public GameObject emptySlotPrefab; // Prefab for the empty slot (used for empty slots)
    public Transform gridContainer; // Parent container for the icons
    public Inventory inventory; // Reference to the Inventory script
    public int maxSlots = 20; // Maximum number of slots

    private Queue<GameObject> iconPool = new Queue<GameObject>(); // Pool for item icons
    private Queue<GameObject> emptySlotPool = new Queue<GameObject>(); // Pool for empty slots
    public bool IsGridPopulated { get; private set; } = false; // Track if the grid has been populated

    void Start()
    {
        // Ensure the inventory reference is set
        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>(); // Find Inventory in the scene if not set
        }

        // Initially populate the grid with empty slots
        PopulateGrid();
    }

    // Populates the grid with item icons
    public void PopulateGrid()
    {
        // Debugging: Log the number of materials in the inventory
        Debug.Log("Populating grid with " + inventory.materialQuantities.Count + " materials");

        // Clear existing icons or empty slots
        ClearGrid();

        // Add new item icons based on the inventory materials
        int slotIndex = 0;
        foreach (var entry in inventory.materialQuantities)
        {
            MaterialType material = entry.Key;
            int quantity = entry.Value;

            // Create or reuse an icon for this material
            GameObject icon = GetIconFromPool();
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

                // You can optionally add functionality here, such as:
                // iconButton.onClick.AddListener(() => OnItemClicked(material)); 
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
            GameObject emptySlot = GetEmptySlotFromPool();
            emptySlot.SetActive(true); // Activate the empty slot
            slotIndex++;
        }

        // Set the grid as populated
        IsGridPopulated = true;
    }

    private void ClearGrid()
    {
        // Disable all existing icons and empty slots before populating the grid
        foreach (Transform child in gridContainer)
        {
            child.gameObject.SetActive(false);
        }

        // Mark the grid as not populated
        IsGridPopulated = false;
    }

    // Reuses an icon from the pool, or instantiates one if none are available
    private GameObject GetIconFromPool()
    {
        if (iconPool.Count > 0)
        {
            GameObject icon = iconPool.Dequeue();
            icon.SetActive(true);
            return icon;
        }
        else
        {
            // Instantiate a new icon if the pool is empty
            return Instantiate(itemIconPrefab, gridContainer);
        }
    }

    // Reuses an empty slot from the pool, or instantiates one if none are available
    private GameObject GetEmptySlotFromPool()
    {
        if (emptySlotPool.Count > 0)
        {
            GameObject emptySlot = emptySlotPool.Dequeue();
            emptySlot.SetActive(true);
            return emptySlot;
        }
        else
        {
            // Instantiate a new empty slot if the pool is empty
            return Instantiate(emptySlotPrefab, gridContainer);
        }
    }

    // Optional: Handle item button click (if you want to add functionality to clicking an item)
    private void OnItemClicked(MaterialType material)
    {
        // Add custom behavior when an item is clicked, such as using the item
        Debug.Log("Item clicked: " + material.name);
    }

    // New UpdateUI method to refresh the UI
    public void UpdateUI()
    {
        PopulateGrid(); // Re-populate the grid with updated inventory data
    }
}
