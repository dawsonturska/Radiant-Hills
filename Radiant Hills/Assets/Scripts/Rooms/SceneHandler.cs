using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance;
    public Inventory playerInventory;
    public IconGrid iconGrid;
    public CameraFollow cameraFollow;
    public Transform player; // Reference to the player

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the scene handler across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }

        // Make sure the player reference is always assigned
        if (player == null)
        {
            player = FindObjectOfType<Player>().transform; // Find player if not set
        }

        if (cameraFollow == null)
        {
            cameraFollow = FindObjectOfType<CameraFollow>();
        }

        if (cameraFollow != null)
        {
            DontDestroyOnLoad(cameraFollow.gameObject); // Keep the camera follow across scenes
        }
        else
        {
            Debug.LogError("CameraFollow reference is missing in SceneHandler.");
        }

        if (iconGrid == null)
        {
            iconGrid = FindObjectOfType<IconGrid>(); // Find the IconGrid object in the scene
        }

        if (player != null)
        {
            DontDestroyOnLoad(player.gameObject); // Ensure player is persistent across scenes
        }
        else
        {
            Debug.LogError("Player reference is missing in SceneHandler.");
        }

        // Ensure inventory and icon grid references are set up for player
        if (playerInventory != null && iconGrid != null)
        {
            playerInventory.iconGrid = iconGrid; // Assign iconGrid to playerInventory
            playerInventory.SetPlayer(player); // Ensure inventory knows about the player
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene load events
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe when the object is destroyed
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure the camera follows the player after the scene is loaded
        if (cameraFollow != null && player != null)
        {
            cameraFollow.SetTarget(player);
        }
        else
        {
            Debug.LogWarning("CameraFollow or player reference is missing in OnSceneLoaded.");
        }

        // Ensure player reference is set after scene load
        if (player == null)
        {
            player = FindObjectOfType<Player>().transform; // Find player in the scene if missing
            if (player == null)
            {
                Debug.LogError("Player not found in the scene after loading.");
            }
        }

        // Reassign the inventory and icon grid if needed
        if (iconGrid != null && playerInventory != null)
        {
            playerInventory.iconGrid = iconGrid; // Ensure iconGrid is properly linked to playerInventory
            playerInventory.SetPlayer(player); // Ensure the player's reference is set in Inventory
        }

        // Handle the "Shop" scene differently
        if (scene.name == "Shop")
        {
            // Handle any additional behavior specific to the Shop scene here
            // For example, you can update UI or handle the inventory UI differently
            if (iconGrid != null)
            {
                iconGrid.PopulateGrid(); // Corrected to call PopulateGrid
            }
        }
        else
        {
            // For non-Shop scenes, ensure the grid is populated
            if (iconGrid != null)
            {
                iconGrid.PopulateGrid(); // Corrected to call PopulateGrid
            }
        }

        // Update Sorting Orders for all relevant objects in the scene
        UpdateSortingOrders();
    }

    private void UpdateSortingOrders()
    {
        SortingOrderAdjuster[] allAdjusters = FindObjectsOfType<SortingOrderAdjuster>();

        foreach (SortingOrderAdjuster adjuster in allAdjusters)
        {
            if (adjuster != null)
            {
                adjuster.UpdateSortingOrder(); // Update sorting order for each adjuster
            }
        }
    }
}
