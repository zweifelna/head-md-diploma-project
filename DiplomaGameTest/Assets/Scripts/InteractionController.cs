using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public GameManager gameManager;
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
    public float placementThreshold = 2.0f; // La distance maximale pour considérer l'objet comme étant dans la zone cible
    [SerializeField]
    private float movementThreshold = 50f; // La distance maximale en pixels que le doigt peut bouger pendant un long press
    private Vector2 initialTouchPosition;
    //private bool isLongPressActive = false;
    public static InteractionController instance;
    private float rotationSum = 0f;
    public int currentDay = 1;
    

    void Awake() {
        instance = this;
    }

    void Start() {
        gameManager = FindObjectOfType<GameManager>();  // Trouver le GameManager dans la scène
    }

    public void ClearReferences() {
        selectedObject = null;
        pressingObject = null;
    }

    void Update()
    {
        if (gameManager.currentState == GameManager.State.GameOver) {
            return;  // Si le jeu est en état de GameOver, ne rien faire
        }
        CheckBinClick();
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
        if (selectedObject != null)
        {
            selectedObject.CheckRotation();
            CheckPhoneRotation();
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
            //Debug.Log("Pressing Object: " + pressingObject.name);
        }
        else
        {
            //Debug.Log("No object hit by raycast.");
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
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 4));
            
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
                    Debug.Log("Snapping Object: " + interactableObject.name);
                    CheckPlacement(interactableObject, true);
                    GameObject destinationZone = GameObject.Find(interactableObject.GetDestinationZoneName());
                    if (destinationZone != null)
                    {
                        // // Snapper l'objet à la position de destinationZone
                        // interactableObject.transform.position = destinationZone.transform.position;

                        // interactableObject.transform.rotation = destinationZone.transform.rotation;

                        
                        interactableObject.CanRotate = true; // Ou toute autre logique nécessaire après le snap

                        // // Vérifier et forcer la mise à jour de la hiérarchie et de la position/rotation
                        // Debug.Log($"{interactableObject.name} parented to {destinationZone.name}");
                        // interactableObject.transform.localPosition = Vector3.zero;
                        // interactableObject.transform.localRotation = Quaternion.identity;
                    }
                }
                else
                {;
                    interactableObject.ResetPosition(); // Ou tout autre feedback nécessaire
                }

                if (selectedObject != null && (object)selectedObject != (object)interactableObject)
                {
                    selectedObject.CanRotate = true;
                }
          
                // if (!interactableObject.IsSelected() && selectedObject == null)
                // {
                //     // Si relâché dans la partie inférieure et qu'il n'y a aucun objet sélectionné, l'objet est sélectionné et agrandi
                //     selectedObject?.Deselect();
                //     selectedObject = interactableObject;
                //     selectedObject.Select();
                // }
                // else{
                //     interactableObject.Deselect();
                // }
            }
            interactableObject.ResetVisualFeedback();
        }

        ResetPressState();
        //gameManager.CheckIfAllDismantled();
        gameManager.CheckIfAllAssembled(); // Vérifier si tous les objets sont assemblés
    }

    private void HandleShortPress()
    {
        if (pressingObject != null)
        {   
            var interactableObject = GetHighestInteractableParent(pressingObject);

            if (interactableObject != null)
            {
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

            if (interactableObject != null && interactableObject.currentState == InteractableObject.ObjectState.Complete)
            {
                gameManager.CheckIfAllAssembled();
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
        //isLongPressActive = false; 
    }

    private bool CheckPlacement(InteractableObject interactableObject, bool applyPlacement = false)
    {
        Collider[] hitColliders = Physics.OverlapSphere(interactableObject.transform.position, placementThreshold);
        foreach (var hitCollider in hitColliders)
        {
            //Debug.Log("Hit Collider: " + hitCollider.gameObject.name);
            if (hitCollider.gameObject.CompareTag("PlacementZone") && hitCollider.gameObject.name == interactableObject.GetDestinationZoneName())
            {
                if (applyPlacement)
                {
                    interactableObject.transform.SetParent(hitCollider.transform, true);
                    interactableObject.Place();
                }
                return true; // Placement possible
            }
        }
        
        if (applyPlacement)
        {
            interactableObject.ResetPosition();
            //Debug.Log("1");
        }
        return false; // Placement non possible
    }

    private InteractableObject GetHighestInteractableParent(GameObject child)
    {
        Transform current = child.transform;
        InteractableObject highestFound = null;

        // Remonte la hiérarchie à la recherche du component InteractableObject
        while (current != null)
        {
            var interactable = current.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                highestFound = interactable;  // Garde en mémoire le dernier InteractableObject trouvé
            }
            current = current.parent;  // Remonte à l'ancêtre suivant
        }

        return highestFound;
    }

    private void CheckPhoneRotation()
    {
        if (selectedObject is InteractableObject interactableObject && interactableObject.currentState == InteractableObject.ObjectState.ReadyToDismantle)
        {
            float rotationThreshold = 30.0f; // Ajustez cette valeur selon vos besoins
            if (Input.gyro.enabled)
            {
                float rotationRate = Input.gyro.rotationRateUnbiased.z;
                if (rotationRate > rotationThreshold)
                {
                    Debug.Log("Phone turned enough");
                    interactableObject.ChangeToReadyToDismantle();
                }
            }
        }
    }

    private void CheckBinClick()
{
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.Log("test bac début");
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.name == "bin" && selectedObject != null && selectedObject is InteractableObject interactableObject)
            {
                //Debug.Log("clique sur le bac réussi");   
                if (interactableObject.isDisposable && interactableObject.currentState == InteractableObject.ObjectState.Complete)
                {
                    //Debug.Log("Etat pour bac complete");

                    if (interactableObject.isRepaired)
                    {
                        gameManager.score++;
                        gameManager.scoreText.text = $"Score: {gameManager.score}";
                        Debug.Log("Score: " + gameManager.score);
                    }
                    interactableObject.Dispose(interactableObject);  // Appeler la méthode Dispose de l'objet

                    gameManager.LoadNewObject();  // Faire apparaître un nouvel objet
                }
            }
        }
    }
}
    

}
