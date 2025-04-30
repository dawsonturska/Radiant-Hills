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

    public Vector2 defaultSpawnPosition = new Vector2(0, 0); // Default spawn point in Inspector

    // Define a list of spawn points for specific scenes
    [System.Serializable]
    public struct SceneSpawnPoint
    {
        public string sceneName;
        public Vector2 spawnPosition;
        public string fromScene; // Optional: Can define from which scene this spawn point applies
    }

    public List<SceneSpawnPoint> sceneSpawnPoints = new List<SceneSpawnPoint>(); // List of spawn points for specific scenes
    private string previousSceneName = "";

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
        // Initialize the player and its position
        InitializePlayer();

        // Initialize all other references (Inventory, CameraFollow, IconGrid, etc.)
        InitializeCameraFollow();
        InitializeIconGrid();
        InitializeInventory();
        InitializeDisplayShelves();
    }

    private void InitializePlayer()
    {
        if (player == null)
        {
            // Find and persist player object
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

        // Get the current scene name
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Current scene: {currentSceneName}");

        // Find the appropriate spawn point for the current scene and previous scene
        Vector2 spawnPosition = defaultSpawnPosition;

        bool spawnPointFound = false; // Flag to track if spawn point is found

        foreach (var spawnPoint in sceneSpawnPoints)
        {
            if (spawnPoint.sceneName == currentSceneName &&
                (string.IsNullOrEmpty(spawnPoint.fromScene) || spawnPoint.fromScene == previousSceneName))
            {
                spawnPosition = spawnPoint.spawnPosition;
                spawnPointFound = true;
                break;
            }
        }

        // Log spawn point info
        if (spawnPointFound)
        {
            Debug.Log($"Spawn point found for scene: {currentSceneName}. Position: {spawnPosition}");
        }
        else
        {
            Debug.Log($"No specific spawn point found for scene: {currentSceneName}. Using default position.");
        }

        // Set player's position after scene load completes
        ResetPlayerPosition(spawnPosition);
    }

    private void ResetPlayerPosition(Vector2 spawnPosition)
    {
        if (player != null)
        {
            // Reset player's position explicitly, ensure Z position stays the same
            player.position = new Vector3(spawnPosition.x, spawnPosition.y, player.position.z);
            Debug.Log($"Player moved to spawn position: {spawnPosition} in scene.");
        }
    }

    private void InitializeInventory()
    {
        // Find and persist Inventory object
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<Inventory>();
            if (playerInventory != null)
            {
                DontDestroyOnLoad(playerInventory.gameObject);
                Debug.Log("Inventory found and set to persist.");
            }
            else
            {
                Debug.LogError("Inventory not found in the scene!");
            }
        }
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

        // Update previous scene name for the next transition
        previousSceneName = scene.name;
    }
}
