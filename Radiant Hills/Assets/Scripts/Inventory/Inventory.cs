using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>(); // List to hold items

    // Method to add an item to the inventory
    public void AddItem(Item item)
    {
        // Check if item is stackable (e.g., potions) and already in the inventory
        Item existingItem = items.Find(i => i.itemID == item.itemID);
        if (existingItem != null)
        {
            existingItem.quantity += item.quantity; // Stack the item
        }
        else
        {
            items.Add(item); // Add new item to the inventory
        }
    }

    // Method to remove an item from the inventory
    public void RemoveItem(Item item)
    {
        Item existingItem = items.Find(i => i.itemID == item.itemID);
        if (existingItem != null)
        {
            existingItem.quantity -= item.quantity; // Decrease the quantity
            if (existingItem.quantity <= 0)
            {
                items.Remove(existingItem); // Remove item if quantity is 0 or less
            }
        }
    }

    // Method to get an item by ID
    public Item GetItem(int itemID)
    {
        return items.Find(i => i.itemID == itemID);
    }
}