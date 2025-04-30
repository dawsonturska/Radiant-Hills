using UnityEngine;

public class BossMusicController : MonoBehaviour
{
    public AudioClip defaultClip;     // Music before aggro
    public AudioClip aggroClip;       // Music when boss is aggroed

    public CentipedeBehavior centipede; // Reference to CentipedeBehavior script
    private AudioSource audioSource;

    private bool isAggroMusicPlaying = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing!");
            return;
        }

        if (defaultClip == null || aggroClip == null)
        {
            Debug.LogError("One or more audio clips are not assigned!");
            return;
        }

        // Play the default music on start
        audioSource.clip = defaultClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    void Update()
    {
        if (centipede == null) return;

        if (centipede.IsAggroed() && !isAggroMusicPlaying)
        {
            SwitchToAggroMusic();
        }
    }

    private void SwitchToAggroMusic()
    {
        isAggroMusicPlaying = true;
        audioSource.Stop();
        audioSource.clip = aggroClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Call this on death to switch back to default music
    public void OnBossDeath()
    {
        isAggroMusicPlaying = false;

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = defaultClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
