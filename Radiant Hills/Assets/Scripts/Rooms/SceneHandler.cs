using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance;
    public Inventory playerInventory;
    public IconGrid iconGrid;
    public CameraFollow cameraFollow;
    public Transform player;
    private PlayerInteractionManager playerInteractionManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the SceneHandler across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
            return;
        }

        // Initialize components
        InitializePlayer();
        InitializeCameraFollow();
        InitializeIconGrid();
        InitializePlayerInteractionManager();
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
            DontDestroyOnLoad(cameraFollow.gameObject);
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
            iconGrid = FindObjectOfType<IconGrid>();
        }

        if (iconGrid == null)
        {
            Debug.LogError("IconGrid not found in the scene.");
        }

        // After IconGrid is initialized, update all InteractionField and DisplayShelf references
        UpdateInteractionFields();
        UpdateDisplayShelves();
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
            Debug.LogWarning("Player is not set for PlayerInteractionManager.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure all InteractionFields and DisplayShelves update their reference to the IconGrid after the scene is loaded
        UpdateInteractionFields();
        UpdateDisplayShelves();
    }

    private void UpdateInteractionFields()
    {
        // Find all InteractionField objects in the scene and update their reference to IconGrid
        InteractionField[] interactionFields = FindObjectsOfType<InteractionField>();
        foreach (InteractionField field in interactionFields)
        {
            field.SetIconGrid(iconGrid); // This ensures the field has the correct IconGrid reference
        }
    }

    private void UpdateDisplayShelves()
    {
        // Find all DisplayShelf objects in the scene and update their reference to IconGrid
        DisplayShelf[] displayShelves = FindObjectsOfType<DisplayShelf>();
        foreach (DisplayShelf shelf in displayShelves)
        {
            shelf.SetIconGrid(iconGrid); // This ensures each DisplayShelf has the correct IconGrid reference
        }
    }
}
