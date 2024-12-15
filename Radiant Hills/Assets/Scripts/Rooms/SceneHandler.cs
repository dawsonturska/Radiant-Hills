using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the event when this object is disabled
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SecondScene") // Replace with your actual second scene name
        {
            Debug.Log("Second scene loaded. Looking for player...");
            GameObject player = GameObject.FindWithTag("Player"); // Find player in the new scene
            if (player != null)
            {
                Debug.Log("Player found in new scene.");
            }
            else
            {
                Debug.LogWarning("Player not found in new scene.");
            }
        }
    }
}
