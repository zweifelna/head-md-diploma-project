using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        State1,
        State2,
        GameOver
    }

    public State currentState { get; private set; }
    public float timeRemaining = 60;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public int score = 0;
    public List<InteractableObject> allInteractableObjects = new List<InteractableObject>();
    public GameObject interactableObjectPrefab;

    void Start()
    {
        timerIsRunning = true;
        SpawnNewObject();
        InitializeInteractableObjects();
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                TransitionToState(State.GameOver); // Transition vers GameOver au lieu de EndGame()
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        timerText.text = $"Temps: {Mathf.Ceil(timeRemaining)}";
    }

    public void CheckActionSuccess()
    {
        // Ici, vérifiez si les actions effectuées par le joueur avec l'objet correspondent aux critères de réussite
        if (true) // Remplacez cela par votre condition réelle
        {
            score++;
            scoreText.text = $"Score: {score}";
            SpawnNewObject();
        }
    }

    void SpawnNewObject()
    {
        // Logique pour détruire ou désactiver l'objet actuel et en créer un nouveau
        Debug.Log("Spawning new object");
        // Assurez-vous d'initialiser correctement le nouvel objet ici
    }

    public void TransitionToState(State newState)
    {
        State tmpInitialState = currentState;
        print("Transitioning from " + tmpInitialState + " to " + newState);
        OnStateExit(tmpInitialState, newState);
        currentState = newState;
        OnStateEnter(newState, tmpInitialState);
    }
        
    public void OnStateEnter(State state, State fromState)
    {
        switch (state)
        {
            case State.State1:
                // Initialiser et démarrer le timer ici si nécessaire
                timeRemaining = 60; // Réinitialiser le timer
                timerIsRunning = true;
                break;
            case State.State2:
                // Actions spécifiques à l'entrée dans State2
                timerIsRunning = false; // Arrêter le timer
                break;
            case State.GameOver:
            HandleGameOver();
            break;
        }
    }
        
    public void OnStateExit(State state, State toState)
    {
        switch (state)
        {
            case State.State1:
                // Nettoyage ou actions de fin d'état
                break;
            case State.State2:
                // Préparation à quitter State2
                break;
            case State.GameOver:
            // Préparation à quitter GameOver, si nécessaire
            break;
        }
    }

    void HandleGameOver()
    {
        Debug.Log("Game Over!");
        timerText.text = "Time's up!";
        // Afficher un écran de fin, enregistrer le score, etc.
    }

    public void CheckIfAllDismantled()
    {
        foreach (InteractableObject obj in allInteractableObjects) // Assurez-vous de maintenir une liste des objets interactifs
        {
            if (obj.currentState != InteractableObject.ObjectState.Dismantled)
                return;
        }
        // Si tous les objets sont démantelés
        Debug.Log("All objects have been dismantled.");
        IncreaseScoreAndLoadNewObject();
    }

    private void IncreaseScoreAndLoadNewObject()
    {
        score++;
        Debug.Log("Score: " + score);
        LoadNewObject(); // Charge ou active un nouvel objet pour interaction
    }

    void InitializeInteractableObjects()
    {
        // Trouve tous les objets de type InteractableObject dans la scène
        allInteractableObjects = new List<InteractableObject>(FindObjectsOfType<InteractableObject>());
        Debug.Log("Found " + allInteractableObjects.Count + " interactable objects.");
        PrintAllInteractableObjectNames();

        // Vous pouvez initialiser d'autres choses ici si nécessaire
    }

    public void AddInteractableObject(InteractableObject newObj)
    {
        if (!allInteractableObjects.Contains(newObj))
        {
            allInteractableObjects.Add(newObj);
            Debug.Log("Added new interactable object: " + newObj.name);
        }
    }

    public void RemoveInteractableObject(InteractableObject obj)
    {
        if (allInteractableObjects.Contains(obj))
        {
            allInteractableObjects.Remove(obj);
            Debug.Log("Removed interactable object: " + obj.name);
        }
    }

    public void LoadNewObject()
    {
        Vector3 spawnPosition = new Vector3(0, 0, 0); // Définissez la position de spawn
        Quaternion spawnRotation = Quaternion.identity; // Rotation initiale

        GameObject newObject = Instantiate(interactableObjectPrefab, spawnPosition, spawnRotation);
        InteractableObject newInteractableObject = newObject.GetComponent<InteractableObject>();

        if (newInteractableObject != null)
        {
            allInteractableObjects.Add(newInteractableObject);
            Debug.Log("New interactable object loaded and initialized.");
        }
    }

    public void PrintAllInteractableObjectNames()
{
    Debug.Log("Currently " + allInteractableObjects.Count + " interactable objects in the scene:");
    foreach (InteractableObject interactableObject in allInteractableObjects)
    {
        Debug.Log("Interactable Object: " + interactableObject.gameObject.name);
    }
}
}