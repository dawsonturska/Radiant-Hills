using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InventoryItem is a pair of (name/ID, quantity)
/// </summary>
[System.Serializable]
public class InventoryItem
{
    public string materialName;
    public int quantity;
}

/// <summary>
/// Inventory is list of inventoryItems
/// </summary>
[System.Serializable]
public class InventoryData
{
    public List<InventoryItem> items;

    public InventoryData()
    {
        items = new List<InventoryItem>();
    }
}

/// <summary>
/// Serializable list of InventoryItems
/// </summary>
[System.Serializable]
public class SerializableInventory
{
    public List<InventoryItem> items = new List<InventoryItem>();
}