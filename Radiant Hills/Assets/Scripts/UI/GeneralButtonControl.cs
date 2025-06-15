using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Extends visual status of buttons
/// </summary>
// adapted from x4000 on Unity Forum

[RequireComponent(typeof(Button))]
public class GeneralButtonControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    #region Button Status Text Colors
    [Header("Button Status Text Colors")]
        [Tooltip("Color for \"Normal\" button status")]
        public Color textNormalColor;
        [Tooltip("Color for \"Normal\" button status")]
        public Color textDisabledColor;
        [Tooltip("Color for \"Normal\" button status")]
        public Color textPressedColor;
        [Tooltip("Color for \"Normal\" button status")]
        public Color textHighlightedColor;
    #endregion

    // Child text component
    protected TextMeshProUGUI txt;
    // Extension of button, need reference
    private Button btn;

    // Previous ButtonStatus, corresponds to Button.SelectedState
    protected ButtonStatus lastButtonStatus = ButtonStatus.Normal;
    // Flag if desired status is "Highlighted"
    protected bool isHighlightDesired = false;
    // Flag if desired status is "Prressed"
    protected bool isPressedDesired = false;

    /// PRIVATE / PROTECTED METHODS ///

    private void Start()
    {
        txt = GetComponentInChildren<TextMeshProUGUI>();
        btn = gameObject.GetComponent<Button>();
    }

    // Can be overriden by child classes
    protected virtual void Update()
    {
        ButtonStatus desiredButtonStatus = ButtonStatus.Normal;
        if (!btn.interactable)
            desiredButtonStatus = ButtonStatus.Disabled;
        else
        {
            if (isHighlightDesired)
                desiredButtonStatus = ButtonStatus.Highlighted;
            if (isPressedDesired)
                desiredButtonStatus = ButtonStatus.Pressed;
        }

        if (desiredButtonStatus != this.lastButtonStatus)
        {
            this.lastButtonStatus = desiredButtonStatus;
            switch (this.lastButtonStatus)
            {
                case ButtonStatus.Normal:
                    txt.color = textNormalColor;
                    break;
                case ButtonStatus.Disabled:
                    txt.color = textDisabledColor;
                    break;
                case ButtonStatus.Pressed:
                    txt.color = textPressedColor;
                    break;
                case ButtonStatus.Highlighted:
                    txt.color = textHighlightedColor;
                    break;
            }
        }
    }


    /// PUBLIC METHODS ///

    // All functions modify desired state

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (btn.interactable) isHighlightDesired = true;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (btn.interactable) isPressedDesired = true;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (btn.interactable) isPressedDesired = false;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (btn.interactable) isHighlightDesired = false;
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        if (btn != null && btn.interactable)
        {
            isHighlightDesired = true;
        }
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        if (btn != null && btn.interactable)
        {
            isHighlightDesired = false;
        }
    }

    /// <summary>
    /// Status of button, corresponds to Button.SelectedState
    /// </summary>
    public enum ButtonStatus
    {
        Normal,
        Disabled,
        Highlighted,
        Pressed
    }
}