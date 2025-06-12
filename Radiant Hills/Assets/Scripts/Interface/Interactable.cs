using UnityEngine;

/// <summary>
/// Attach this to scripts where the "Interact" input does something
/// </summary>
public interface IInteractable
{
    void Interact(PlayerInputHandler handler);
}

//  SETUP:
//
//  1. IInteractable to script derivation
//
//  2. Interact Function
//
//  public void Interact(PlayerInputHandler handler)
//  {
//      INTERACTION LOGIC
//  }
//
//  3. Set Interactable via some trigger.
//      Ex: Collision
//
//  private void OnTriggerEnter2D(Collider2D collision)
//  {
//    if (collision.CompareTag("Player"))
//    {
//        var playerHandler = collision.GetComponent<PlayerInputHandler>();
//        if (playerHandler != null)
//        {
//            playerHandler.SetCurrentInteractable(OBJECT);
//        }
//    }
//  }
//
//  private void OnTriggerExit2D(Collider2D collision)
//  {
//    if (collision.CompareTag("Player"))
//    {
//        var playerHandler = collision.GetComponent<PlayerInputHandler>();
//        if (playerHandler != null)
//        {
//            playerHandler.ClearInteractable(OBJECT);
//        }
//    }
//  }