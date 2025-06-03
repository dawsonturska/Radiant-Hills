using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines interactions for Register object, 
/// </summary>
public class Register : MonoBehaviour, IInteractable
{
    public GameObject player; // Reference to the player

    [Tooltip("Reference to the \"Buttons\" material")]
    public MaterialType buttonsMaterial;

    [Header("Audio")]
    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip buttonAddedClip; // The audio clip to play when buttons are added

    // List of stored items
    private Dictionary<MaterialType, int> storedItems = new Dictionary<MaterialType, int>();
    // Tracks if player is inside the trigger area
    private bool playerInTrigger = false;

    private void Start()
    {
        FindPlayerReference();

        if (buttonsMaterial == null)
        {
            Debug.LogError("Buttons material is not assigned! Assign it in the Inspector.");
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource not found! Make sure the AudioSource component is attached.");
            }
        }

        if (buttonAddedClip == null)
        {
            Debug.LogError("Button Added Audio Clip is not assigned! Assign it in the Inspector.");
        }
    }

    private void OnEnable()
    {
        FindPlayerReference();
    }

    private void FindPlayerReference()
    {
        if (player == null)
        {
            GameObject found = GameObject.FindWithTag("Player");
            if (found != null)
            {
                player = found;
                Debug.Log("Player reference updated dynamically.");
            }
        }
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

            int buttonAmount = Mathf.FloorToInt(material.price * quantity);
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

            // Play the button added audio clip
            if (audioSource != null && buttonAddedClip != null)
            {
                audioSource.PlayOneShot(buttonAddedClip);
            }

            storedItems.Clear();
        }
        else
        {
            Debug.LogWarning("No Buttons were added. Check if materials have a valid price.");
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

    public void SetPlayerInTrigger(bool value) { playerInTrigger = value; }

    public void DisplayStoredItems()
    {
        if (storedItems.Count == 0)
        {
            Debug.Log("Register is empty.");
        }
        else
        {
            string itemList = "";
            foreach (var item in storedItems)
            {
                itemList += $"{item.Key.materialName} (x{item.Value}), ";
            }

            itemList = itemList.TrimEnd(',', ' ');

            Debug.Log("Register contains: " + itemList);
        }
    }

    /// <summary>
    /// Handler for "Interact" actions
    /// </summary>
    public void Interact(PlayerInputHandler handler)
    {
        if (player == null)
        {
            FindPlayerReference(); // Keep looking until player is found
        } 
        else if (playerInTrigger)
        {
            Debug.Log("Player pressed E inside the counter trigger zone.");
            CollectItems();
        }
    }
}