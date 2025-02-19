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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeReferences();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void InitializeReferences()
    {
        Debug.Log("Initializing references...");

        InitializePlayer();
        InitializeCameraFollow();
        InitializeIconGrid();
        InitializePlayerInteractionManager();
        InitializeSortingOrderAdjuster();
        InitializeDisplayShelves();
        InitializeEnemySorting();
    }

    private void InitializeIconGrid()
    {
        if (iconGrid == null)
        {
            iconGrid = FindObjectOfType<IconGrid>();
            if (iconGrid != null)
            {
                DontDestroyOnLoad(iconGrid.gameObject);
                Debug.Log("IconGrid found and set to persist.");
            }
            else
            {
                Debug.LogError("IconGrid not found! Ensure it is in the scene.");
            }
        }
    }

    private void InitializePlayer()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>()?.transform;
        }

        if (player != null)
        {
            DontDestroyOnLoad(player.gameObject);
            Debug.Log("Player found and set to persist.");
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
            DontDestroyOnLoad(cameraFollow.gameObject);
            Debug.Log("CameraFollow found and set to persist.");
        }
        else
        {
            Debug.LogError("CameraFollow not found! Ensure it is in the scene.");
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
            Debug.Log("PlayerInteractionManager created and set to persist.");
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
            sortingOrderAdjuster.SetPlayer(player?.gameObject);
            sortingOrderAdjuster.UpdateCustomerList();
            Debug.Log("SortingOrderAdjuster found and updated.");
        }
        else
        {
            Debug.LogError("SortingOrderAdjuster not found! Ensure it is in the scene.");
        }
    }

    private void InitializeDisplayShelves()
    {
        displayShelves.Clear();
        displayShelves.AddRange(FindObjectsOfType<DisplayShelf>());

        Debug.Log($"Found {displayShelves.Count} DisplayShelves in the scene.");

        foreach (var shelf in displayShelves)
        {
            if (shelf == null) continue;

            shelf.InitializeReferences();
            shelf.LoadStoredItem();

            if (iconGrid != null)
            {
                shelf.SetIconGrid(iconGrid);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene '{scene.name}' loaded. Reinitializing references...");

        InitializeReferences();
        InitializeDisplayShelves();
        InitializeEnemySorting();

        if (playerInventory != null)
        {
            playerInventory.LoadInventory();
        }
        else
        {
            Debug.LogError("Player inventory is not set in SceneHandler.");
        }
    }

    private void InitializeEnemySorting()
    {
        var enemySorters = FindObjectsOfType<EnemySorting>();

        Debug.Log($"Found {enemySorters.Length} enemy sorters in the scene.");

        foreach (var sorter in enemySorters)
        {
            if (sorter != null)
            {
                sorter.SetPlayer(player?.gameObject);
                sorter.UpdateSorting();
            }
        }
    }
}
