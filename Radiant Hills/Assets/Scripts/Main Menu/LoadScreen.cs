using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    public Canvas fadeCanvas;

    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndSwitch(sceneName));
    }

    private IEnumerator FadeAndSwitch(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(Fade(1f));

        // Load scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Optional: wait one frame to allow scene to settle
        yield return null;

        // Let the scene's entry point (SceneLoadTrigger) do its thing first if needed
        SceneLoadTrigger trigger = FindObjectOfType<SceneLoadTrigger>();
        if (trigger != null)
            yield return trigger.OnSceneLoaded();

        // Fade back in
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        Color color = fadeImage.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, targetAlpha);
    }
    private void Start()
    {
        // Make sure it starts transparent
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
    }
    public void StartSceneTransition(string sceneName)
    {
        StartCoroutine(FadeThenSwitch(sceneName));
    }

    private IEnumerator FadeThenSwitch(string sceneName)
    {
        yield return StartCoroutine(Fade(1f)); // Fade to black
        yield return StartCoroutine(FadeAndSwitch(sceneName)); // Load + fade in
        yield return null;
        RebindCanvasCamera();
    }

    private void RebindCanvasCamera()
    {
        if (fadeCanvas != null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                fadeCanvas.worldCamera = mainCam;
            }
        }
    }

}
