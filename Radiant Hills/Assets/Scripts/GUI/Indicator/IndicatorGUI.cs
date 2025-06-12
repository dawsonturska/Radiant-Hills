using UnityEngine;

/// <summary>
/// Base class for any in-world indicator elements
/// </summary>
public class IndicatorGUI : MonoBehaviour
{
    [Tooltip("Get reference to SpriteRenderer for Indicator")]
    [SerializeField] protected SpriteRenderer indicatorIcon;

    private void Awake()
    {
        // Set icon automatically if null
        if (indicatorIcon == null)
            indicatorIcon = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Set indicatorIcon
    /// </summary>
    public void SetIcon(Sprite newIcon)
    {
        if (indicatorIcon != null)
            indicatorIcon.sprite = newIcon;
    }

    /// <summary>
    /// Show Indicator
    /// </summary>
    /// <param name="onComplete"> Callback that is triggered when ShowIndicator is complete. </param>
    public virtual void ShowIndicator(System.Action onComplete = null) 
    { 
        indicatorIcon.gameObject.SetActive(true);
        if (onComplete != null) onComplete();
    }

    /// <summary>
    /// Hide Indicator
    /// </summary>
    /// <param name="onComplete"> Callback that is triggered when HideIndicator is complete.</param>
    public virtual void HideIndicator(System.Action onComplete = null) { 
        indicatorIcon.gameObject.SetActive(false);
        if (onComplete != null) onComplete();
    }
}