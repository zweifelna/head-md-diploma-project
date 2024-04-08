using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public bool isMainObject = false;
    public Vector3 centerPosition = new Vector3(0, 0, 0);
    public float moveSpeed = 5f;
    public float animationDuration = 0.4f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isSelected = false;
    public float rotateSpeed = 100f; 
    [SerializeField] private Color selectedColor = Color.green; // Couleur lors de la sélection
    private Color originalColor; // Pour stocker la couleur originale
    private bool canRotate = true; // Contrôle si l'objet peut tourner


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

    public void SetCanRotate(bool rotate)
    {
        canRotate = rotate;
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
        //animationDuration = 1f; // Durée de l'animation, en secondes. Ajuste selon les besoins.
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = (targetPosition == initialPosition) ? initialRotation : transform.rotation;

        while (elapsedTime < animationDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / animationDuration);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assure que l'objet est exactement à la position et rotation cibles à la fin de l'animation
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
        Debug.Log("ToggleState appelé sur " + gameObject.name);
        isSelected = !isSelected; // Bascule l'état de sélection
        GetComponent<Renderer>().material.color = isSelected ? selectedColor : originalColor;
    }
}
