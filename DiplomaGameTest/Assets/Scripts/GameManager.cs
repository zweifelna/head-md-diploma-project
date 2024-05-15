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
        GameActive,
        GamePause,
        GameOver,
        Win
    }

    public enum SubState
    {
        None,
        ObjectSelected,
        PhoneShaking,
        VibrationPatternPlaying,
        ActionResponse,
        RepairScreen,
        ReplaceBatteries,
        FullDismantle
    }

    public State currentState;
    public SubState currentSubState;
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
    public List<List<float>> vibrationPatterns = new List<List<float>>();
    public int selectedPatternIndex = -1;  // Indice du motif sélectionné

    void Start()
    {
        timerIsRunning = true;
        SpawnNewObject();
        InitializeInteractableObjects();
        InitializeVibrationPatterns();
        UpdateQuotaDisplay();
    }

    void Update()
    {
        // if (Input.GetMouseButtonDown(0)) // Vérifie si le bouton gauche de la souris est cliqué
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;

        //     if (Physics.Raycast(ray, out hit, 100.0f)) // 100.0f est la distance maximale du raycast
        //     {
        //         Debug.Log("Hit: " + hit.collider.gameObject.name); // Log le nom de l'objet touché

        //         if (hit.collider.gameObject.name == "Keyboard")
        //         {
        //             Debug.Log("Keyboard clicked");
        //             // Change de caméra ou effectue l'action désirée
        //         }
        //         else if (hit.collider.gameObject.CompareTag("Interactable"))
        //         {
        //             Debug.Log("Interactable object selected: " + hit.collider.gameObject.name);
        //         }
        //     }
        //     else
        //     {
        //         Debug.Log("No object hit by raycast");
        //     }
        // }

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

        switch (currentSubState)
        {
            case SubState.PhoneShaking:
                // Logique de détection du secouement du téléphone
                if (DetectPhoneShake())
                {
                    SetSubState(SubState.VibrationPatternPlaying); // Méthode hypothétique pour gérer la suite des actions
                }
                break;
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
            case State.GameActive:
                // Initialiser et démarrer le timer ici si nécessaire
                timeRemaining = 60; // Réinitialiser le timer
                timerIsRunning = true;
                break;
            case State.GamePause:
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
            case State.GameActive:
                // Nettoyage ou actions de fin d'état
                break;
            case State.GamePause:
                // Préparation à quitter State2
                break;
            case State.GameOver:
            // Préparation à quitter GameOver, si nécessaire
            break;
        }
    }

    private void SetSubState(SubState subState)
    {
        currentSubState = subState;
        HandleSubStateEntry(subState);
    }

    private void HandleSubStateEntry(SubState subState)
    {
        switch (subState)
        {
            case SubState.ObjectSelected:
                Debug.Log("Object has been selected. Ready for phone shaking.");
                // Configurer le prochain sous-état après une certaine action ou un certain délai
                Invoke("PrepareForPhoneShaking", 2.0f); // Exemple: attendre 2 secondes avant de passer à PhoneShaking
                break;

            case SubState.VibrationPatternPlaying:
                PlayVibrationPattern();
                TransitionToRepairSubState(selectedPatternIndex);
                break;

            case SubState.RepairScreen:
                // Logique pour démonter et replacer l'écran
                StartScreenRepair();
                break;

            case SubState.ReplaceBatteries:
                // Logique pour remplacer les trois piles
                StartBatteryReplacement();
                break;

            case SubState.FullDismantle:
                // Logique pour démonter et remplacer tout
                StartFullDismantle();
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

        if (allDismantled)
        {
            Debug.Log("All objects have been dismantled.");
            foreach (InteractableObject obj in allInteractableObjects)
            {
                obj.MarkAsRepaired();
            }
        } else {
            Debug.Log("Not all objects are dismantled.");
        }
    }

    public void CheckIfAllAssembled()
    {
        bool allAssembled = true;
        foreach (InteractableObject obj in allInteractableObjects)
        {
            if (obj.currentState != InteractableObject.ObjectState.Complete)
            {
                allAssembled = false;
                break;
            }
        }

        if (allAssembled)
        {
            Debug.Log("All objects have been assembled.");
            // Marquer l'objet comme prêt à être jeté
            foreach (InteractableObject obj in allInteractableObjects)
            {
                obj.isDisposable = true;
            }
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
        //PrintAllInteractableObjectNames();

        // Vous pouvez initialiser d'autres choses ici si nécessaire
    }

    public void LoadNewObject()
    {
        RemoveAllObjects();
        Vector3 spawnPosition = new Vector3(-1.73f, 1.19f, -3.04f);
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

    // public void PrintAllInteractableObjectNames()
    // {
    //     Debug.Log("Currently " + allInteractableObjects.Count + " interactable objects in the scene:");
    //     foreach (InteractableObject interactableObject in allInteractableObjects)
    //     {
    //         Debug.Log("Interactable Object: " + interactableObject.gameObject.name);
    //     }
    // }

    private void RemoveAllObjects()
    {
        if (InteractionController.instance != null) {
            InteractionController.instance.ClearReferences();
        }

        // foreach (InteractableObject obj in allInteractableObjects)
        // {
        //     if (obj.isMainObject)
        //     {
        //         // Supprime ou désactive l'objet principal et tous ses enfants
        //         DestroyEntireStructure(obj.gameObject);
        //     }
        //     else
        //     {
        //         // Supprime ou désactive les objets normalement
        //         GameObject.Destroy(obj.gameObject);
        //     }
        // }
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

    public void ObjectSelected()
    {
        SetSubState(SubState.ObjectSelected);
    }


    private void PrepareForPhoneShaking()
    {
        SetSubState(SubState.PhoneShaking);
    }

    private bool DetectPhoneShake()
    {
        // Seuil de secousse, ajustez cette valeur en fonction de la sensibilité désirée
        float shakeDetectionThreshold = 2.5f;

        // Accélération instantanée
        Vector3 acceleration = Input.acceleration;

        // Vérifier si l'accélération dépasse un certain seuil
        if (acceleration.sqrMagnitude >= shakeDetectionThreshold * shakeDetectionThreshold) {
            Debug.Log("Shake detected");
            GameManager.Instance.SetSubState(GameManager.SubState.PhoneShaking); // Passer au substate suivant
            return true;
        }

        // Simulation de secouement en appuyant sur le scoreText dans Unity Editor
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) {
            Vector2 pos = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint(scoreText.rectTransform, pos, null)) {
                Debug.Log("Shake simulated by clicking on scoreText");
                GameManager.Instance.SetSubState(GameManager.SubState.PhoneShaking); // Passer au substate suivant
                return true;
            }
        }
        #endif

        return false;
    }

    void InitializeVibrationPatterns() 
    {
        // Exemple de motifs de vibration
        vibrationPatterns.Add(new List<float> { 0.1f, 0.1f, 0.1f });  // Vibre 0.1 sec, pause 0.1 sec, vibre 0.1 sec
        vibrationPatterns.Add(new List<float> { 0.2f, 0.1f, 0.2f, 0.1f, 0.2f });
        vibrationPatterns.Add(new List<float> { 0.3f, 0.15f, 0.3f });
    }
    private void PlayVibrationPattern() 
    {
        selectedPatternIndex = Random.Range(0, vibrationPatterns.Count);
        StartCoroutine(PlayPattern(vibrationPatterns[selectedPatternIndex]));
    }
    IEnumerator PlayPattern(List<float> pattern) 
    {
        for (int i = 0; i < pattern.Count; i++) {
            if (i % 2 == 0) {  // Indices pairs sont les vibrations
                VibrateForDuration(pattern[i]);
            } else {  // Indices impairs sont les pauses
                yield return new WaitForSeconds(pattern[i]);
            }
        }
    }

    void VibrateForDuration(float duration) 
    {
        Debug.Log("Vibrating for: " + duration + " seconds");
        // Utilisez la méthode appropriée pour déclencher une vibration sur le dispositif
        #if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
        #endif
        // NOTE: Handheld.Vibrate() n'autorise pas des durées personnalisées sans accès natif sur iOS/Android
        // Vous devrez utiliser des plugins ou des appels natifs pour des vibrations personnalisées.
    }
    
    public void TransitionToRepairSubState(int patternIndex)
    {
        switch (patternIndex)
        {
            case 0:
                SetSubState(SubState.RepairScreen);
                break;
            case 1:
                SetSubState(SubState.ReplaceBatteries);
                break;
            case 2:
                SetSubState(SubState.FullDismantle);
                break;
            default:
                Debug.LogError("Unknown pattern index: " + patternIndex);
                break;
        }
    }

    void StartScreenRepair()
    {
        Debug.Log("Starting screen repair sequence.");
        // Implémenter la logique de vérification des conditions de réparation
        // if (CheckRepairCompletion()) // Supposons que cette méthode vérifie si la réparation est correcte
        // {
        //     CompleteRepairProcess();
        // }
    }

    void StartBatteryReplacement()
    {
        Debug.Log("Starting battery replacement sequence.");
        // Implémenter la logique de vérification des conditions de réparation
        // if (CheckRepairCompletion())
        // {
        //     CompleteRepairProcess();
        // }
    }

    void StartFullDismantle()
    {
        Debug.Log("Starting full dismantle sequence.");
        // Implémenter la logique de vérification des conditions de réparation
        // if (CheckRepairCompletion())
        // {
        //     CompleteRepairProcess();
        // }
    }

    void CompleteRepairProcess()
    {
        score++;
        scoreText.text = $"Score: {score}";
        LoadNewObject(); // Charge un nouvel objet
        SetSubState(SubState.ObjectSelected); // Retour à l'état initial pour sélectionner le nouvel objet
    }

    bool CheckRepairCompletion(InteractableObject targetObject)
    {
        if (targetObject.currentState == InteractableObject.ObjectState.Dismantled && !targetObject.IsSnapped)
        {
            Debug.Log($"{targetObject.gameObject.name} is removed.");
            targetObject.currentState = InteractableObject.ObjectState.Following; // Marque l'étape intermédiaire
            return false;
        }
        else if (targetObject.currentState == InteractableObject.ObjectState.Following && targetObject.IsSnapped)
        {
            Debug.Log($"{targetObject.gameObject.name} is replaced.");
            targetObject.currentState = InteractableObject.ObjectState.Complete;
            return true;
        }
        return false;
    }

}