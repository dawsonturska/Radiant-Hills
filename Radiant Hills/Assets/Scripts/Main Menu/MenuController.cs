using UnityEngine;
using TMPro;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public GameObject logo;             // The logo GameObject
    public GameObject buttonsPanel;     // The panel containing the buttons
    public TextMeshProUGUI clickToStartText; // The "Click to Start" TextMeshPro text
    public float slideSpeed = 2f;       // Speed of the logo sliding
    public float maxDistance = 500f;    // Max distance to slide the logo (set this in the inspector)
    public float fadeSpeed = 2f;        // Speed of fading for the text

    private Vector3 logoStartPosition;
    private Vector3 logoEndPosition;
    private bool isLogoMoving = false;
    private bool isTextFadingOut = false;

    private Coroutine pulseCoroutine;

    void Start()
    {
        logoStartPosition = logo.transform.position;
        float xPosition = Mathf.Max(-Screen.width / 4, -maxDistance);
        logoEndPosition = new Vector3(xPosition, logoStartPosition.y, logoStartPosition.z);

        buttonsPanel.SetActive(false);
        clickToStartText.gameObject.SetActive(true);
        clickToStartText.alpha = 0;

        pulseCoroutine = StartCoroutine(PulseText());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isLogoMoving && !isTextFadingOut)
        {
            isTextFadingOut = true;

            if (pulseCoroutine != null)
                StopCoroutine(pulseCoroutine);

            StartCoroutine(FadeOutAndSlideLogo());
        }

        if (isLogoMoving)
        {
            logo.transform.position = Vector3.MoveTowards(logo.transform.position, logoEndPosition, slideSpeed * Time.deltaTime);

            if (logo.transform.position == logoEndPosition)
            {
                buttonsPanel.SetActive(true);
                isLogoMoving = false;
            }
        }
    }

    private IEnumerator PulseText()
    {
        float alpha = 0f;
        bool increasing = true;

        while (true)
        {
            if (increasing)
                alpha += Time.deltaTime * fadeSpeed;
            else
                alpha -= Time.deltaTime * fadeSpeed;

            clickToStartText.alpha = Mathf.Clamp01(alpha);

            if (alpha >= 1f)
                increasing = false;
            else if (alpha <= 0f)
                increasing = true;

            yield return null;
        }
    }

    private IEnumerator FadeOutAndSlideLogo()
    {
        float alpha = clickToStartText.alpha;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            clickToStartText.alpha = Mathf.Clamp01(alpha);
            yield return null;
        }

        clickToStartText.gameObject.SetActive(false);
        isLogoMoving = true;
    }
}
