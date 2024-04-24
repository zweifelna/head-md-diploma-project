using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Optionnel, pour garder le manager actif entre les scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum State
    {
        State1,
        State2,
        GameOver,
        Win
    }

    public State currentState { get; private set; }
    public float timeRemaining = 60;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI quotaText;
    public int quota = 2;
    public int score = 0;
    public List<InteractableObject> allInteractableObjects = new List<InteractableObject>();
    public GameObject interactableObjectPrefab;
    public bool dismantlingCompleted = false;

    void Start()
    {
        timerIsRunning = true;
        SpawnNewObject();
        InitializeInteractableObjects();
        UpdateQuotaDisplay();
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
                //CheckIfAllDismantled();
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                CheckQuota();  
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        timerText.text = $"Temps: {Mathf.Ceil(timeRemaining)}";
    }
    private void UpdateQuotaDisplay()
    {
        quotaText.text = $"Quota: {quota}";
    }
    private void CheckQuota()
    {
        if (score >= quota)
        {
            TransitionToState(State.Win);  // Transition vers l'état de victoire
        }
        else
        {
            TransitionToState(State.GameOver);  // Transition vers l'état de défaite
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
            case State.Win:
                HandleWin();
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
        timerText.text = "You're fired!";
        // Afficher un écran de fin, enregistrer le score, etc.
    }

    void HandleWin()
    {
        Debug.Log("You Win!");
        timerText.text = "You're not fired!";
        // Ajoutez ici toute action de victoire, comme sauvegarder le score, afficher des animations, etc.
    }

    public void CheckIfAllDismantled() 
    {
        if (dismantlingCompleted) return; // Maintenant que vous gérez cela mieux, assurez-vous que cette logique est toujours nécessaire.

        bool allDismantled = true;
        foreach (InteractableObject obj in allInteractableObjects) {
            if (obj.currentState != InteractableObject.ObjectState.Dismantled) {
                Debug.Log(obj.gameObject.name + " is not dismantled. Current state: " + obj.currentState);
                allDismantled = false;
                break; // Bonne utilisation du break pour optimiser la boucle
            }
        }

        if (allDismantled) {
            Debug.Log("All objects have been dismantled.");
            dismantlingCompleted = true;
            RemoveAllObjects();
            IncreaseScoreAndLoadNewObject();
        } else {
            Debug.Log("Not all objects are dismantled.");
        }
    }

    private void IncreaseScoreAndLoadNewObject()
    {
        score++;
        scoreText.text = $"Score: {score}";
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

    public void LoadNewObject()
    {
        Vector3 spawnPosition = new Vector3(-1.73f, 1.19f, -3.04f); // Définissez la position de spawn
        Quaternion spawnRotation = Quaternion.Euler(0, 0, -90);

        GameObject newObject = Instantiate(interactableObjectPrefab, spawnPosition, spawnRotation);
        InteractableObject newInteractableObject = newObject.GetComponent<InteractableObject>();

        if (newInteractableObject != null)
        {
            InitializeInteractableObjects();
            Debug.Log("New interactable object loaded and initialized.");
            dismantlingCompleted = false;
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

    private void RemoveAllObjects()
    {
        if (InteractionController.instance != null) {
            InteractionController.instance.ClearReferences();
        }

        foreach (InteractableObject obj in allInteractableObjects)
        {
            if (obj.isMainObject)
            {
                // Supprime ou désactive l'objet principal et tous ses enfants
                DestroyEntireStructure(obj.gameObject);
            }
            else
            {
                // Supprime ou désactive les objets normalement
                GameObject.Destroy(obj.gameObject);
            }
        }
        allInteractableObjects.Clear(); // Nettoyer la liste
    }

    private void DestroyEntireStructure(GameObject mainObject)
    {
        foreach (Transform child in mainObject.transform)
        {
            GameObject.Destroy(child.gameObject); // Supprime tous les enfants
        }
        GameObject.Destroy(mainObject); // Supprime l'objet principal
    }

    public void RemoveInteractableObject(InteractableObject obj)
    {
        if (allInteractableObjects.Contains(obj))
        {
            allInteractableObjects.Remove(obj);
            Debug.Log("Removed interactable object: " + obj.name);
        }
    }
}