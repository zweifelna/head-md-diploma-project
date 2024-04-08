using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public float longPressThreshold = 2f;
    private IInteractable selectedObject;
    private GameObject pressingObject;
    private float pressTime = 0f;
    private bool isPressing = false;
    private bool isFollowing = false;
    private float selectHeight = 7.99f;

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BeginPress();
        }

        if (isPressing)
        {
            pressTime += Time.deltaTime;
            HandleLongPress();
        }

        if (isFollowing)
        {
            UpdateFollowingObjectPosition();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndPress();
        }
    }

    private void BeginPress()
    {
        isPressing = true;
        pressTime = 0f;
        pressingObject = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            pressingObject = hit.collider.gameObject;
        }
    }

    private void HandleLongPress()
    {
        if (pressTime >= longPressThreshold && !isFollowing && pressingObject != null)
        {
            var interactableObject = pressingObject.GetComponent<InteractableObject>();
            if (interactableObject != null && !interactableObject.IsSelected())
            {
                isFollowing = true;
            }
        }
    }

    private void UpdateFollowingObjectPosition()
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectHeight));
        pressingObject.transform.position = newPosition;
    }

    private void EndPress()
    {
        if (pressTime < longPressThreshold)
        {
            HandleShortPress();
        }
        else if (isFollowing && pressingObject != null)
        {
            var interactableObject = pressingObject.GetComponent<InteractableObject>();
            if (interactableObject != null)
            {
                interactableObject.SetCanRotate(true);

                // Détermine la position de relâchement
                if (Input.mousePosition.y > Screen.height / 2)
                {
                    // Si relâché dans la partie supérieure, l'objet retourne à sa place
                    interactableObject.Deselect();
                }
                else if (!interactableObject.IsSelected())
                {
                    // Si relâché dans la partie inférieure, l'objet est sélectionné et agrandi
                    selectedObject?.Deselect();
                    selectedObject = interactableObject;
                    selectedObject.Select();   
                }
            }
        }

        ResetPressState();
    }

    private void HandleShortPress()
    {
        if (pressingObject != null)
        {
            var interactable = pressingObject.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (selectedObject != interactable)
                {
                    selectedObject?.Deselect();
                    selectedObject = interactable;
                    selectedObject.Select();
                }
            }
            else
            {
                selectedObject.Deselect();
                selectedObject = null;
            }
        }
        else if (selectedObject != null)
        {
            selectedObject.Deselect();
            selectedObject = null;
        }
    }

    private void ResetPressState()
    {
        isPressing = false;
        isFollowing = false;
        pressingObject = null;
        pressTime = 0f;
    }
}
