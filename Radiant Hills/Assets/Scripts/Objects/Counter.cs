using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    private Dictionary<MaterialType, int> storedItems = new Dictionary<MaterialType, int>();

    public BoxCollider2D Register; // Interaction area for collecting items (set to trigger)
    public GameObject player; // Reference to the player
    public MaterialType buttonsMaterial; // Reference to the "Buttons" material

    private bool playerInTrigger = false; // Tracks if player is inside the trigger area

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        if (Register == null)
        {
            Debug.LogError("Register collider is not assigned!");
        }
        else if (!Register.isTrigger)
        {
            Debug.LogWarning("Register BoxCollider2D should be set to 'Is Trigger'!");
        }

        if (buttonsMaterial == null)
        {
            Debug.LogError("Buttons material is not assigned! Assign it in the Inspector.");
        }
    }

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed E inside the counter trigger zone.");
            CollectItems();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player entered the counter trigger zone.");
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player exited the counter trigger zone.");
            playerInTrigger = false;
        }
    }

    public void ReceiveItem(MaterialType material)
    {
        if (material == null)
        {
            Debug.LogWarning("Received null material.");
            return;
        }

        if (storedItems.ContainsKey(material))
        {
            storedItems[material]++;
        }
        else
        {
            storedItems.Add(material, 1);
        }

        Debug.Log($"Item '{material.materialName}' delivered to counter! Current quantity: {storedItems[material]}");
    }

    private void CollectItems()
    {
        if (storedItems.Count == 0)
        {
            Debug.Log("Counter is empty. No items to collect.");
            return;
        }

        Inventory playerInventory = player.GetComponent<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogError("Player does not have an Inventory component.");
            return;
        }

        int totalButtons = 0;

        foreach (var item in storedItems)
        {
            MaterialType material = item.Key;
            int quantity = item.Value;

            Debug.Log($"Processing {quantity}x {material.materialName}, Price per item: {material.price}");

            int buttonAmount = Mathf.FloorToInt(material.price * quantity); // Convert float price to int
            totalButtons += buttonAmount;

            Debug.Log($"Converted {quantity}x {material.materialName} into {buttonAmount} Buttons.");
        }

        if (totalButtons > 0)
        {
            if (buttonsMaterial == null)
            {
                Debug.LogError("Buttons material is not assigned in the Inspector!");
                return;
            }

            Debug.Log($"Attempting to add {totalButtons} Buttons to the player's inventory...");
            playerInventory.AddMaterial(buttonsMaterial, totalButtons);
            Debug.Log($"Successfully added {totalButtons} Buttons to the player's inventory.");

            storedItems.Clear();
        }
        else
        {
            Debug.LogWarning("No Buttons were added. Check if materials have a valid price.");
        }
    }

    public void DisplayStoredItems()
    {
        if (storedItems.Count == 0)
        {
            Debug.Log("Counter is empty.");
        }
        else
        {
            string itemList = "";
            foreach (var item in storedItems)
            {
                itemList += $"{item.Key.materialName} (x{item.Value}), ";
            }

            itemList = itemList.TrimEnd(',', ' ');

            Debug.Log("Counter contains: " + itemList);
        }
    }
}
