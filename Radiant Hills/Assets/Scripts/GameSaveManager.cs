using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    public DayCycleManager dayCycleManager; // Reference to the DayCycleManager
    public Inventory inventory;             // Reference to the Inventory
    private bool isGridOpen = false;        // Whether the inventory grid is visible

    // Ensure the GameSaveManager is not destroyed when switching scenes
    private void Awake()
    {
        if (dayCycleManager == null || inventory == null)
        {
            Debug.LogError("GameSaveManager is missing references to DayCycleManager or Inventory.");
        }
    }

    void Update()
    {
        // Check for Numpad 7 (Save Game)
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            SaveGame();
            Debug.Log("Game Saved.");
        }

        // Check for Numpad 8 (Load Game)
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            LoadGame();
            Debug.Log("Game Loaded.");
        }

        // Check for Numpad 9 (Advance Day)
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            AdvanceDay();
            Debug.Log("Day Advanced.");
        }


    }

    // Method to save the game state (day and inventory)
    public void SaveGame()
    {
        dayCycleManager.SaveDay();      // Save the current day and time of day
        inventory.SaveInventory();      // Save the player's inventory
        Debug.Log("Game saved.");
    }

    // Method to load the game state (day and inventory)
    public void LoadGame()
    {
        dayCycleManager.LoadDay();      // Load the saved day and time of day
        inventory.LoadInventory();      // Load the player's inventory
        Debug.Log("Game loaded.");
    }

    // Method to advance the day
    public void AdvanceDay()
    {
        dayCycleManager.AdvanceTime();  // Advance the day and update time of day
    }
}

