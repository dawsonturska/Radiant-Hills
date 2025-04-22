using UnityEngine;

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
}
