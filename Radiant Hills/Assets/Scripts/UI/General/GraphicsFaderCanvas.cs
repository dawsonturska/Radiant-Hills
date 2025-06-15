using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(CanvasGroup))]
public class GraphicsFaderCanvas : MonoBehaviour
{
    private Coroutine fadeCoroutine = null;
    private CanvasGroup canvasGroup = null;

    [SerializeField] private float fadeInDuration = 0.5f;
    public float fadeInWaitDuration = 0f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    public float fadeOutWaitDuration = 0f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// FadeTurnOff: Fade out CanvasGroup given fadeOutWaitDuration and fadeOutDuration
    /// </summary>
    /// <param name="isDisable">Set true if elements should be disabled after fadeout</param>
    public void FadeTurnOff(bool isDisable)
    {
        fadeCoroutine = StartCoroutine(FadeOutElements(fadeOutDuration, fadeOutWaitDuration, isDisable));
    }

    /// <summary>
    /// FadeTurnOff: Fade in CanvasGroup given fadeInWaitDuration and fadeInDuration
    /// </summary>
    /// <param name="isDisable">Set true if elements should begin as disabled</param>
    public void FadeTurnOn(bool isDisable)
    {
        fadeCoroutine = StartCoroutine(FadeInElements(fadeInDuration, fadeInWaitDuration, isDisable));
    }

    private IEnumerator FadeInElements(float fadeDuration, float waitDuration, bool isDisable)
    {
        if (canvasGroup == null)
        {
            Debug.LogWarning("UIFader: CanvasGroup is missing!");
            yield break;
        }

        if (!canvasGroup.gameObject.activeSelf)
            canvasGroup.gameObject.SetActive(true);

        // Linq method to find all child gameObjects that have graphic components
        GameObject[] childObjects = GetComponentsInChildren<Graphic>()
            .Where(t => t != transform)
            .Select(t => t.gameObject)
            .ToArray();

        if (isDisable)
        {
            // Disable all child objects initially
            foreach (var child in childObjects)
            {
                if (child != null) child.SetActive(false);
            }
        }

        canvasGroup.alpha = 0f;

        // Wait before fading in
        yield return new WaitForSeconds(waitDuration);

        // Activate child objects at the start of fade
        foreach (var child in childObjects)
        {
            if (child != null) child.SetActive(true);
        }

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutElements(float fadeDuration, float waitDuration, bool isDisable)
    {
        if (canvasGroup == null)
        {
            Debug.LogWarning("UIFader: CanvasGroup is missing!");
            yield break;
        }

        // Linq method to find all child gameObjects that have graphic components
        GameObject[] childObjects = GetComponentsInChildren<Graphic>()
            .Where(t => t != transform)
            .Select(t => t.gameObject)
            .ToArray();

        // Wait before starting fade out
        yield return new WaitForSeconds(waitDuration);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;

        if (isDisable)
        {
            // Deactivate child objects after fade
            foreach (var child in childObjects)
            {
                if (child != null) child.SetActive(false);
            }
        }
    }


}