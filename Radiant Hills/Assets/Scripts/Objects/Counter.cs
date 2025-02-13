using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    // Using a dictionary to track the material and its count at the counter
    private Dictionary<MaterialType, int> storedItems = new Dictionary<MaterialType, int>();

    public void ReceiveItem(MaterialType material)
    {
        if (material == null)
        {
            Debug.LogWarning("Received null material.");
            return;
        }

        // If the material is already in the dictionary, increment the count
        if (storedItems.ContainsKey(material))
        {
            storedItems[material]++;
        }
        else
        {
            // Otherwise, add the material with a count of 1
            storedItems.Add(material, 1);
        }

        Debug.Log($"Item '{material.materialName}' delivered to counter!");
    }

    public void DisplayStoredItems()
    {
        if (storedItems.Count == 0)
        {
            Debug.Log("Counter is empty.");
        }
        else
        {
            // Display all stored materials and their counts
            string itemList = "";
            foreach (var item in storedItems)
            {
                itemList += $"{item.Key.materialName} (x{item.Value}), ";
            }

            // Remove trailing comma and space
            itemList = itemList.TrimEnd(',', ' ');

            Debug.Log("Counter contains: " + itemList);
        }
    }
}
