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
    [SerializeField]
    private float movementThreshold = 50f; // La distance maximale en pixels que le doigt peut bouger pendant un long press
    private Vector2 initialTouchPosition;
    private bool isLongPressActive = false;


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
        initialTouchPosition = Input.mousePosition; // Enregistre la position initiale du toucher
        pressingObject = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            pressingObject = hit.collider.gameObject;
        }
    }

    private void HandleLongPress()
    {
        // Calcule la distance parcourue depuis le début du press
        float distanceMoved = Vector2.Distance(initialTouchPosition, Input.mousePosition);
        if (pressTime >= longPressThreshold && !isFollowing && pressingObject != null && distanceMoved <= movementThreshold)
        {
            var interactableObject = pressingObject.GetComponent<InteractableObject>();
            if (interactableObject != null && !interactableObject.IsSelected())
            {
                isFollowing = true;
                Debug.Log($"L'objet interactable détecté est : {interactableObject.gameObject.name}");
                // Suspend la rotation de l'objet actuellement sélectionné
                if (selectedObject != null)
                {
                    selectedObject.CanRotate = false;
                }
            }
        }
        else if (distanceMoved > movementThreshold)
        {
            // Si l'utilisateur a déplacé le doigt au-delà du seuil, considère que ce n'est plus un long press
            // Réinitialise ou annule le long press ici si nécessaire
            isPressing = false; // Optionnel : arrête de considérer cela comme un press
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
                
                bool canPlace = CheckPlacement(interactableObject, false); // Vérifie si on doit snapper
            
                if (canPlace)
                {
                    CheckPlacement(interactableObject, true);
                    GameObject destinationZone = GameObject.Find(interactableObject.GetDestinationZoneName());
                    if (destinationZone != null)
                    {
                        // Snapper l'objet à la position de destinationZone
                        interactableObject.transform.position = destinationZone.transform.position;
                        // Parente l'objet snappé à l'objet sélectionné
                        interactableObject.transform.SetParent(destinationZone.transform, true);
                        interactableObject.CanRotate = true; // Ou toute autre logique nécessaire après le snap
                    }
                }
                else
                {
                    interactableObject.ResetPosition(); // Ou tout autre feedback nécessaire
                }

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
                else if (!interactableObject.IsSelected() && selectedObject == null)
                {
                    // Si relâché dans la partie inférieure et qu'il n'y a aucun objet sélectionné, l'objet est sélectionné et agrandi
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
            Debug.Log($"Objet pressé: {pressingObject.name}");
            
            var interactableObject = pressingObject.GetComponent<InteractableObject>();
            if (interactableObject != null)
            {
                Debug.Log($"{pressingObject.name} IsSnapped: {interactableObject.IsSnapped}");
                // Vérifie si l'objet n'est pas snappé ou si c'est le fond qui est cliqué
                if (!interactableObject.IsSnapped)
                {
                    if (selectedObject != null)
                    {
                        selectedObject.Deselect();
                    }
                    else
                    {
                        // Gestion du cas où le fond est cliqué
                        selectedObject = null;
                    }

                    selectedObject = interactableObject;
                    selectedObject.Select();

                }
                
            }
            else 
            {
                    selectedObject.Deselect();
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
        isLongPressActive = false;
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
