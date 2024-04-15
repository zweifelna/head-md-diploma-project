using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public bool isMainObject = false;
    public Vector3 centerPosition = new Vector3(0, 0, 0);
    public float animationDuration = 0.4f;
    [SerializeField]
    private Vector3 initialPosition;
    private Quaternion initialRotation;
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
    public bool IsSnapped { get; set; } = false;


    public enum ObjectState
    {
        Complete,
        Dismantled,
        Following,
        Placed
    }

    public ObjectState currentState;

    private void Start()
    {
        //initialPosition = transform.position;
        initialRotation = transform.rotation;
        originalColor = GetComponent<Renderer>().material.color;

        //Debug.Log("Valid rotations loaded: " + validRotations.Count + " | Threshold: " + rotationThreshold);

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
    }

    public void Deselect()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPosition(initialPosition, () => {
            // Réinitialise la rotation une fois que l'objet est de retour à sa position initiale
            transform.rotation = initialRotation;
        }));
        isSelected = false;

        // Détacher cet objet de son parent actuel (le rendre à nouveau un objet de premier niveau dans la hiérarchie)
        this.transform.SetParent(null);
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, System.Action onComplete = null)
    {
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
                Debug.Log("Appel la vérification du tour de smartphone");
                CheckCircularMotion();
            }
        }
    }

    public void Place()
    {
        // Logique pour "placer" l'objet, comme le déplacer à une position précise
        currentState = ObjectState.Placed; // Met à jour l'état pour indiquer que l'objet a été correctement placé
        IsSnapped = true;
        Debug.Log($"{gameObject.name} est maintenant snappé.");
        // Appliquer ici l'animation ou l'effet visuel de "placement réussi"
    }

    public void ResetPosition()
    {
        // Logique pour réinitialiser l'objet à sa position/état initial
        currentState = ObjectState.Dismantled;
        IsSnapped = false;
        Debug.Log($"{gameObject.name} a été réinitialisé et n'est plus snappé.");
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
            // Rendre la couleur un peu plus claire
            // Lerp entre la couleur originale et le blanc pour éclaircir
            float lerpValue = 0.2f; // Ajuste cette valeur pour contrôler à quel point la couleur est plus claire (0 = couleur originale, 1 = vert)
            Color lighterColor = Color.Lerp(originalColor, Color.green, lerpValue);
            renderer.material.color = lighterColor;
        }
        else
        {
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
        Quaternion currentRotation = transform.rotation;
        foreach (var validRotation in validRotations)
        {
            float angle = Quaternion.Angle(currentRotation, validRotation);

            if (angle <= rotationThreshold)
            {
                isInCorrectOrientation = true;
                PerformAction(); // Appelle une méthode quand la rotation est correcte
                return;
            }
        }
        isInCorrectOrientation = false;
        GetComponent<Renderer>().material.color = originalColor;
    }
    private void PerformAction()
    {
        // Action à réaliser lorsque la rotation est correcte
        //Debug.Log("Rotation correcte, action déclenchée.");
        GetComponent<Renderer>().material.color = highlightColor;
        TriggerVibration();
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

            Debug.Log("Rotation Rate Y: " + rotationRate * Time.deltaTime + ", Current Rotation Sum: " + rotationSum);

            if (Mathf.Abs(rotationSum) >= requiredRotation)
            {
                PerformAction();
                rotationSum = 0;
                Debug.Log("Tour complet du téléphone effectué");
            }
        }
    }
}


