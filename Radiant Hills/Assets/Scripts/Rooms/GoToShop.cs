using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToShop : MonoBehaviour
{
    public string newSceneName = "Shop"; // Replace with desired scene name

    // Existing method to go to the shop
    public void GoShop()
    {
        Debug.Log("Going to Shop now...");

        // Use the SceneLoader to handle the fade + scene load
        SceneLoader.Instance.StartSceneTransition(newSceneName);
    }

    // New method specifically for external calls, can be triggered from Dialogue system
    public void GoToShopFromDialogue()
    {
        Debug.Log("Going to the shop from dialogue system...");

        // Call GoShop method when triggered
        GoShop();
    }
}