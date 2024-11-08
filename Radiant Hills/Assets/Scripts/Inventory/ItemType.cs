using UnityEngine;

public enum ItemType { Weapon, Armor, Potion, Misc } // Example item types

[System.Serializable]
public class Item
{
    public string itemName;
    public ItemType itemType;
    public int itemID;  // A unique ID for each item
    public Sprite itemIcon;  // A UI icon for the item
    public int quantity; // If stackable, this is how many are in the stack

    // Constructor for creating an item
    public Item(string name, ItemType type, int id, Sprite icon, int qty = 1)
    {
        itemName = name;
        itemType = type;
        itemID = id;
        itemIcon = icon;
        quantity = qty;
    }
}