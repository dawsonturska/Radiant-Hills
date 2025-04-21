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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void InitializeReferences()
    {
        InitializePlayer();
        InitializeCameraFollow(); // Call the new method to initialize CameraFollow
        InitializeIconGrid();
        InitializeDisplayShelves();
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
                Debug.LogError("IconGrid not found in the scene!");
            }
        }
    }

    private void InitializePlayer()
    {
        player = FindObjectOfType<Player>()?.transform;


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

    private void InitializeDisplayShelves()
    {
        displayShelves.Clear();
        displayShelves.AddRange(FindObjectsOfType<DisplayShelf>());

        foreach (var shelf in displayShelves)
        {
            shelf.InitializeReferences();
            shelf.SetIconGrid(iconGrid);
            shelf.LoadStoredItem();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene '{scene.name}' loaded. Reinitializing references...");
        InitializeReferences();
    }

    // New method to initialize CameraFollow
    private void InitializeCameraFollow()
    {
        if (cameraFollow == null)
        {
            cameraFollow = FindObjectOfType<CameraFollow>();
            if (cameraFollow == null)
            {
                Debug.LogError("CameraFollow reference is missing! Ensure it is in the scene.");
            }
        }
    }
}
