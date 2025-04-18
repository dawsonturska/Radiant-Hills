using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToShop : MonoBehaviour
{
    public string newSceneName = "Shop"; // Replace with desired scene name

    public void GoShop()
    {


        Debug.Log("Going to Shop now...");

        // Use the SceneLoader to handle the fade + scene load
        SceneLoader.Instance.StartSceneTransition(newSceneName);
    }
}
