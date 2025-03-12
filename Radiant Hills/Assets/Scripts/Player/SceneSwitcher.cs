using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchOnKeyPress : MonoBehaviour
{
    public string sceneForNumpad0; // Scene to load when Numpad 0 is pressed
    public string sceneForNumpad1; // Scene to load when Numpad 1 is pressed

    void Update()
    {
        // Check if the player presses Numpad 0
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            LoadScene(sceneForNumpad0);
        }

        // Check if the player presses Numpad 1
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            LoadScene(sceneForNumpad1);
        }
    }

    private void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName); // Load the scene
        }
        else
        {
            Debug.LogError("Scene name is not set in the SceneSwitchOnKeyPress script!");
        }
    }
}
