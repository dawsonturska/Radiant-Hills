using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance;
    public Inventory playerInventory;
    public IconGrid iconGrid;
    public CameraFollow cameraFollow;
    public Transform player;
    private PlayerInteractionManager playerInteractionManager;
    private SortingOrderAdjuster sortingOrderAdjuster;
    private List<DisplayShelf> displayShelves = new List<DisplayShelf>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeReferences();
        SceneManager.sceneLoaded += OnSceneLoaded; // Update references when a new scene is loaded
    }

    private void InitializeReferences()
    {
        InitializePlayer();
        InitializeCameraFollow();
        InitializeIconGrid();
        InitializePlayerInteractionManager();
        InitializeSortingOrderAdjuster();
        InitializeDisplayShelves();
    }

    private void InitializePlayer()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>()?.transform;
        }

        if (player != null)
        {
            DontDestroyOnLoad(player.gameObject); // Keep the player across scenes
        }
        else
        {
            Debug.LogError("Player not found! Ensure a player object is present.");
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
            Debug.LogError("CameraFollow not found! Ensure a CameraFollow object is present.");
        }
    }

    private void InitializeIconGrid()
    {
        if (iconGrid == null)
        {
            iconGrid = FindObjectOfType<IconGrid>();
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
            DontDestroyOnLoad(managerObject);
        }

        if (player != null)
        {
            playerInteractionManager.SetPlayer(player);
        }
        else
        {
            Debug.LogError("Player is not set in PlayerInteractionManager.");
        }
    }

    private void InitializeSortingOrderAdjuster()
    {
        sortingOrderAdjuster = FindObjectOfType<SortingOrderAdjuster>();

        if (sortingOrderAdjuster != null)
        {
            sortingOrderAdjuster.player = player?.gameObject;
            sortingOrderAdjuster.UpdateCustomerList(); // Ensure customers update
        }
        else
        {
            Debug.LogError("SortingOrderAdjuster not found! Ensure a SortingOrderAdjuster object is present.");
        }
    }

    private void InitializeDisplayShelves()
    {
        displayShelves.Clear();
        displayShelves.AddRange(FindObjectsOfType<DisplayShelf>());

        foreach (var shelf in displayShelves)
        {
            if (shelf == null) continue; // Prevent accessing a destroyed shelf

            shelf.InitializeReferences();
            shelf.LoadStoredItem(); // Load the shelf data

            // Assign IconGrid reference if it exists
            if (iconGrid != null)
            {
                shelf.SetIconGrid(iconGrid);
            }
        }
    }

    private void InitializeEnemySorting()
    {
        var enemySorters = FindObjectsOfType<EnemySorting>();
        foreach (var sorter in enemySorters)
        {
            if (sorter != null)
            {
                sorter.UpdateSorting();
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this == null) return; // Ensure SceneHandler isn't destroyed

        InitializeReferences(); // Refresh references after scene loads
        InitializeDisplayShelves(); // Reinitialize display shelves
        InitializeEnemySorting(); // Update enemy sorting

        if (playerInventory != null)
        {
            playerInventory.LoadInventory();
        }
        else
        {
            Debug.LogError("Player inventory is not set in SceneHandler.");
        }
    }
}
