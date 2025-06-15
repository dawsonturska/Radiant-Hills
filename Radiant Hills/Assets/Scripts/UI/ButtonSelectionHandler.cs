using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Manages currently selected UI object, triggers certain events if types match
/// </summary>
public class ButtonSelectionHandler : MonoBehaviour
{
    // Button-Selection-Related Events
    public UnityEvent OnGeneralButtonSelected = new UnityEvent();
    public UnityEvent OnGeneralObjectSelected = new UnityEvent();

    private GameObject lastSelected;

    /// <summary>
    /// Flag for whether selection was made automatically (by script, not by player input)
    /// </summary>
    public bool IsAutoNavigate
    {
        get { return _isAutoNavigate; }
        set { _isAutoNavigate = value; }
    }

    private bool _isAutoNavigate = false;

    private void Update()
    {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != lastSelected && currentSelected != null)
        {
            // if automatically navigated, return
            if (IsAutoNavigate) {
                lastSelected = currentSelected;
                IsAutoNavigate = false;
                return; 
            }

            // Invoke for general buttons
            if (currentSelected != null && currentSelected.GetComponent<GeneralButtonControl>() != null)
            {
                OnGeneralButtonSelected?.Invoke();
                lastSelected = currentSelected;
                return;
            }

            // Invoke for general selections
            if (currentSelected != null)
            {
                OnGeneralObjectSelected?.Invoke();
                lastSelected = currentSelected;
                return;
            }
        }
    }
}