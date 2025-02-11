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

        // IconGrid initialization no longer needs to update DisplayShelves directly
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
}
