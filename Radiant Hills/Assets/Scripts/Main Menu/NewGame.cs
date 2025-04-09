using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{
    // The name or index of the scene to load for a new game
    public string newGameSceneName = "Cutscene 1"; // Replace with your actual scene name

    public void StartNewGame()
    {
        // Clear all saved data (or you can selectively reset keys if needed)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("Starting a new game...");

        // Load the game scene
        SceneManager.LoadScene(newGameSceneName);
    }
}
