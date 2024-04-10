using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapController : MonoBehaviour
{
    public GameObject targetSnapPoint; // Le point d'accroche cible sur l'autre morceau
    public GameObject mySnapPoint; // Le point d'accroche de cet objet
    public float snapThreshold = 0.5f; // La distance à partir de laquelle le snap est appliqué

    void Update()
    {
        // Vérifie la distance entre les points d'accroche
        if (Vector3.Distance(mySnapPoint.transform.position, targetSnapPoint.transform.position) <= snapThreshold)
        {
            // Applique le snap en ajustant la position de cet objet pour que les points d'accroche coïncident
            transform.position = targetSnapPoint.transform.position - (mySnapPoint.transform.position - transform.position);
            // Optionnel : Verrouiller la position pour empêcher d'autres mouvements
        }
    }
}

