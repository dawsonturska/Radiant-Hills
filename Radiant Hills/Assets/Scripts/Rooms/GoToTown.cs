using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToTown : MonoBehaviour
{
    public string newSceneName = "Town"; // Replace with desired scene name

    // Existing method to go to the shop
    public void GoTown()
    {
        Debug.Log("Going to Town now...");

        // Use the SceneLoader to handle the fade + scene load
        SceneLoader.Instance.StartSceneTransition(newSceneName);
    }

    // New method specifically for external calls, can be triggered from Dialogue system
    public void GoToTownFromDialogue()
    {
        Debug.Log("Going to the town from dialogue system...");

        // Call GoShop method when triggered
        GoTown();
    }
}