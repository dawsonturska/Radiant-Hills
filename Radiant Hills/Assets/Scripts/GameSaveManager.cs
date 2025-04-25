using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSaveManager : MonoBehaviour
{
    private void Awake()
    {
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
        if (playerTransform == null) return;

        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("PlayerX", playerTransform.position.x);
        PlayerPrefs.SetFloat("PlayerY", playerTransform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", playerTransform.position.z);

        GameObject.FindObjectOfType<DayCycleManager>()?.SaveDay();
        GameObject.FindObjectOfType<Inventory>()?.SaveInventory();

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
        while (!asyncOp.isDone) yield return null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedRestore());
    }

    private IEnumerator DelayedRestore()
    {
        // Wait until systems are initialized
        yield return new WaitUntil(() =>
            GameObject.FindWithTag("Player") != null &&
            GameObject.FindObjectOfType<Inventory>() != null &&
            GameObject.FindObjectOfType<DayCycleManager>() != null);

        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        DayCycleManager dayCycle = GameObject.FindObjectOfType<DayCycleManager>();
        Inventory inventory = GameObject.FindObjectOfType<Inventory>();
        IconGrid iconGrid = GameObject.FindObjectOfType<IconGrid>();

        dayCycle.LoadDay();
        inventory.LoadInventory();
        iconGrid.PopulateGrid();

        float x = PlayerPrefs.GetFloat("PlayerX", playerTransform.position.x);
        float y = PlayerPrefs.GetFloat("PlayerY", playerTransform.position.y);
        float z = PlayerPrefs.GetFloat("PlayerZ", playerTransform.position.z);
        playerTransform.position = new Vector3(x, y, z);

        Debug.Log("Game state restored. Player position: " + playerTransform.position);
    }
}
