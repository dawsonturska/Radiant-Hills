using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{
    public string newGameSceneName = "Cutscene 1"; // Replace with desired scene name

    public void StartNewGame()
    {
        // Clear saved data
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("Starting a new game...");

        // Use the SceneLoader to handle the fade + scene load
        SceneLoader.Instance.StartSceneTransition(newGameSceneName);
    }
}
