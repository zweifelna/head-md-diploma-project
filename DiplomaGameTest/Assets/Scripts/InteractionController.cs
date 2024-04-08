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
        if (pressingObject != null)
        {
            // Calcule la position cible basée sur la position actuelle de la souris/toucher
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectHeight));
            
            // Option 1: Utilisation de Lerp pour un mouvement fluide avec un retard
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
                interactableObject.SetCanRotate(true);
                CheckPlacement(interactableObject); // Vérifie si l'objet doit être placé ou réinitialisé

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

    private void CheckPlacement(InteractableObject interactableObject)
    {
        // Log pour indiquer que la vérification de placement commence
        Debug.Log($"Vérification du placement pour {interactableObject.name}");

        Collider[] hitColliders = Physics.OverlapSphere(interactableObject.transform.position, placementThreshold);
        bool placedCorrectly = false;

        foreach (var hitCollider in hitColliders)
        {
            // Vérifie si l'objet est proche de sa zone de placement
            Debug.Log($"Objet {interactableObject.name} détecté proche de {hitCollider.gameObject.name}");

            if (hitCollider.gameObject.CompareTag("PlacementZone") && hitCollider.gameObject.name == interactableObject.GetDestinationZoneName())
            {
                Debug.Log($"Objet {interactableObject.name} placé correctement dans {hitCollider.gameObject.name}");
                placedCorrectly = true;
                break;
            }
        }
        
        if (placedCorrectly)
        {
            interactableObject.Place();
        }
        else
        {
            Debug.Log($"Objet {interactableObject.name} n'a pas été placé correctement.");
            interactableObject.ResetPosition();
        }
    }
}
