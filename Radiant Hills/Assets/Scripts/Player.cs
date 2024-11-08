using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory playerInventory; // Reference to the player's inventory
    public InventoryUI inventoryUI;  // Reference to the InventoryUI

    void Start()
    {
        // Ensure the Inventory component is attached to the player
        playerInventory = GetComponent<Inventory>();
        if (playerInventory == null)
        {
            playerInventory = gameObject.AddComponent<Inventory>(); // Add Inventory if not already attached
        }

        // Ensure the InventoryUI component is found in the scene
        inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI not found in the scene!");
        }
    }

    void Update()
    {
        // Example use-case: Add an HP Pot (Health Potion) to the inventory when pressing the "H" key
        if (Input.GetKeyDown(KeyCode.H))
        {
            // Load the HP Potion icon (ensure this is in the Resources folder as "HP_Potion_Icon")
            Sprite hpPotionIcon = Resources.Load<Sprite>("HP_Potion_Icon");

            // Create the HP Pot item and add it to the inventory
            Item hpPot = new Item("HP Pot", ItemType.Potion, 1, hpPotionIcon, 1);
            playerInventory.AddItem(hpPot);
            inventoryUI.UpdateInventoryUI();  // Refresh the UI after adding the item
            Debug.Log("HP Pot added to inventory");
        }

        // Example use-case: Remove a health potion from the inventory when pressing the "R" key
        if (Input.GetKeyDown(KeyCode.R))
        {
            Item healthPotion = playerInventory.GetItem(1); // Assuming potion ID is 1
            if (healthPotion != null)
            {
                playerInventory.RemoveItem(healthPotion);
                inventoryUI.UpdateInventoryUI();  // Refresh the UI after removal
                Debug.Log("Health Potion removed from inventory");
            }
        }
    }
}