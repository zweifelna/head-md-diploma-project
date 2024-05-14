using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera camRepair; // Caméra pour la réparation
    [SerializeField] private Camera camTerminal; // Caméra pour le terminal

    [SerializeField] private Canvas canvasRepair; // Canvas associé à camRepair
    [SerializeField] private Canvas canvasTerminal; // Canvas associé à camTerminal

    public Transform terminalPosition;  // Position devant le terminal
    public Transform backPosition;      // Position arrière pour l'effet de recul

    public float transitionDuration = 2.0f;  // Durée de l'animation de la caméra
    [SerializeField] private Quaternion repairCamRotation;
    [SerializeField] private Quaternion terminalCamRotation;
    [SerializeField] private Quaternion backPositionRotation;


    // Singleton pattern pour un accès facile depuis d'autres scripts sans référence directe
    public static CameraManager Instance { get; private set; }

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        SetActiveCamera(camRepair);  // Définir camRepair comme la caméra active au démarrage
        canvasRepair.enabled = true;
        canvasTerminal.enabled = false;
        repairCamRotation = camRepair.transform.rotation; // Sauvegarder la rotation initiale
        terminalCamRotation = Quaternion.Euler(0, 19.915f, 0); // Définir une rotation face au terminal
        backPositionRotation = Quaternion.Euler(55.062f, 0, 0); // Définir une rotation pour la position arrière
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Détecte un clic gauche de la souris
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit " + hit.collider.gameObject.name); // Affiche le nom de l'objet touché

                // Vérifie si l'objet cliqué a le nom "keyboard"
                if (hit.collider.gameObject.name == "keyboard") 
                {
                    Debug.Log("Keyboard clicked"); // Confirme que le clavier a été cliqué
                    SwitchToTerminalCamera();
                }
                // Vérifie si l'objet cliqué est le terminal
                if (hit.collider.gameObject.name == "terminal" && camTerminal.enabled) 
                {
                    Debug.Log("Terminal clicked, returning to repair camera.");
                    StartCoroutine(AnimateCameraInverse(camTerminal, terminalPosition.position, backPosition.position, camRepair.transform.position, terminalCamRotation, repairCamRotation));
                }
            }
            else
            {
                Debug.Log("No repair hit"); // Aucun objet touché
            }
            
            Ray rayTerminal = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitTerminal;

            if (Physics.Raycast(rayTerminal, out hitTerminal))
            {
                Debug.Log("Hit " + hitTerminal.collider.gameObject.name); // Affiche le nom de l'objet touché

                // Vérifie si l'objet cliqué a le nom "keyboard"
                if (hitTerminal.collider.gameObject.name == "terminal") 
                {
                    Debug.Log("Terminal clicked"); // Confirme que le clavier a été cliqué
                    StartCoroutine(AnimateCameraInverse(camTerminal, terminalPosition.position, backPosition.position, camRepair.transform.position, terminalCamRotation, repairCamRotation));
                }
                // Vérifie si l'objet cliqué est le terminal
                if (hitTerminal.collider.gameObject.name == "terminal" && camTerminal.enabled) 
                {
                    Debug.Log("Terminal clicked, returning to repair camera.");
                    StartCoroutine(AnimateCameraInverse(camTerminal, terminalPosition.position, backPosition.position, camRepair.transform.position, terminalCamRotation, repairCamRotation));
                }
            }
            else
            {
                Debug.Log("No terminal hit"); // Aucun objet touché
            }
        }
    }

    // Méthode pour changer la caméra active
    public void SetActiveCamera(Camera newCamera) {
        // Désactive toutes les caméras
        camRepair.enabled = false;
        camTerminal.enabled = false;

        Debug.Log($"Deactivating all cameras. Setting {newCamera.name} as active.");

        // Active la nouvelle caméra
        newCamera.enabled = true;

        // Active/Désactive les canvas correspondants
        Debug.Log($"Enabling canvas for {newCamera.name}");
        canvasRepair.enabled = (newCamera == camRepair);
        canvasTerminal.enabled = (newCamera == camTerminal);
        Debug.Log($"Canvas statuses: Repair Canvas = {canvasRepair.enabled}, Terminal Canvas = {canvasTerminal.enabled}");
    }

    // Méthodes pour changer spécifiquement de caméra
    public void SwitchToRepairCamera() {
        Debug.Log("Switching back to repair camera.");
        SetActiveCamera(camRepair);
    }

    public void SwitchToTerminalCamera() {
        Debug.Log("Switching to terminal camera.");
        StartCoroutine(AnimateCamera(camTerminal, camRepair.transform.position, backPosition.position, terminalPosition.position,repairCamRotation, terminalCamRotation));
    }

    IEnumerator AnimateCamera(Camera camera, Vector3 startPosition, Vector3 backPosition, Vector3 endPosition, Quaternion startRotation, Quaternion endRotation) 
    {
        Debug.Log("Starting camera animation to terminal.");
        camera.enabled = true;
        float timer = 0.0f;

        // Première phase de l'animation: déplacement vers la position intermédiaire sans changer la rotation
        while (timer < transitionDuration / 2) {
            float progress = timer / (transitionDuration / 2);
            camera.transform.position = Vector3.Lerp(startPosition, backPosition, progress);
            // Maintien de la rotation initiale
            camera.transform.rotation = startRotation;
            timer += Time.deltaTime;
            yield return null;
        }

        // Réinitialisation du timer pour la deuxième phase
        timer = 0.0f;

        // Deuxième phase de l'animation: de la position intermédiaire à la position finale avec rotation
        while (timer < transitionDuration / 2) {
            float progress = timer / (transitionDuration / 2);
            camera.transform.position = Vector3.Lerp(backPosition, endPosition, progress);
            // Interpolation de la rotation de la position arrière vers la position finale
            camera.transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(endPosition - backPosition), endRotation, progress);
            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Animation complete. Camera has reached the terminal.");
        OnCameraReachedTerminal();  // Cette fonction est appelée lorsque la caméra atteint la position finale
    }



    IEnumerator AnimateCameraInverse(Camera camera, Vector3 startTerminal, Vector3 backPosition, Vector3 repairPosition, Quaternion startRotation, Quaternion repairRotation) {
        Debug.Log("Starting camera animation back to repair position.");
        float timer = 0.0f;
        while (timer < transitionDuration / 2) {
            camera.transform.position = Vector3.Lerp(startTerminal, backPosition, timer / (transitionDuration / 2));
            camera.transform.rotation = Quaternion.Lerp(startRotation, Quaternion.LookRotation(backPosition - startTerminal), timer / (transitionDuration / 2));
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0.0f;
        while (timer < transitionDuration / 2) {
            camera.transform.position = Vector3.Lerp(backPosition, repairPosition, timer / (transitionDuration / 2));
            camera.transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(backPosition - repairPosition), repairRotation, timer / (transitionDuration / 2));
            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Animation back to repair position complete.");
        camRepair.enabled = true;
        camera.enabled = false;
    }

    void OnCameraReachedTerminal() {
        // Activez ici le canvas ou le texte pour le terminal
        SetActiveCamera(camTerminal);
        Debug.Log("Camera has reached the terminal.");
        
    }
}