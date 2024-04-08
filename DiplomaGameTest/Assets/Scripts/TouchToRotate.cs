using UnityEngine;

public class SelectAndRotate : MonoBehaviour
{
    public float rotateSpeed = 0.5f; // Ajuste cette valeur selon tes besoins
    private static GameObject selectedObject = null;
    private bool isDragging = false;

    void Update()
    {
        // Gère la sélection et la désélection d'objets
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            // Sélectionne un objet
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        if (selectedObject != gameObject)
                        {
                            selectedObject = gameObject;
                            isDragging = true;
                        }
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (isDragging)
                {
                    isDragging = false;
                }
                selectedObject = null; // Désélectionne l'objet après le relâchement
            }
        }

        // Rotation de l'objet sélectionné
        if (isDragging && selectedObject == gameObject)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                float rotationX = touch.deltaPosition.x * rotateSpeed * Time.deltaTime;
                float rotationY = touch.deltaPosition.y * rotateSpeed * Time.deltaTime;

                selectedObject.transform.Rotate(Vector3.down, rotationX, Space.World);
                selectedObject.transform.Rotate(Vector3.right, rotationY, Space.World);
            }
        }
    }
}
