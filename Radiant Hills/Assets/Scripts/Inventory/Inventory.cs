using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Terrain;

public class Inventory : MonoBehaviour
{
    private Dictionary<MaterialType, int> materialQuantities = new Dictionary<MaterialType, int>();

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
        // Check for "I" key to display inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            DisplayInventory();
        }
    }
}