using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public bool isMainObject = false;
    public Vector3 centerPosition = new Vector3(0, 0, 0);
    //public float moveSpeed = 5f;
    public float animationDuration = 0.4f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isSelected = false;
    public float rotateSpeed = 100f; 
    [SerializeField] private Color selectedColor = Color.green; // Couleur lors de la sélection
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
    [SerializeField]
    private string destinationZoneName; // Le nom de la zone de destination spécifique pour cet objet
    public string GetDestinationZoneName()
    {
        return destinationZoneName;
    }


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
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        originalColor = GetComponent<Renderer>().material.color;
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
            }
            // Ajoute ici une logique similaire pour les interactions tactiles si nécessaire
        }
    }

    public void ToggleState()
    {
        isSelected = !isSelected; // Bascule l'état de sélection
        GetComponent<Renderer>().material.color = isSelected ? selectedColor : originalColor;
    }

    public void Place()
    {
        // Logique pour "placer" l'objet, comme le déplacer à une position précise
        currentState = ObjectState.Placed; // Met à jour l'état pour indiquer que l'objet a été correctement placé
        // Appliquer ici l'animation ou l'effet visuel de "placement réussi"
    }

    public void ResetPosition()
    {
        // Logique pour réinitialiser l'objet à sa position/état initial
        currentState = ObjectState.Dismantled; // ou Complete, selon la logique de ton jeu
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
}


