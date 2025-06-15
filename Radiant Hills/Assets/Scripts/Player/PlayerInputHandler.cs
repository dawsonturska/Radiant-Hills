using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Centralized player input manager that forwards callbacks and manages action maps
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(Inventory))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions inputActions;

    // Reference to main player components
    #region Player Components
    private PlayerMovement pMovement;
    private PlayerAttack pAttack;
    private Inventory pInventory;
    private PauseMenuController pPauseMenuController;
    #endregion

    // currently selected interactable object
    private IInteractable currInteractable;
    // list for handling nearby interactables
    private List<IInteractable> nearbyInteractables = new();


    private void Awake()
    {
        // Create PlayerInputActions here
        inputActions = new PlayerInputActions();

        pMovement = GetComponent<PlayerMovement>();
        pAttack = GetComponent<PlayerAttack>();
        pInventory = GetComponent<Inventory>();

        // Pause menu is a child canvas
        pPauseMenuController = GetComponentInChildren<PauseMenuController>();

        pPauseMenuController.OnUnpause += HandleUnpause;
}

    private void OnEnable()
    {
        // For now, enable player controls by default
        EnablePlayerControls();
    }

    private void OnDisable()
    {
        pPauseMenuController.OnUnpause -= HandleUnpause;
        DisableAllControls();
    }


    /// <summary>
    /// Enable Player Controls and Disable UI Controls
    /// </summary>
    public void EnablePlayerControls()
    {
        inputActions.UI.Disable();
        inputActions.Player.Enable();

        // Interactions to forward to single objects
        inputActions.Player.Movement.performed += OnMovement;
        inputActions.Player.Movement.canceled += OnMovement;
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Inventory.performed += OnInventory;
        inputActions.Player.Pause.performed += OnPause;

        // Generic "Interact" actions need to be forwarded to mutliple objects, so special cases
        inputActions.Player.Interact.performed += OnInteractPerformed;

        Debug.Log("here");
        
    }

    /// <summary>
    /// Enable UI Controls and Disable Player Controls
    /// </summary>
    public void EnableUIControls()
    {
        inputActions.Player.Disable();
        inputActions.UI.Enable();

        // Interactions to forward to single objects
        inputActions.UI.Cancel.performed += OnCancelUI;
        inputActions.UI.Unpause.performed += OnUnpauseSpecial;
    }

    public void DisableAllControls()
    {
        inputActions.Disable();
    }

    #region PLAYER CONTROL FORWARDING

        // Forward the "Movement" action
        private void OnMovement(InputAction.CallbackContext context)
        {
            pMovement.HandleMovement(context);
        }

        // Forward the "Attack" action
        private void OnAttack(InputAction.CallbackContext context)
        {
            pAttack.HandleAttack(context);
        }

        // Forward the "Inventory" action
        private void OnInventory(InputAction.CallbackContext context)
        {
            pInventory.HandleToggleInventory(context);
        }

        // Forward the "Pause" action
        private void OnPause(InputAction.CallbackContext context)
        {
            if (!PauseMenuController.IsPaused)
            {
                pPauseMenuController.Pause(context);
                EnableUIControls();
            }
        }

    #endregion

    #region UI CONTROL FORWARDERS
        // Forward the "Cancel" UI action
        private void OnCancelUI(InputAction.CallbackContext context)
        {
            if (PauseMenuController.IsPaused) { 
                pPauseMenuController.Unpause(); 
                HandleUnpause(); 
            }

            // more cases here

        }
        
        // Unpause special case (for any non-cancel actions that unpause)
        private void OnUnpauseSpecial(InputAction.CallbackContext context)
        {
            if (PauseMenuController.IsPaused)
            {
                pPauseMenuController.Unpause();
                HandleUnpause();
            }
        }

        #region PUBLIC CANCEL HANDLERS

            public void HandleUnpause()
            {
                EnablePlayerControls();
            }

        #endregion

    #endregion

    #region INTERACTION HANDLERS

        // Forward the generic "Interact" action to currInteractable
        private void OnInteractPerformed(InputAction.CallbackContext ctx)
        {
            currInteractable?.Interact(this);
        }

        // Set current interactable properly
        private void UpdateCurrentInteractable()
        {
            currInteractable = nearbyInteractables.LastOrDefault();
        }

        /// <summary>
        /// Set Interactable, always call when entering the collision range of an interactable
        /// </summary>
        /// <param name="interactable"></param>
        public void SetCurrentInteractable(IInteractable interactable)
        {
            if (Time.timeScale == 0) return; // Ignore input if the game is paused

            if (!nearbyInteractables.Contains(interactable))
                nearbyInteractables.Add(interactable);

            UpdateCurrentInteractable();
        }

        /// <summary>
        /// Clear Interactable, always call when exiting the collision range of an interactable
        /// </summary>
        /// <param name="interactable"></param>
        public void ClearInteractable(IInteractable interactable)
        {
            nearbyInteractables.Remove(interactable);
            UpdateCurrentInteractable();
        }

    #endregion

}