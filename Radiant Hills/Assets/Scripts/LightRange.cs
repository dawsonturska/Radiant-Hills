using UnityEngine;


[RequireComponent(typeof(UnityEngine.Rendering.Universal.Light2D), typeof(Collider2D))]
public class LightRangeTrigger : MonoBehaviour
{
    public float targetIntensity = 4f; // Intensity when player is nearby
    public float defaultIntensity = 2.25f; // Intensity when player is away
    public float fadeSpeed = 2f; // How fast the light fades in/out

    private UnityEngine.Rendering.Universal.Light2D light2D;
    private bool playerInRange = false;

    void Start()
    {
        light2D = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        light2D.intensity = defaultIntensity;
    }

    void Update()
    {
        float target = playerInRange ? targetIntensity : defaultIntensity;
        light2D.intensity = Mathf.Lerp(light2D.intensity, target, Time.deltaTime * fadeSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
