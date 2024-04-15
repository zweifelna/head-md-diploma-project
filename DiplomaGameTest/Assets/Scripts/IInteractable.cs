public interface IInteractable
{
    void Select();
    void Deselect();
    void CheckRotation();
    bool CanRotate { get; set; }

    bool IsRotating { get; set;}
    bool isInCorrectOrientation { get; set; }
    
}