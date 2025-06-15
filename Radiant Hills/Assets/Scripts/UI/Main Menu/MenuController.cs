using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class MainMenuController : MonoBehaviour
{
    public bool IsTitleGraphic { get; private set; } = true;

    [Tooltip("Reference to animator of title object")]
    [SerializeField] private Animator titleAnimator;

    [Tooltip("Reference to container of main menu buttons")]
    [SerializeField] private GameObject mainMenuButtonsContainer;

    [Tooltip("First button to select on main menu")]
    [SerializeField] private GameObject menuFirstSelectedButton;

    // Reference to "Submit" UI InputAction
    private InputAction submitAction;

    private void OnEnable()
    {
        mainMenuButtonsContainer.SetActive(false);

        // Assumes this object has an attached PlayerInput component
        var inputActionAsset = GetComponent<PlayerInput>().actions;
        submitAction = inputActionAsset["Submit"];

        submitAction.performed += HandleSubmit;
        if (!submitAction.enabled) { submitAction.Enable(); }
    }

    private void OnDisable()
    {
        submitAction.performed -= HandleSubmit;
        submitAction.Disable();
    }

    private void Update()
    {
        // Handle mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (IsTitleGraphic) StartCoroutine(TitleSlideOut());
        }
    }

    private void HandleSubmit(InputAction.CallbackContext context)
    {
        if (IsTitleGraphic) StartCoroutine(TitleSlideOut());
    }

    // Coroutine for sliding out title and activating Main Menu Buttons Container
    private IEnumerator TitleSlideOut()
    {
        titleAnimator.SetTrigger("slideOut");
        mainMenuButtonsContainer.SetActive(true);
        mainMenuButtonsContainer.GetComponent<GraphicsFaderCanvas>().FadeTurnOn(false);

        yield return null;

        EventSystemSelectHelper.SetSelectedGameObject(menuFirstSelectedButton);
        IsTitleGraphic = false;
    }

    /// <summary>
    /// Set all Main Menu Buttons as active or inactive
    /// </summary>
    /// <param name="interactable"></param>
    public void SetAllMenuButtonsInteractable(bool interactable)
    {
        Button[] buttons = mainMenuButtonsContainer.GetComponentsInChildren<Button>(includeInactive: true);
        foreach (var button in buttons)
        {
            button.interactable = interactable;
        }
    }
}
