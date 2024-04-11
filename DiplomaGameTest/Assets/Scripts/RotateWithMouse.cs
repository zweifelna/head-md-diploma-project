using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithMouse : MonoBehaviour
{
    public float mouseSpeed = 5f;
    public float touchSpeed = 1f;

    private Vector2 lastRotationSpeed = Vector2.zero;
    private bool isRotating = false; 

    void Update()
    {
        // Pour PC - Utilisation de la souris
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Vérifie si l'objet touché par le rayon est cet objet
                if (hit.collider.gameObject == gameObject)
                {
                    isRotating = true;
                    float rotateX = Input.GetAxis("Mouse X") * mouseSpeed;
                    float rotateY = Input.GetAxis("Mouse Y") * mouseSpeed;

                    lastRotationSpeed = new Vector2(rotateX, rotateY);

                    transform.Rotate(Vector3.down, rotateX, Space.World);
                    transform.Rotate(Vector3.right, rotateY, Space.World);
                }
            }
        }
        else
        {
            isRotating = false;
        }
    }
}
