/// <summary>
/// Attach this to scripts where the "Interact" input does something
/// </summary>
public interface IInteractable
{
    void Interact(PlayerInputHandler handler);
}