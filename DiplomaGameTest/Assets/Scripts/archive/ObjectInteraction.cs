using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public Vector3 originalPosition;
    public float rotateSpeed = 0.5f; // Ajuste cette valeur selon tes besoins
    private bool isRotating = false;

    void Awake()
    {
        originalPosition = transform.position; // Stocke la position initiale de l'objet
    }

    void Update()
    {
        if (isRotating && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float rotationX = touch.deltaPosition.x * rotateSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, -rotationX, Space.World);
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        StartCoroutine(MoveTowards(position));
    }

    IEnumerator MoveTowards(Vector3 position)
    {
        while (Vector3.Distance(transform.position, position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * 5f);
            yield return null;
        }
        isRotating = true; // Permet la rotation une fois arrivé au centre
    }

    public void ReturnToOriginalPosition()
    {
        StartCoroutine(MoveTowards(originalPosition));
        isRotating = false; // Arrête la rotation
    }
}
