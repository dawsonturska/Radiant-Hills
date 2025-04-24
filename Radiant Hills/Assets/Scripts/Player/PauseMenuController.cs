using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false;

    private Canvas canvas;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.enabled = false;
        }

        IsPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        IsPaused = !IsPaused;

        if (canvas != null)
        {
            canvas.enabled = IsPaused;
        }

        Time.timeScale = IsPaused ? 0 : 1;
    }

    public void Unpause()
    {
        IsPaused = false;
        if (canvas != null)
        {
            canvas.enabled = false;
        }
        Time.timeScale = 1;
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
