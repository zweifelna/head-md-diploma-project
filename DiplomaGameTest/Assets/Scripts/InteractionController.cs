using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public float longPressThreshold = 2f;
    [SerializeField]
    private float moveSpeed = 5f;
    private IInteractable selectedObject;
    private GameObject pressingObject;
    private float pressTime = 0f;
    private bool isPressing = false;
    private bool isFollowing = false;
    [SerializeField]
    private float selectHeight = 8.55f;
    private InteractableObject.ObjectState currentState = InteractableObject.ObjectState.Complete;
    [SerializeField]
    private float placementThreshold = 1.0f; // La distance maximale pour considérer l'objet comme étant dans la zone cible


    void Update()
    {
        HandleInput();

        if (isFollowing && pressingObject != null)
        {
            UpdateFollowingObjectPosition();

            // Faire le check de placement en live pour le feedback visuel
            var interactableObject = pressingObject.GetComponent<InteractableObject>();
            if (interactableObject != null)
            {
                bool canPlace = CheckPlacement(interactableObject, false); // Ne pas appliquer le placement, juste vérifier
                interactableObject.ShowPlacementFeedback(canPlace);
            }
            
        }
        
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
                // Suspend la rotation de l'objet actuellement sélectionné
                if (selectedObject != null)
                {
                    selectedObject.CanRotate = false;
                }
            }
        }
    }

    private void UpdateFollowingObjectPosition()
    {
        if (pressingObject != null)
        {
            // Calcule la position cible basée sur la position actuelle de la souris/toucher
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectHeight));
            
            // Utilisation de Lerp pour un mouvement fluide avec un retard
            pressingObject.transform.position = Vector3.Lerp(pressingObject.transform.position, targetPosition, Time.deltaTime * moveSpeed);
            
        }
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
                //interactableObject.SetCanRotate(true);
                CheckPlacement(interactableObject); // Vérifie si l'objet doit être placé ou réinitialisé
                if (selectedObject != null && (object)selectedObject != (object)interactableObject)
                {
                    selectedObject.CanRotate = true;
                }

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
            interactableObject.ResetVisualFeedback();
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

    private bool CheckPlacement(InteractableObject interactableObject, bool applyPlacement = false)
    {
        Collider[] hitColliders = Physics.OverlapSphere(interactableObject.transform.position, placementThreshold);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("PlacementZone") && hitCollider.gameObject.name == interactableObject.GetDestinationZoneName())
            {
                if (applyPlacement)
                {
                    interactableObject.Place();
                }
                return true; // Placement possible
            }
        }
        
        if (applyPlacement)
        {
            interactableObject.ResetPosition();
        }
        return false; // Placement non possible
    }

}
