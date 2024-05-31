using UnityEngine;

public class USBPluggable : MonoBehaviour
{
    public Transform keyboardPlugPosition; // La position et la rotation où la clé USB doit se plugger
    private InteractableObject usbKey;
    public float plugThreshold = 0.0001f; // Distance de tolérance pour plugger la clé USB
    private bool isPressing = false;
    private float pressStartTime;

    void Start()
    {
        // Trouver l'objet clé USB dans la scène
        usbKey = GameObject.Find("usbkey").GetComponent<InteractableObject>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartPress();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndPress();
        }
    }

    private void StartPress()
    {
        isPressing = true;
        pressStartTime = Time.time;
    }

    private void EndPress()
    {
        if (isPressing)
        {
            float pressDuration = Time.time - pressStartTime;

            if (pressDuration >= 1f) // Si la durée de l'appui est supérieure ou égale à 1 seconde
            {
                // Vérifier la distance entre la clé USB et la position de plug du clavier
                float distance = Vector3.Distance(usbKey.transform.position, keyboardPlugPosition.position);
                if (distance <= plugThreshold)
                {
                    PlugUSBToKeyboard();
                }
            }
            isPressing = false;
        }
    }

    private void PlugUSBToKeyboard()
    {
        // Déplacer et orienter la clé USB pour la plugger au clavier
        usbKey.transform.position = keyboardPlugPosition.position;
        usbKey.transform.rotation = keyboardPlugPosition.rotation;

        // Supprimer le composant InteractableObject
        Destroy(usbKey.GetComponent<InteractableObject>());
    }
}
