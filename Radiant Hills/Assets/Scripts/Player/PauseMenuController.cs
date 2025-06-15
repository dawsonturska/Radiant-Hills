using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    // Static bool that all objects can reference to know whether game is paused
    public static bool IsPaused { get; private set; } = false;

    [Tooltip("First button to select when opening pause menu")]
    [SerializeField] private GameObject firstSelectedPauseMenuButton;

    // Action triggered when game unpaused
    public event Action OnUnpause;

    private Canvas pauseMenuCanvas;

    void Awake()
    {
        pauseMenuCanvas = GetComponent<Canvas>();
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.enabled = false;
        }

        IsPaused = false;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        IsPaused = true;
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.enabled = true;
            EventSystemSelectHelper.SetSelectedGameObject(firstSelectedPauseMenuButton);
        }
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        IsPaused = false;
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.enabled = false;
            EventSystemSelectHelper.SetSelectedGameObject(null);
        }
        Time.timeScale = 1;
        OnUnpause?.Invoke();
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;

        if (SceneLoader.Instance != null)
        {
            StartCoroutine(FadeOutDeleteAndLoadMenu());
        }
        else
        {
            // Fallback if SceneLoader is not available
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Destroy(player);
            }
            SceneManager.LoadScene("MainMenu");
        }
    }

    public System.Collections.IEnumerator FadeOutDeleteAndLoadMenu()
    {
        // Fade to black first
        yield return SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.Fade(1f));

        // Destroy the player
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }

        // Load MainMenu with fade-in
        yield return SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.FadeAndSwitch("MainMenu"));
    }
}
