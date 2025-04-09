using UnityEngine;

public class ExitGame : MonoBehaviour
{
    // Call this method to exit the game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        // This line is only for use inside the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
