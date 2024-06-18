using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public bool isMainObject = false;
    private bool isFirstSelect = true;
    public bool isDisposable = false;
    public bool isRepaired = false;
    public bool isBeingRepaired = false;
    public Vector3 centerPosition = new Vector3(0, 0, 0);
    public float animationDuration = 0.4f;
    [SerializeField]
    public Vector3 initialPosition;
    public Quaternion initialRotation;
    private bool isSelected = false;
    public float rotateSpeed = 100f; 
    [SerializeField]
    private List<Quaternion> validRotations = new List<Quaternion>();
    public float rotationThreshold = 300.0f;
    private bool _isInCorrectOrientation = false;
    public bool isInCorrectOrientation
    {
        get { return _isInCorrectOrientation; }
        set { _isInCorrectOrientation = value; }
    }
    public float requiredRotation = 10f; // Définissez combien de degrés sont nécessaires pour considérer le mouvement complet
    private float rotationSum = 0f; // Somme accumulée de la rotation
    [SerializeField] private Color highlightColor = Color.green; // Couleur lors de la sélection
    private Color originalColor; // Pour stocker la couleur originale
    private Color readyToDismantleColor = Color.blue;
    private Renderer objectRenderer;
    private bool canRotate = true; // Contrôle si l'objet peut tourner
    public bool CanRotate
    {
        get { return canRotate; }
        set
        {
            canRotate = value;
        }
    }
    public bool isRotating = false;
     public bool IsRotating
    {
        get { return isRotating; }
        set
        {
            isRotating = value;
        }
    }
    [SerializeField]
    private string destinationZoneName; // Le nom de la zone de destination spécifique pour cet objet
    public string GetDestinationZoneName()
    {
        return destinationZoneName;
    }
    [SerializeField]
    public bool IsSnapped = false;
    public event Action OnSnapped;


    public enum ObjectState
    {
        Complete,
        Dismantled,
        Following,
        Placed,
        ReadyToDismantle
    }

    public ObjectState currentState;

    private void Start()
    {
        //initialPosition = transform.position;
        initialRotation = transform.rotation;
        originalColor = GetComponent<Renderer>().material.color;
        objectRenderer = GetComponent<Renderer>();
        isBeingRepaired = true;

        //Debug.Log("Valid rotations loaded: " + validRotations.Count + " | Threshold: " + rotationThreshold);
        if(isMainObject)
        {
            currentState = ObjectState.Dismantled;
        }
        else{
            currentState = ObjectState.Complete; // État initial défini comme complet
        }

        isRepaired = true; // Initialiser tous les objets comme réparés
        
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        else
        {
            Debug.Log("Gyroscope not supported on this device.");
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void Select()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPosition(centerPosition));
        isSelected = true;

        // if(isFirstSelect){
        //     GameManager.Instance.ObjectSelected();
        //     isFirstSelect = false;
        // }
        
    }

    public void Deselect()
    {
        if (this != null)
        {
            //Debug.Log("deselected");
            StopAllCoroutines();
            StartCoroutine(MoveToPosition(initialPosition, () => {
                // Réinitialise la rotation une fois que l'objet est de retour à sa position initiale
                transform.rotation = initialRotation;
            }));
            isSelected = false;

            // Détacher cet objet de son parent actuel (le rendre à nouveau un objet de premier niveau dans la hiérarchie)
            this.transform.SetParent(null);
        }
    }

    public void ChangeToReadyToDismantle()
    {
        currentState = ObjectState.ReadyToDismantle;
        objectRenderer.material.color = readyToDismantleColor;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, System.Action onComplete = null)
    {
        currentState = ObjectState.Dismantled;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = (targetPosition == initialPosition) ? initialRotation : transform.rotation;

        while (elapsedTime < animationDuration)
        {
            // Utilise SmoothStep pour un effet d'ease in/out
            float fraction = elapsedTime / animationDuration;
            float smoothFraction = Mathf.SmoothStep(0.0f, 1.0f, fraction);

            transform.position = Vector3.Lerp(startPosition, targetPosition, smoothFraction);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, smoothFraction);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = endRotation;

        onComplete?.Invoke();
    }

    void Update()
    {
        if (isSelected && canRotate)
        {
            if (Input.GetMouseButton(0)) // Pour la souris
            {
                float rotateX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
                float rotateY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

                transform.Rotate(Vector3.forward, -rotateX, Space.World);
                transform.Rotate(Vector3.right, rotateY, Space.World);

                IsRotating = true;
            }
            else
            {
                IsRotating = false;
            }
            if (isInCorrectOrientation) // Assurez-vous que cette propriété est correctement définie ailleurs dans le script
            {
                //Debug.Log("Appel la vérification du tour de smartphone");
                CheckCircularMotion();
            }
        }
    }

    public void Place()
    {
        // Logique pour "placer" l'objet, comme le déplacer à une position précise
        currentState = ObjectState.Complete; // Met à jour l'état pour indiquer que l'objet a été correctement placé
        //Debug.Log($"{gameObject.name} current state after placing: {currentState}");
        IsSnapped = true;
        OnSnapped?.Invoke();
        MarkAsRepaired();
        //Debug.Log($"{gameObject.name} current state after placing: {currentState}");
        //Debug.Log($"{gameObject.name} est maintenant snappé.");
        // Appliquer ici l'animation ou l'effet visuel de "placement réussi"
        // Forcer la mise à jour de la hiérarchie et de la position/rotation
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, -90);
        Debug.Log("TEST ECRAN PLACE");
        if (GameManager.Instance.currentState == GameManager.State.Tutorial && gameObject.name == "Repaired_Ecran(Clone)")
        {
            Debug.Log("NOUVEL ECRAN PLACE");
            GameManager.Instance.ReplaceScreen();
        }
    
        // Validation supplémentaire
        //Debug.Log($"Placed: Local Position = {transform.localPosition}, Local Rotation = {transform.localRotation}");
    }

    public void ResetPosition()
    {
        //Debug.Log("ResetPosition called for " + gameObject.name);
        // Logique pour réinitialiser l'objet à sa position/état initial
        currentState = ObjectState.Dismantled;
        IsSnapped = false;
        //Debug.Log($"{gameObject.name} a été réinitialisé et n'est plus snappé.");
        transform.SetParent(null);
        // Commence une coroutine pour déplacer l'objet à sa position initiale avec une animation
        StartCoroutine(MoveToPosition(initialPosition, () => {
            // Optionnel : Réinitialise la rotation ou d'autres propriétés si nécessaire
        }));
    }

    public void ShowPlacementFeedback(bool canPlace)
    {
        var renderer = GetComponent<Renderer>();
        if (canPlace)
        {
            //Debug.Log($"{gameObject.name} can be placed.");
            // Lerp entre la couleur originale et le blanc pour éclaircir
            float lerpValue = 0.2f; // Ajuste cette valeur pour contrôler à quel point la couleur est plus claire (0 = couleur originale, 1 = vert)
            Color lighterColor = Color.Lerp(originalColor, Color.green, lerpValue);
            renderer.material.color = lighterColor;
        }
        else
        {
            //Debug.Log($"{gameObject.name} cannot be placed.");
            // Revenir à la couleur normale ou indiquer que le placement n'est pas possible
            ResetVisualFeedback();
        }
    }

    public void ResetVisualFeedback()
    {
        // Rétablit la couleur originale de l'objet
        GetComponent<Renderer>().material.color = originalColor;
    }

    public void CheckRotation()
    {
        if (this == null || gameObject == null) return;

        Quaternion currentRotation = transform.rotation;
        foreach (var validRotation in validRotations)
        {
            if (this == null || gameObject == null) return; // Vérifie encore une fois dans la boucle
            float angle = Quaternion.Angle(currentRotation, validRotation);

            if (angle <= rotationThreshold)
            {
                isInCorrectOrientation = true;
                PerformAction(); // Appelle une méthode quand la rotation est correcte
                return;
            }
        }
        isInCorrectOrientation = false;
        if (GetComponent<Renderer>() != null) // Vérifie si le Renderer existe encore
            GetComponent<Renderer>().material.color = originalColor;
    }
    private void PerformAction()
    {
        // Action à réaliser lorsque la rotation est correcte
        //Debug.Log("Rotation correcte, action déclenchée.");
        GetComponent<Renderer>().material.color = highlightColor;
        //TriggerVibration();
    }
    private void TriggerVibration()
    {
        #if UNITY_IOS
        Handheld.Vibrate();
        #endif
    }

    void CheckCircularMotion()
    {
        if (Input.gyro.enabled)
        {
            float rotationRate = Input.gyro.rotationRateUnbiased.y;
            rotationSum += rotationRate * Time.deltaTime;

            //Debug.Log("Rotation Rate Y: " + rotationRate * Time.deltaTime + ", Current Rotation Sum: " + rotationSum);

            if (Mathf.Abs(rotationSum) >= requiredRotation)
            {
                PerformAction();
                rotationSum = 0;
                //Debug.Log("Tour complet du téléphone effectué");
            }
        }
    }

    public void Dismantle()
    {
        if (currentState == ObjectState.Complete || currentState == ObjectState.Placed)
        {
            currentState = ObjectState.Dismantled;
            // Debug.Log($"{gameObject.name} has been dismantled.");
            // Ajoutez ici toute logique supplémentaire pour l'effet visuel ou le déplacement de la pièce
        }
        else
        {
            //Debug.Log(gameObject.name + " is already in state: " + currentState);
        }
    }

    public void Assemble()
    {
        if (currentState == ObjectState.Dismantled)
        {
            currentState = ObjectState.Complete;
            // Debug.Log($"{gameObject.name} has been reassembled.");
            // Ajoutez ici toute logique supplémentaire pour l'animation ou le retour à la position initiale
        }
    }

    void OnDestroy()
    {
        // Appeler une méthode dans le GameManager pour retirer cet objet de la liste
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemoveInteractableObject(this);
        }
    }

    public void MarkAsDisposable()
    {
        isDisposable = true;
        // Change le matériau ou autre indication visuelle pour montrer que l'objet est prêt à être jeté
        GetComponent<Renderer>().material.color = Color.blue;  // Exemple de changement de couleur
    }

    public void Dispose(InteractableObject interactableObject)
    {
        // Logique pour déplacer l'objet au-dessus du bac et activer la gravité
        Vector3 binPosition = new Vector3(-1.73f, 4.09f, 3.88f); // Remplacez par la position du bac dans votre scène
        transform.position = binPosition;
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        GameManager.Instance.discardedObjects.Add(interactableObject.gameObject);
        RemoveAllInteractableComponents(interactableObject);
    }

    private void RemoveAllInteractableComponents(InteractableObject interactableObject)
    {
        // Supprime le composant InteractableObject de l'objet principal
        Destroy(interactableObject);

        // Supprime les composants InteractableObject des enfants récursivement
        RemoveInteractableComponentsRecursively(interactableObject.transform);
    }

    private void RemoveInteractableComponentsRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            InteractableObject childInteractable = child.GetComponent<InteractableObject>();
            if (childInteractable != null)
            {
                Destroy(childInteractable);
                GameManager.Instance.allInteractableObjects.Remove(childInteractable);
            }

            // Appel récursif pour gérer tous les niveaux de la hiérarchie
            RemoveInteractableComponentsRecursively(child);
        }
    }

    public void MarkAsRepaired()
    {
        isRepaired = true;
        Debug.Log("the object is marked as repaired.");
    }

    public void MarkAsUnrepaired()
    {
        isRepaired = false;
    }
}


