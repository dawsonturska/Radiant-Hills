using System.Collections;
using UnityEngine;

/// <summary>
/// Interact Indicator, derived from IndicatorGUI
/// </summary>
public class IndicatorGUI_Interact : IndicatorGUI
{
    private Animator animator;

    private void Awake()
    {
        // Set icon automatically if null
        if (indicatorIcon == null)
            indicatorIcon = GetComponentInChildren<SpriteRenderer>();

        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Show Indicator
    /// </summary>
    /// <param name="onComplete"> Callback that is triggered when ShowIndicator is complete. </param>
    public override void ShowIndicator(System.Action onComplete = null) 
    {
        indicatorIcon.gameObject.SetActive(true);
        animator.SetTrigger("Appear");
        if (onComplete != null) onComplete();
    }

    /// <summary>
    /// Hide Indicator
    /// </summary>
    /// <param name="onComplete"> Callback that is triggered when HideIndicator is complete.</param>
    public override void HideIndicator(System.Action onComplete = null) 
    {
        if (onComplete != null) StartCoroutine(DisappearThenCallback(onComplete));
    }

    // Callback after completing disappear animation
    private IEnumerator DisappearThenCallback(System.Action onComplete)
    {
        animator.SetTrigger("Disappear");
        // Wait until state has finished playing
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        indicatorIcon.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}