using UnityEngine;
using UnityEngine.SceneManagement;

public class ContextAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip gatherClip;
    public AudioClip bossClip;

    [Header("Audio Settings")]
    public float fadeDuration = 1.5f;

    private AudioSource audioSource;
    private Coroutine currentFade;
    private string currentContext = "";

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on this GameObject.");
            return;
        }

        audioSource.clip = gatherClip;
        audioSource.loop = true;
        audioSource.Play();
        currentContext = "Gather";
    }

    void Update()
    {
        string context = DetermineContext();

        if (context != currentContext)
        {
            SwitchContextAudio(context);
            currentContext = context;
        }
    }

    string DetermineContext()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // Example conditions; adjust as needed
        bool isInGatherScene = sceneName == "Gather";
        bool isFightingBoss = BossFightManager.Instance != null && BossFightManager.Instance.IsBossFightActive;

        if (isFightingBoss)
            return "Boss";
        if (isInGatherScene)
            return "Gather";

        return "Unknown";
    }

    void SwitchContextAudio(string context)
    {
        AudioClip newClip = null;

        switch (context)
        {
            case "Boss":
                newClip = bossClip;
                break;
            case "Gather":
                newClip = gatherClip;
                break;
        }

        if (newClip != null && newClip != audioSource.clip)
        {
            if (currentFade != null) StopCoroutine(currentFade);
            currentFade = StartCoroutine(FadeToClip(newClip));
        }
    }

    System.Collections.IEnumerator FadeToClip(AudioClip newClip)
    {
        float startVolume = audioSource.volume;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = startVolume;
    }
}
