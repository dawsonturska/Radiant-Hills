using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsFader : MonoBehaviour
{
    private Coroutine fadeCoroutine = null;

    [SerializeField] private float fadeInDuration = 0.5f;
    public float fadeInWaitDuration = 0f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    public float fadeOutWaitDuration = 0f;

    public void FadeTurnOff()
    {
        if (fadeCoroutine == null) fadeCoroutine = StartCoroutine(FadeOutElements(fadeOutDuration, fadeOutWaitDuration));
    }

    public void FadeTurnOn()
    {
        if (fadeCoroutine == null) fadeCoroutine = StartCoroutine(FadeInElements(fadeInDuration, fadeInWaitDuration));
    }

    private IEnumerator FadeOutElements(float fadeDuration, float waitDuration)
    {
        yield return new WaitForSeconds(waitDuration);
        // Get all UI components under the parent
        Graphic[] graphics = GetComponentsInChildren<Graphic>();

        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);

            foreach (var graphic in graphics)
            {
                if (graphic != null)
                {
                    Color color = graphic.color;
                    color.a = alpha;
                    graphic.color = color;
                }
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        foreach (var graphic in graphics)
        {
            if (graphic != null)
            {
                Color color = graphic.color;
                color.a = 0;
                graphic.color = color;
                graphic.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator FadeInElements(float fadeDuration, float waitDuration)
    {
        // Get all UI components under the parent
        Graphic[] graphics = GetComponentsInChildren<Graphic>();

        foreach (var graphic in graphics)
        {
            if (graphic != null)
            {
                Color color = graphic.color;
                color.a = 1;
                graphic.color = color;
                graphic.gameObject.SetActive(false);
            }
        }

        yield return new WaitForSeconds(waitDuration);

        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration);

            foreach (var graphic in graphics)
            {
                if (graphic != null)
                {
                    graphic.gameObject.SetActive(true);
                    Color color = graphic.color;
                    color.a = alpha;
                    graphic.color = color;
                }
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        foreach (var graphic in graphics)
        {
            if (graphic != null)
            {
                Color color = graphic.color;
                color.a = 1;
                graphic.color = color;
                graphic.gameObject.SetActive(true);
            }
        }
    }
}