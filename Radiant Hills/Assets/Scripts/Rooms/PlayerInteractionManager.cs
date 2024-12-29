using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    public static PlayerInteractionManager Instance { get; private set; } // Singleton instance

    private DisplayShelf activeDisplayShelf; // Currently active display shelf
    private Transform player; // Reference to the player

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Set the currently active display shelf
    public void SetActiveDisplayShelf(DisplayShelf shelf)
    {
        activeDisplayShelf = shelf;
        Debug.Log($"Active display shelf set to: {shelf.gameObject.name}");
    }

    // Clear the active display shelf (e.g., when exiting range)
    public void ClearActiveDisplayShelf(DisplayShelf shelf)
    {
        if (activeDisplayShelf == shelf)
        {
            activeDisplayShelf = null;
            Debug.Log("Cleared active display shelf.");
        }
    }

    // Set the player reference
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
        Debug.Log($"Player set to: {player.gameObject.name}");
    }

    // Interact with the current display shelf
    public void InteractWithActiveShelf()
    {
        if (activeDisplayShelf != null)
        {
            Debug.Log($"Interacting with {activeDisplayShelf.gameObject.name}");
            // Add interaction logic here, e.g., updating the displayed material
        }
        else
        {
            Debug.Log("No active display shelf to interact with.");
        }
    }

    // Get the current player
    public Transform GetPlayer()
    {
        return player;
    }
}
