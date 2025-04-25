using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public List<InventoryItem> items;

    public InventoryData()
    {
        items = new List<InventoryItem>();
    }

    [System.Serializable]
    public class InventoryItem
    {
        public string materialName;
        public int quantity;
    }
}
