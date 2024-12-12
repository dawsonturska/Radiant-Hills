using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public Dictionary<MaterialType, int> materialQuantities = new Dictionary<MaterialType, int>();  // Make this public
    private List<MaterialType> inventorySlots = new List<MaterialType>(); // Tracks the order of items in the slots
    public int maxUniqueItems = 20; // Maximum number of unique items allowed in the inventory
    public int maxItemQuantityPerSlot = 16; // Max quantity per slot before taking up a new one

    public IconGrid iconGrid; // Reference to IconGrid (set this in the Inspector)
    private bool isGridOpen = false; // Track whether the grid is open or closed

    private GameObject panel; // Reference to the parent panel (the one with the grid)

    void Awake()
    {
        // Get the parent panel (this is the GameObject the IconGrid is attached to)
        if (iconGrid != null)
        {
            panel = iconGrid.transform.parent.gameObject; // Get the parent GameObject (panel)
        }
    }

    void Start()
    {
        // Set the inventory panel and IconGrid to be inactive at the start
        if (panel != null)
        {
            panel.SetActive(false); // Make the inventory panel invisible by default
        }

        if (iconGrid != null)
        {
            iconGrid.gameObject.SetActive(false); // Make the IconGrid invisible by default
        }
    }

    // Adds a material to the inventory
    public void AddMaterial(MaterialType materialType)
    {
        if (materialType == null)
        {
            Debug.LogWarning("MaterialType is null. Cannot add to inventory.");
            return;
        }

        // Check if the inventory already has the maximum number of unique items
        if (materialQuantities.Count >= maxUniqueItems && !materialQuantities.ContainsKey(materialType))
        {
            Debug.LogWarning("Inventory is full. Cannot add more unique items.");
            return;
        }

        // If the item already exists, increment its quantity, or create a new entry
        if (materialQuantities.ContainsKey(materialType))
        {
            materialQuantities[materialType]++;
        }
        else
        {
            materialQuantities[materialType] = 1;
        }

        // Rebuild the inventory slots to reflect the updated material quantities
        RebuildInventorySlots();

        // Call PopulateGrid to update the UI if iconGrid is still available
        if (iconGrid != null)
        {
            iconGrid.PopulateGrid();
        }

        Debug.Log($"Added {materialType.materialName}. New count: {materialQuantities[materialType]}");
    }

    // Rebuild the inventory slots based on the materialQuantities dictionary
    private void RebuildInventorySlots()
    {
        inventorySlots.Clear();

        foreach (var entry in materialQuantities)
        {
            MaterialType material = entry.Key;
            int quantity = entry.Value;

            // Calculate how many slots are required for the quantity
            int fullSlots = Mathf.FloorToInt((float)quantity / maxItemQuantityPerSlot);
            int remainingItems = quantity % maxItemQuantityPerSlot;

            // Fill the inventory with full slots (for each full 16 items)
            for (int i = 0; i < fullSlots; i++)
            {
                inventorySlots.Add(material);
            }

            // Add the remaining items to another slot if necessary
            if (remainingItems > 0)
            {
                inventorySlots.Add(material);
            }
        }
    }

    // Displays the current inventory
    public void DisplayInventory()
    {
        Debug.Log("Current Inventory:");

        if (materialQuantities.Count == 0)
        {
            Debug.Log("Inventory is empty.");
            return;
        }

        foreach (var entry in materialQuantities)
        {
            Debug.Log($"{entry.Key.materialName}: {entry.Value}");
        }
    }

    private void Update()
    {
        // Toggle the visibility of both the inventory panel and IconGrid on "I" key press
        if (Input.GetKeyDown(KeyCode.I))
        {
            isGridOpen = !isGridOpen; // Toggle the grid's open/close state
            Debug.Log($"Inventory Panel Open: {isGridOpen}");  // Debug to check state toggle

            // Toggle the parent panel's visibility using SetActive
            if (panel != null)
            {
                panel.SetActive(isGridOpen); // Toggle the visibility of the parent panel (the one with the grid)
            }

            // Toggle the IconGrid's visibility using SetActive
            if (iconGrid != null)
            {
                iconGrid.gameObject.SetActive(isGridOpen); // Toggle IconGrid visibility
            }
        }

        // Optionally, you can also display the inventory in the console when "I" is pressed
        if (Input.GetKeyDown(KeyCode.I))
        {
            DisplayInventory();
        }
    }
}
