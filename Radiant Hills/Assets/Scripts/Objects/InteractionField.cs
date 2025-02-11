using UnityEngine;
using UnityEngine.Events;

public class InteractionField : MonoBehaviour
{
    public UnityEvent onInteract;
    public GameObject slot1;
    public GameObject inventoryPanel;

    private bool isPlayerInRange = false;
    private IconGrid iconGrid;
    private Inventory inventory;

    public void Initialize(Inventory assignedInventory, IconGrid assignedIconGrid)
    {
        inventory = assignedInventory;
        iconGrid = assignedIconGrid;
        Debug.Log("InteractionField initialized with Inventory and IconGrid.");
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }

        if (inventoryPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }
    }

    private void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            bool isOpen = inventoryPanel.activeSelf;
            inventoryPanel.SetActive(!isOpen);
            Debug.Log(isOpen ? "Inventory Closed." : "Inventory Opened.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CloseInventory();
        }
    }

    private void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    // **Added Missing SetIconGrid() Method**
    public void SetIconGrid(IconGrid grid)
    {
        iconGrid = grid;
    }
}
