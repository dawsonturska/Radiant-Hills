using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;  // For Unity events

public class TransitionHandler : MonoBehaviour
{
    // Define variables to be set in the inspector
    public GameObject targetObject;  // The object to toggle visibility
    public UnityEvent onSceneLoaded; // The event to trigger
    public Transform playerTransform;  // Reference to the player's transform

    private void OnEnable()
    {
        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method gets called every time a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Toggle visibility of the object
        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }

        // Trigger the UnityEvent
        if (onSceneLoaded != null)
        {
            onSceneLoaded.Invoke();
        }

        // Set the player's position to (0, 0)
        if (playerTransform != null)
        {
            playerTransform.position = new Vector3(0, 0, playerTransform.position.z);  // Assuming z doesn't change, otherwise set z too
        }

        // Reset the state for future use
        ResetForNextScene();
    }

    // Resets the script to be ready for the next scene load
    private void ResetForNextScene()
    {
        // Reset any properties, variables, or behaviors here
        // Example: if you want to reset the visibility of the target object back to its original state:
        if (targetObject != null)
        {
            targetObject.SetActive(true);  // Set to true or the desired initial state
        }
    }
}
