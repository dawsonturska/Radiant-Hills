using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public delegate void SlotClickHandler(int slotIndex); // Delegate for slot click events
    public event SlotClickHandler OnSlotClicked; // Event to subscribe to slot clicks

    public Dictionary<MaterialType, int> materialQuantities = new Dictionary<MaterialType, int>(); // Inventory items
    public IconGrid iconGrid; // Reference to the IconGrid (set this in the Inspector)
    private GameObject panel; // Reference to the parent panel (where IconGrid is)
    private bool isGridOpen = false; // Whether the grid is open or closed

    private Transform player; // Reference to the player object

    void Awake()
    {
        // Singleton pattern to ensure only one instance of Inventory exists
        if (Instance == null)
        {
            Instance = this; // Set this as the instance
            DontDestroyOnLoad(gameObject); // Prevent destruction on scene load
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }

        // Ensure IconGrid is not destroyed on scene load
        if (iconGrid != null)
        {
            DontDestroyOnLoad(iconGrid.gameObject);
        }
    }

    void Start()
    {
        // Ensure the panel and IconGrid are inactive at the start
        if (panel != null)
        {
            panel.SetActive(false); // Make the inventory panel invisible by default
        }

        if (iconGrid != null)
        {
            iconGrid.gameObject.SetActive(false); // Make the IconGrid invisible by default
        }
    }

    void Update()
    {
        // Check for the I key press to toggle inventory visibility
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventoryVisibility();
        }
    }

    // Toggle visibility of the inventory grid
    public void ToggleInventoryVisibility()
    {
        isGridOpen = !isGridOpen;

        if (panel != null)
        {
            panel.SetActive(isGridOpen); // Toggle visibility of the inventory panel
        }

        if (iconGrid != null)
        {
            iconGrid.gameObject.SetActive(isGridOpen); // Toggle visibility of the IconGrid
        }
    }

    // Property to check if the inventory is visible
    public bool IsInventoryVisible
    {
        get { return isGridOpen; }
    }

    // Set the player reference in the inventory
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    // Add material to the inventory
    public void AddMaterial(MaterialType materialType, int quantity)
    {
        if (materialQuantities.ContainsKey(materialType))
        {
            materialQuantities[materialType] += quantity;
        }
        else
        {
            materialQuantities[materialType] = quantity;
        }

        // Refresh the grid after adding material
        if (iconGrid != null)
        {
            iconGrid.PopulateGrid();  // Calls the PopulateGrid method to update the inventory UI
        }
    }
}
