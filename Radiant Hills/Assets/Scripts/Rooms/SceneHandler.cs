using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance;
    public Inventory playerInventory;
    public IconGrid iconGrid;
    public CameraFollow cameraFollow;
    public Transform player; // Reference to the player
    private PlayerInteractionManager playerInteractionManager; // Reference to PlayerInteractionManager

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
            return;
        }

        // Initialize essential components
        InitializePlayer();
        InitializeCameraFollow();
        InitializeIconGrid();
        InitializePlayerInteractionManager();
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
            player = FindObjectOfType<Player>()?.transform; // Find player in the scene if missing
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
            if (iconGrid != null)
            {
                iconGrid.PopulateGrid();
            }
        }
        else
        {
            // For non-Shop scenes, ensure the grid is populated
            if (iconGrid != null)
            {
                iconGrid.PopulateGrid();
            }
        }

        // Update Sorting Orders for all relevant objects in the scene
        UpdateSortingOrders();

        // Ensure PlayerInteractionManager is linked and functioning in the new scene
        InitializePlayerInteractionManager();
    }

    private void InitializePlayer()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>()?.transform; // Find player if not set
        }

        if (player != null)
        {
            DontDestroyOnLoad(player.gameObject); // Ensure player is persistent across scenes
        }
        else
        {
            Debug.LogError("Player reference is missing in SceneHandler.");
        }
    }

    private void InitializeCameraFollow()
    {
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
    }

    private void InitializeIconGrid()
    {
        if (iconGrid == null)
        {
            iconGrid = FindObjectOfType<IconGrid>(); // Find the IconGrid object in the scene
        }
    }

    private void InitializePlayerInteractionManager()
    {
        if (playerInteractionManager == null)
        {
            playerInteractionManager = FindObjectOfType<PlayerInteractionManager>();
        }

        if (playerInteractionManager == null)
        {
            GameObject managerObject = new GameObject("PlayerInteractionManager");
            playerInteractionManager = managerObject.AddComponent<PlayerInteractionManager>();
            DontDestroyOnLoad(managerObject); // Persist the manager across scenes
        }

        // Ensure the manager is aware of the current player
        if (player != null)
        {
            playerInteractionManager.SetPlayer(player);
        }
        else
        {
            Debug.LogWarning("Player is not set for PlayerInteractionManager.");
        }
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
