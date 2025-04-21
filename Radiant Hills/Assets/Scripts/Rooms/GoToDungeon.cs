using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToDungeon : MonoBehaviour
{
    public string newSceneName = "Gather"; // Replace with desired scene name

    // Existing method to go to the shop
    public void GoDungeon()
    {
        Debug.Log("Going to the Meek Woods now...");

        // Use the SceneLoader to handle the fade + scene load
        SceneLoader.Instance.StartSceneTransition(newSceneName);
    }

    // New method specifically for external calls, can be triggered from Dialogue system
    public void GoToDungeonFromDialogue()
    {
        Debug.Log("Going to the Meek Woods from dialogue system...");

        // Call GoDungeon method when triggered
        GoDungeon();
    }
}