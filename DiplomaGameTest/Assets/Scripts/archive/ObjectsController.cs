using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsController : MonoBehaviour
{
    public static GameObject selectedObject = null;
    public GameObject centerPoint; // Assigne un objet vide dans l'éditeur qui représente le centre
    private float moveSpeed = 5f;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (touch.phase == TouchPhase.Began && Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Interactable"))
                {
                    if (selectedObject != hit.collider.gameObject)
                    {
                        if (selectedObject != null)
                        {
                            // Désélectionne l'objet précédent
                            selectedObject.GetComponent<ObjectInteraction>().ReturnToOriginalPosition();
                        }
                        // Sélectionne un nouvel objet
                        selectedObject = hit.collider.gameObject;
                        selectedObject.GetComponent<ObjectInteraction>().MoveToPosition(centerPoint.transform.position);
                    }
                }
                else if (selectedObject != null)
                {
                    // Désélectionne l'objet actuel si on touche en dehors d'un objet interactif
                    selectedObject.GetComponent<ObjectInteraction>().ReturnToOriginalPosition();
                    selectedObject = null;
                }
            }
        }
    }
}
