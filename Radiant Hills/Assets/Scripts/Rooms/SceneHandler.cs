using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (cameraFollow == null)
        {
            cameraFollow = FindObjectOfType<CameraFollow>();
        }

        if (cameraFollow != null)
        {
            DontDestroyOnLoad(cameraFollow.gameObject);
        }
        else
        {
            Debug.LogError("CameraFollow reference is missing in SceneHandler.");
        }

        if (iconGrid == null)
        {
            iconGrid = FindObjectOfType<IconGrid>();
        }

        if (player != null)
        {
            DontDestroyOnLoad(player.gameObject); // Ensure player is persistent
        }
        else
        {
            Debug.LogError("Player reference is missing in SceneHandler.");
        }

        if (playerInventory != null && iconGrid != null)
        {
            playerInventory.iconGrid = iconGrid;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
            player = FindObjectOfType<Player>().transform; // Ensure player is found dynamically if missing
            if (player == null)
            {
                Debug.LogError("Player not found in the scene after loading.");
            }
        }

        // Reassign the inventory and icon grid if needed
        if (iconGrid != null && playerInventory != null)
        {
            playerInventory.iconGrid = iconGrid;
        }

        // Optionally: Refresh the UI
        if (iconGrid != null)
        {
            iconGrid.UpdateUI();
        }

        // Update Sorting Orders
        UpdateSortingOrders();
    }

    private void UpdateSortingOrders()
    {
        SortingOrderAdjuster[] allAdjusters = FindObjectsOfType<SortingOrderAdjuster>();

        foreach (SortingOrderAdjuster adjuster in allAdjusters)
        {
            if (adjuster != null)
            {
                adjuster.UpdateSortingOrder();
            }
        }
    }
}
