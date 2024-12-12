using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI Image

public class IconGrid : MonoBehaviour
{
    public GameObject slotPrefab; // The prefab for each inventory slot
    public Transform gridContainer; // Container for the grid slots
    public List<InventorySlot> inventorySlots = new List<InventorySlot>(); // Holds all the slots
    public Inventory inventory; // Reference to the player's inventory

    void Start()
    {
        PopulateGrid();
    }

    // This method will be used to update the grid
    public void PopulateGrid()
    {
        // Clear the grid
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        // Add the slots
        foreach (var entry in inventory.materialQuantities)
        {
            MaterialType materialType = entry.Key;
            int quantity = entry.Value;

            // Create a new slot for this item
            GameObject slot = Instantiate(slotPrefab, gridContainer);
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            inventorySlot.materialType = materialType; // Assign the material type
            inventorySlot.quantity = quantity; // Assign the quantity
            inventorySlot.UpdateSlot(); // Update the icon and overlay
        }
    }
}
