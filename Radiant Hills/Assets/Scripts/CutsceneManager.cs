using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public Image cutsceneImage;
    public Sprite image1;
    public Sprite image2;
    public Sprite image3;

    public float fadeDuration = 1f; // Duration of the fade in seconds

    void Start()
    {
        cutsceneImage.sprite = image1;
        cutsceneImage.color = new Color(1f, 1f, 1f, 1f); // Fully visible
    }

    public void OnEvent1()
    {
        StartCoroutine(FadeToSprite(image2));
    }

    public void OnEvent2()
    {
        StartCoroutine(FadeToSprite(image3));
    }

    private IEnumerator FadeToSprite(Sprite newSprite)
    {
        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = 1f - (t / fadeDuration);
            cutsceneImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        // Switch sprite
        cutsceneImage.sprite = newSprite;

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = t / fadeDuration;
            cutsceneImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        // Ensure fully visible at the end
        cutsceneImage.color = new Color(1f, 1f, 1f, 1f);
    }
}
