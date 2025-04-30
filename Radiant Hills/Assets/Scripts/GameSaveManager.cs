using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSaveManager : MonoBehaviour
{
    private static GameSaveManager instance;

    private void Awake()
    {
        // Singleton pattern to avoid duplicates
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad7)) SaveGame();
        if (Input.GetKeyDown(KeyCode.Keypad8)) LoadGame();
    }

    public void SaveGame()
    {
        Transform playerTransform = GameObject.FindWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogWarning("SaveGame: Player not found.");
            return;
        }

        // Save the current scene and player position
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("PlayerX", playerTransform.position.x);
        PlayerPrefs.SetFloat("PlayerY", playerTransform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", playerTransform.position.z);

        // Save day cycle and inventory data
        GameObject.FindObjectOfType<DayCycleManager>()?.SaveDay();
        GameObject.FindObjectOfType<Inventory>()?.SaveInventory(); // This will save the inventory data to a file

        Debug.Log("Game saved.");
    }

    public void LoadGame()
    {
        string sceneName = PlayerPrefs.GetString("LastScene", SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        else
        {
            StartCoroutine(DelayedRestore());
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOp.isDone)
        {
            yield return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedRestore());
    }

    private IEnumerator DelayedRestore()
    {
        yield return new WaitUntil(() =>
            GameObject.FindWithTag("Player") != null &&
            GameObject.FindObjectOfType<Inventory>() != null &&
            GameObject.FindObjectOfType<DayCycleManager>() != null);

        // Restore the player's position based on saved data, handled by SceneHandler
        Debug.Log($"Game state restored.");
    }
}
