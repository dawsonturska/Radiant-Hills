using UnityEngine;
using UnityEngine.EventSystems;

public static class EventSystemSelectHelper
{
    /// <summary>
    /// Every time a script wants to automatically change EventSystem's currently selected object, this method should be called
    /// </summary>
    /// <param name="go"></param>
    public static void SetSelectedGameObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null); // clear selected object

        if (go == null) return;

        ButtonSelectionHandler handler = go.GetComponentInParent<ButtonSelectionHandler>();

        if (handler != null) handler.IsAutoNavigate = true;

        EventSystem.current.SetSelectedGameObject(go); // set new selected object
    }
}