using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToDungeon : MonoBehaviour
{
    public string newSceneName = "Gather"; // Scene to load

    public void GoDungeon()
    {
        Debug.Log("Going to the Meek Woods now...");

        // Use the SceneLoader to handle the fade + scene load
        SceneLoader.Instance.StartSceneTransition(newSceneName);
    }

    public void GoToDungeonFromDialogue()
    {
        Debug.Log("Going to the Meek Woods from dialogue system...");
        GoDungeon();
    }
}
