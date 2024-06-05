using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public Canvas titleScreenCanvas;
    public Button startButton;
    public Transform titleCameraPosition;
    public Transform terminalCameraPosition;
    public float transitionDuration = 2.0f; // Duration for the camera transition

    public Camera terminalCamera;
    public Camera repairCamera;

    void Start()
    {
        // Désactiver la caméra de réparation et activer la caméra du terminal
        repairCamera.enabled = false;
        terminalCamera.enabled = true;

        // Positionner la caméra du terminal à la position initiale du titre
        terminalCamera.transform.position = titleCameraPosition.position;
        terminalCamera.transform.rotation = titleCameraPosition.rotation;

        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    void OnStartButtonClicked()
    {
        StartCoroutine(TransitionToTerminal());
    }

    private IEnumerator TransitionToTerminal()
    {
        // Masquer l'écran titre
        titleScreenCanvas.gameObject.SetActive(false);
        float elapsedTime = 0;
        Vector3 startingPos = terminalCamera.transform.position;
        Quaternion startingRot = terminalCamera.transform.rotation;

        while (elapsedTime < transitionDuration)
        {
            terminalCamera.transform.position = Vector3.Lerp(startingPos, terminalCameraPosition.position, elapsedTime / transitionDuration);
            terminalCamera.transform.rotation = Quaternion.Lerp(startingRot, terminalCameraPosition.rotation, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        terminalCamera.transform.position = terminalCameraPosition.position;
        terminalCamera.transform.rotation = terminalCameraPosition.rotation;

        

        // Démarrer le jeu
        GameManager.Instance.StartGame();
    }
}
