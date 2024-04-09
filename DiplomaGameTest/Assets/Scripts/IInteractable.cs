public interface IInteractable
{
    void Select();
    void Deselect();
    bool CanRotate { get; set; }
}