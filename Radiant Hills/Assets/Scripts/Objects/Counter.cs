using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    private Dictionary<MaterialType, int> storedItems = new Dictionary<MaterialType, int>();

    public GameObject player; // Reference to the player
    public MaterialType buttonsMaterial; // Reference to the "Buttons" material

    private bool playerInTrigger = false; // Tracks if player is inside the trigger area

    // New fields for audio
    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip buttonAddedClip; // The audio clip to play when buttons are added

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
    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed E inside the counter trigger zone.");
            CollectItems();
        }
        if (player == null)
        {
            FindPlayerReference(); // Keep looking until player is found
        }
    }

    public void SetPlayerInTrigger(bool state)
    {
        playerInTrigger = state;
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
}
