using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    private Dictionary<MaterialType, int> materialQuantities = new Dictionary<MaterialType, int>();

    public IconGrid iconGrid; // Reference to IconGrid (set this in the Inspector)
    private bool isGridOpen = false; // Track whether the grid is open or closed

    private GameObject panel; // Reference to the parent panel

    void Awake()
    {
        // Get the parent panel (this is the GameObject the IconGrid is attached to)
        if (iconGrid != null)
        {
            panel = iconGrid.transform.parent.gameObject; // Get the parent GameObject (panel)
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

        if (materialQuantities.ContainsKey(materialType))
        {
            materialQuantities[materialType]++;
        }
        else
        {
            materialQuantities[materialType] = 1;
        }

        Debug.Log($"Added {materialType.materialName}. New count: {materialQuantities[materialType]}");

        // Call PopulateGrid to update the UI if iconGrid is still available
        if (iconGrid != null)
        {
            iconGrid.PopulateGrid();
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
        // Toggle the visibility of the IconGrid panel and IconGrid on 'I' key press
        if (Input.GetKeyDown(KeyCode.I))
        {
            isGridOpen = !isGridOpen; // Toggle the grid's open/close state
            Debug.Log($"Icon Grid Open: {isGridOpen}");  // Debug to check state toggle

            // Toggle the parent panel's visibility using SetActive
            if (panel != null)
            {
                panel.SetActive(isGridOpen); // Toggle the visibility of the parent panel
            }

            // Toggle the IconGrid's visibility using SetActive
            if (iconGrid != null)
            {
                iconGrid.gameObject.SetActive(isGridOpen); // Toggle IconGrid visibility
            }
        }

        // Check for "I" key to display inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            DisplayInventory();
        }
    }
}
