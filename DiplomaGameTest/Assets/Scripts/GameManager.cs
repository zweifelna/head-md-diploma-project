using System;
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
    private Dictionary<int, Action> dailyEvents;
    public int currentDay = 1;
    public List<GameObject> discardedObjects = new List<GameObject>();
    public GameObject screenReplacementPrefab;
    public GameObject battery1ReplacementPrefab;
    public GameObject battery2ReplacementPrefab;
    public GameObject battery3ReplacementPrefab;
    public float batteryZOffset = 0.4f;
    public Light mainLight;
    public Light deskLamp;
    public Renderer dayLightZoneRenderer;
    public Gradient dayLightColor; // Dégradé de couleur pour la lumière du jour
    private float elapsedTime = 0f;
    private bool isNight = false;
    public BasicInkExample2 inkStoryManager; // Référence au script Ink

    public GameObject usbKeyPrefab;

    private List<(string, SubState)> repairPlan;
    private int currentRepairIndex;
    private InteractableObject currentObject;
    private Dictionary<int, List<(string, SubState)>> dailyRepairPlans;


    void Start()
    {
        Debug.Log("Starting new day with currentDay: " + currentDay);
        InitializeGame();
        StartWithTerminal();
    }

    void InitializeGame()
    {
        InitializeDailyRepairPlans();
        PrepareRepairPlan();
        LoadNextObject();
        InitializeDailyEvents();
        InitializeInteractableObjects();
        UpdateQuotaDisplay();
        deskLamp.enabled = false;
        deskLamp.intensity = 100f;
        SetQuotaForCurrentDay();
    }

    void Update()
    {
        Debug.Log(currentDay);
        if (timerIsRunning)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= timeRemaining)
            {
                elapsedTime = timeRemaining;
                timerIsRunning = false;
                CheckQuota();
            }

            UpdateDayLightZone();
            UpdateTimerDisplay();
        }

    }

    void InitializeDailyEvents()
    {
        dailyEvents = new Dictionary<int, Action>();

        // Exemple d'ajout d'événements avec plusieurs actions
        dailyEvents.Add(1, () =>
        {
            ShowMessage("Bienvenue au premier jour !");
            AddSpecialObjectToScene();
        });

        dailyEvents.Add(2, () =>
        {
            ShowMessage("Deuxième jour, nouvelle tâche !");
            AddAnotherSpecialObjectToScene();
        });

        dailyEvents.Add(3, () =>
        {
            ShowMessage("Un nouvel événement s'est produit le jour 3 !");
            AddSpecialObjectToScene();
            TriggerSpecialEvent();
        });

        // Ajoutez d'autres événements ici...
    }

    void InitializeDailyRepairPlans()
    {
        dailyRepairPlans = new Dictionary<int, List<(string, SubState)>>()
        {
            { 1, new List<(string, SubState)> 
                { 
                    ("Objet 1", SubState.RepairScreen),
                    ("Objet 2", SubState.RepairScreen)
                } 
            },
            { 2, new List<(string, SubState)> 
                { 
                    ("Objet 1", SubState.ReplaceBatteries),
                    ("Objet 2", SubState.ReplaceBatteries),
                    ("Objet 3", SubState.ReplaceBatteries)
                } 
            },
            { 3, new List<(string, SubState)> 
                { 
                    ("Objet 1", SubState.ReplaceBatteries),
                    ("Objet 2", SubState.RepairScreen),
                    ("Objet 3", SubState.RepairScreen)
                } 
            },
            { 4, new List<(string, SubState)> 
                { 
                    ("Objet 1", SubState.RepairScreen),
                    ("Objet 2", SubState.ReplaceBatteries),
                    ("Objet 3", SubState.RepairScreen)
                } 
            },
            { 35, new List<(string, SubState)> 
                { 
                    ("ObjetAvecUSB", SubState.ReplaceBatteries), // Premier objet contient la clé USB
                    ("Objet 2", SubState.RepairScreen),
                    ("Objet 3", SubState.RepairScreen)
                } 
            },
            // Ajouter d'autres jours ici...
        };
    }

    private void UpdateTimerDisplay()
    {
        timerText.text = $"Temps: {Mathf.Ceil(elapsedTime)}";
    }
    private void UpdateQuotaDisplay()
    {
        quotaText.text = $"Quota: {quota}";
    }
    private void CheckQuota()
    {
        if (score >= quota)
        {
            Debug.Log("quota checked and win");
            TransitionToState(State.Win);  // Transition vers l'état de victoire
        }
        else
        {
            TransitionToState(State.GameOver);  // Transition vers l'état de défaite
        }
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
            case SubState.RepairScreen:
                Debug.Log("Entering RepairScreen substate.");
                StartScreenRepair();
                // Logique pour l'état de réparation de l'écran
                break;
            case SubState.ReplaceBatteries:
                Debug.Log("Entering ReplaceBatteries substate.");
                StartBatteryReplacement();
                // Logique pour l'état de remplacement des batteries
                break;
            case SubState.FullDismantle:
                Debug.Log("Entering FullDismantle substate.");
                // Logique pour l'état de démontage complet
                break;
            // Supprimez les autres cas de substate qui ne sont plus nécessaires
        }
    }

    void HandleGameOver()
    {
        Debug.Log("Game Over!");
        StartCoroutine(EndOfDayRoutine("game_over"));
    }

    void HandleWin()
    {
        Debug.Log("You Win!");
        StartCoroutine(EndOfDayRoutine("quota_achieved"));
        // Passer à la journée suivante
        ResetGameForNextDay();
        //inkStoryManager.RestartStory();
    }

    private IEnumerator EndOfDayRoutine(string knotName)
    {
        // Passer à la caméra du terminal
        CameraManager.Instance.SwitchToTerminalCamera();
        // Mettre à jour le score dans Ink
        inkStoryManager.SetInkVariable("current_score", score);
        inkStoryManager.SetInkVariable("currentDay", currentDay);
        // Commencer l'histoire Ink à partir du nœud spécifié
        Debug.Log("Envoie la demande Ink de commencer à "+knotName);
        if(knotName == "game_over")
        {
            inkStoryManager.StartStoryFromKnot(knotName);
        }else{
            inkStoryManager.StartStory(currentDay);
        }
        


        // Attendre que la caméra du terminal soit active
        yield return new WaitUntil(() => CameraManager.Instance.IsTerminalActive);

    }

    private void ResetGameForNextDay()
    {
        timeRemaining = 60; // Réinitialiser le timer
        timerIsRunning = true;
        score = 0; // Réinitialiser le score
        UpdateQuotaDisplay();
        DestroyObjects();
        ClearDiscardedObjects();
        AdvanceDay();
        SetQuotaForCurrentDay();
        PrepareRepairPlan();
        Debug.Log("charge un nouvel objet jour 2");
        LoadNextObject(); // Charger un nouvel objet
        ResetLighting();
        TransitionToState(State.GameActive); // Retourner à l'état de jeu actif
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
            if(!obj.isMainObject)
            {
                Debug.Log("1");
                if (!obj.isMainObject && !obj.IsSnapped && obj.isBeingRepaired)
                {
                    Debug.Log("2");
                    Debug.Log(obj.gameObject.name + " is not assembled. Current state: " + obj.currentState);
                    allAssembled = false;
                    obj.isDisposable = false;
                    obj.currentState = InteractableObject.ObjectState.Dismantled;
                    break;
                }
            }
        }
        if (allAssembled)
        {
            Debug.Log("3");
            Debug.Log("All objects have been assembled.");
            // Marquer l'objet comme prêt à être jeté
            foreach (InteractableObject obj in allInteractableObjects)
            {
                obj.isDisposable = true;
                if (obj.isMainObject)
                {
                    obj.currentState = InteractableObject.ObjectState.Complete;
                    obj.isDisposable = true;
                    obj.isRepaired = true;
                    Debug.Log("Main object marked as disposable and repaired.");
                }
            }
        }
    }

    private void IncreaseScoreAndLoadNewObject()
    {
        score++;
        scoreText.text = $"Score: {score}";
        //LoadNewObject(); // Charge ou active un nouvel objet pour interaction
    }

    void InitializeInteractableObjects()
    {
        allInteractableObjects.Clear();
        // Trouve tous les objets de type InteractableObject dans la scène
        allInteractableObjects = new List<InteractableObject>(FindObjectsOfType<InteractableObject>());
        Debug.Log("Found " + allInteractableObjects.Count + " interactable objects.");
        foreach (InteractableObject obj in allInteractableObjects)
        {
            if (obj.transform.parent != null)
            {
                obj.IsSnapped = true;  // Initialiser isSnapped à true si l'objet a un parent InteractableObject
            }
            else
            {
                obj.IsSnapped = false;
            }
        }
    }

    // public void LoadNewObject()
    // {
    //     RemoveAllObjects();
    //     Vector3 spawnPosition = new Vector3(-1.73f, 1.19f, -3.04f);
    //     Quaternion spawnRotation = Quaternion.Euler(0, 0, -90);

    //     GameObject newObject = Instantiate(interactableObjectPrefab, spawnPosition, spawnRotation);
    //     InteractableObject newInteractableObject = newObject.GetComponent<InteractableObject>();

    //     if (newInteractableObject != null)
    //     {
    //         InitializeInteractableObjects();
    //         Debug.Log("New interactable object loaded and initialized.");
    //         dismantlingCompleted = false;
    //     }
    // }

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
        Debug.Log("Found " + allInteractableObjects.Count + " interactable objects.");
    }

    private void DestroyObjects()
    {
        if (InteractionController.instance != null) {
            InteractionController.instance.ClearReferences();
            Debug.Log("Les références sont cleared");
        }

        foreach (InteractableObject obj in allInteractableObjects)
        {
            if (obj.isMainObject)
            {
                // Supprime ou désactive l'objet principal et tous ses enfants
                DestroyEntireStructure(obj.gameObject);
                Debug.Log("L'objet principal et ses enfants sont supprimés");
            }
            else
            {
                // Supprime ou désactive les objets normalement
                GameObject.Destroy(obj.gameObject);
                Debug.Log("Un objet est supprimé");
            }
        }
        allInteractableObjects.Clear(); // Nettoyer la liste
        Debug.Log("La liste d'objet est nettoyée");
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

    // public void ObjectSelected()
    // {
    //     SetSubState(SubState.ObjectSelected);
    // }


    // private void PrepareForPhoneShaking()
    // {
    //     SetSubState(SubState.PhoneShaking);
    // }

    // private bool DetectPhoneShake()
    // {
    //     // Seuil de secousse, ajustez cette valeur en fonction de la sensibilité désirée
    //     float shakeDetectionThreshold = 2.5f;

    //     // Accélération instantanée
    //     Vector3 acceleration = Input.acceleration;

    //     // Vérifier si l'accélération dépasse un certain seuil
    //     if (acceleration.sqrMagnitude >= shakeDetectionThreshold * shakeDetectionThreshold) {
    //         Debug.Log("Shake detected");
    //         GameManager.Instance.SetSubState(GameManager.SubState.PhoneShaking); // Passer au substate suivant
    //         return true;
    //     }

    //     // Simulation de secouement en appuyant sur le scoreText dans Unity Editor
    //     #if UNITY_EDITOR
    //     if (Input.GetMouseButtonDown(0)) {
    //         Vector2 pos = Input.mousePosition;
    //         if (RectTransformUtility.RectangleContainsScreenPoint(scoreText.rectTransform, pos, null)) {
    //             Debug.Log("Shake simulated by clicking on scoreText");
    //             GameManager.Instance.SetSubState(GameManager.SubState.PhoneShaking); // Passer au substate suivant
    //             return true;
    //         }
    //     }
    //     #endif

    //     // Simulation de secouement en appuyant sur la touche "S"
    //     if (Input.GetKeyDown(KeyCode.S))
    //     {
    //         Debug.Log("Shake simulated by pressing 'S' key");
    //         GameManager.Instance.SetSubState(GameManager.SubState.PhoneShaking); // Passer au substate suivant
    //         return true;
    //     }

    //     return false;
    // }

    void InitializeVibrationPatterns() 
    {
        // Exemple de motifs de vibration
        vibrationPatterns.Add(new List<float> { 0.1f, 0.1f, 0.1f });  // Vibre 0.1 sec, pause 0.1 sec, vibre 0.1 sec
        vibrationPatterns.Add(new List<float> { 0.2f, 0.1f, 0.2f, 0.1f, 0.2f });
    }
    private void PlayVibrationPattern() 
    {
        selectedPatternIndex = UnityEngine.Random.Range(0, vibrationPatterns.Count);
        //selectedPatternIndex = 0;
        AudioManager.Instance.Play(AudioManager.Instance.sounds[selectedPatternIndex].name);
        //StartCoroutine(PlayPattern(vibrationPatterns[selectedPatternIndex]));
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
        InteractableObject oldScreen = FindScreenObject();
        if (oldScreen != null)
        {
            Debug.Log("oldscreen found");
            // Définir isRepaired à false pour l'écran initial
            oldScreen.isRepaired = false;
            oldScreen.isBeingRepaired = true;
            // Instancier le nouvel écran à remplacer
            Vector3 newPosition = oldScreen.initialPosition - new Vector3(0, 0, (batteryZOffset*3));
            GameObject newScreen = Instantiate(screenReplacementPrefab, newPosition, oldScreen.initialRotation);
            InteractableObject newScreenInteractable = newScreen.GetComponent<InteractableObject>();

            if (newScreenInteractable != null)
            {
                newScreenInteractable.isRepaired = true; // Le nouvel écran est marqué comme réparé
            }

            // Ajouter le nouvel écran à la liste des objets interactables
            allInteractableObjects.Add(newScreenInteractable);

            newScreenInteractable.OnSnapped += () =>
        {
            oldScreen.isBeingRepaired = false;
            oldScreen.isDisposable = false;
            oldScreen.currentState = InteractableObject.ObjectState.Dismantled;
            Debug.Log("Old screen marked as dismantled.");
            CheckIfAllAssembled();
        };
        }
        else
        {
            Debug.LogError("Screen object not found!");
        }
    }

    InteractableObject FindScreenObject()
    {
        foreach (InteractableObject obj in allInteractableObjects)
        {
            if (obj.gameObject.name == "Ecran")
            {
                return obj;
            }
        }
        return null;
    }

    void StartBatteryReplacement()
    {
        Debug.Log("Starting battery replacement sequence.");
        List<InteractableObject> oldBatteries = FindBatteryObjects();
        Debug.Log(oldBatteries.Count);
        if (oldBatteries.Count >= 3)
        {
            // Vérifiez si c'est le jour 35 et le premier objet est réparé
            Debug.Log("C'est le "+currentDay+" jour");
            Debug.Log("Repair Index : "+ currentRepairIndex);
            if (currentDay == 2)
            {
                
                SpawnUSBKey(oldBatteries[0].transform.parent);
                Debug.Log("USB spawned");
            }
            Debug.Log("On continue avec la boucle for");
            for (int i = 0; i < oldBatteries.Count; i++)
            {
                InteractableObject oldBattery = oldBatteries[i];
                oldBattery.isRepaired = false;
                oldBattery.isBeingRepaired = true;

                GameObject newBatteryPrefab = null;
                switch (i)
                {
                    case 0:
                        newBatteryPrefab = battery1ReplacementPrefab;
                        break;
                    case 1:
                        newBatteryPrefab = battery2ReplacementPrefab;
                        break;
                    case 2:
                        newBatteryPrefab = battery3ReplacementPrefab;
                        break;
                }

                if (newBatteryPrefab != null)
                {
                    Vector3 newPosition = oldBattery.initialPosition + new Vector3(0, 0, batteryZOffset);
                    GameObject newBattery = Instantiate(newBatteryPrefab, newPosition, oldBattery.initialRotation);
                    InteractableObject newBatteryInteractable = newBattery.GetComponent<InteractableObject>();

                    if (newBatteryInteractable != null)
                    {
                        newBatteryInteractable.isRepaired = true;
                        newBatteryInteractable.initialPosition = newBattery.transform.position;
                        newBatteryInteractable.initialRotation = newBattery.transform.rotation;
                    }

                    allInteractableObjects.Add(newBatteryInteractable);

                    newBatteryInteractable.OnSnapped += () =>
                    {
                        oldBattery.isBeingRepaired = false;
                        oldBattery.isDisposable = false;
                        oldBattery.currentState = InteractableObject.ObjectState.Dismantled;
                        Debug.Log("Old battery marked as dismantled.");
                        CheckIfAllAssembled();
                    };
                }
            }
        }
        else
        {
            Debug.LogError("Not enough batteries found!");
        }
    }

    List<InteractableObject> FindBatteryObjects()
    {
        List<InteractableObject> batteries = new List<InteractableObject>();
        foreach (InteractableObject obj in allInteractableObjects)
        {
            if (obj.gameObject.name.Contains("Pile"))
            {
                batteries.Add(obj);
            }
        }
        return batteries;
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

    public void CompleteRepairProcess()
    {
        score++;
        Debug.Log("LE SCORE EST DE " + score);

        // Vérifiez si le score est inférieur au quota avant d'appeler LoadNextObject
        Debug.Log("Score : "+score+" et quota : "+quota);
        if (score < quota)
        {
            LoadNextObject();
        }
        else{
            CheckQuota();
        }
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

    public void AdvanceDay()
    {
        currentDay++;
        Debug.Log("Ending day. New currentDay: " + currentDay);
        //inkStoryManager.StartStory(currentDay);
        TriggerDailyEvent(currentDay);
    }

    void TriggerDailyEvent(int day)
    {
        if (dailyEvents.ContainsKey(day))
        {
            dailyEvents[day]?.Invoke();
        }
    }

    void ShowMessage(string message)
    {
        // Affiche un message à l'écran
        Debug.Log(message);
        // Ajoutez ici votre logique pour afficher le message dans l'UI du jeu
    }

    void AddSpecialObjectToScene()
    {
        // Ajouter ici la logique pour ajouter un objet spécial à la scène
        Debug.Log("Ajout d'un objet spécial à la scène pour le jour " + currentDay);
        // Exemple : Instantiate(specialObjectPrefab, spawnPosition, Quaternion.identity);
    }

    void AddAnotherSpecialObjectToScene()
    {
        // Ajouter ici la logique pour ajouter un autre objet spécial à la scène
        Debug.Log("Ajout d'un autre objet spécial à la scène pour le jour " + currentDay);
        // Exemple : Instantiate(anotherSpecialObjectPrefab, anotherSpawnPosition, Quaternion.identity);
    }

    void TriggerSpecialEvent()
    {
        // Logique pour déclencher un événement spécial
        Debug.Log("Événement spécial déclenché pour le jour " + currentDay);
        // Exemple : specialEventManager.TriggerEvent(specialEventID);
    }

    private void ClearDiscardedObjects()
    {
        foreach (GameObject discardedObject in discardedObjects)
        {
            Destroy(discardedObject);
        }
        discardedObjects.Clear();
    }

    // public void InitializeObjectSelection()
    // {
    //     SetSubState(SubState.ObjectSelected);
    // }

     void UpdateDayLightZone()
    {
        float dayProgress = elapsedTime / timeRemaining;

        // Modifier la couleur de la zone lumineuse en fonction de l'heure
        dayLightZoneRenderer.material.color = dayLightColor.Evaluate(dayProgress);

        // Gérer la diminution de l'intensité de la lumière principale vers la fin
        if (dayProgress >= 0.8f)
        {
            float remainingTimeFraction = (timeRemaining - elapsedTime) / (timeRemaining * 0.2f); // Fraction du temps restant
            mainLight.intensity = Mathf.Lerp(0.5f, 1f, remainingTimeFraction); // Diminuer l'intensité de 1 à 0.5
        }

        // Gérer l'allumage de la lampe de bureau
        if (dayProgress >= 0.8f && !isNight)
        {
            //deskLamp.enabled = true;
            isNight = true;
        }
        else if (dayProgress < 0.8f && isNight)
        {
            //deskLamp.enabled = false;
            isNight = false;
        }
    }

    private void ResetLighting()
    {
        elapsedTime = 0f; // Réinitialise le temps écoulé
        isNight = false; // Réinitialise l'état de nuit
        mainLight.intensity = 1f; // Réinitialise l'intensité de la lumière principale
        deskLamp.enabled = false; // Désactive la lampe de bureau
        dayLightZoneRenderer.material.color = dayLightColor.Evaluate(0f); // Réinitialise la couleur de la zone lumineuse
    }

    public void StartWithTerminal()
    {
        // Afficher le terminal avec le texte du jour basé sur currentDay
        CameraManager.Instance.StartWithTerminalCamera();
        inkStoryManager.StartStory(currentDay);
    }


    public void StartTimer()
    {
        timerIsRunning = true;
    }

    void PrepareRepairPlan()
    {
        if (dailyRepairPlans.ContainsKey(currentDay))
        {
            repairPlan = dailyRepairPlans[currentDay];
            currentRepairIndex = 0;
        }
        else
        {
            Debug.LogError($"No repair plan found for day {currentDay}");
        }
    }

    InteractableObject CreateInteractableObject(string name)
    {
        // Logique pour créer et initialiser un objet interactif
        GameObject newObject = Instantiate(interactableObjectPrefab);
        InteractableObject interactableObject = newObject.GetComponent<InteractableObject>();
        interactableObject.name = name;
        return interactableObject;
    }

    void LoadNextObject()
    {
        if (currentRepairIndex < repairPlan.Count)
        {
            allInteractableObjects.Remove(currentObject); // Supprimer l'objet de la liste
            
            var nextRepair = repairPlan[currentRepairIndex];
            currentObject = CreateInteractableObject(nextRepair.Item1);
            SubState subState = nextRepair.Item2;

            currentObject.gameObject.SetActive(true); // Activez le prochain objet
            InitializeInteractableObjects();

            currentRepairIndex++;
            SetSubState(subState);
        }
        else
        {
            // Quota atteint, passez à la fin de la journée ou autre logique
            CheckQuota();
        }
    }

    void SpawnUSBKey(Transform parent)
    {
        GameObject usbKey = Instantiate(usbKeyPrefab, parent);
        usbKey.transform.localPosition = new Vector3(0, 0, 0); // Ajustez cette position en fonction de vos besoins
        usbKey.transform.localRotation = Quaternion.identity;
        usbKey.GetComponent<InteractableObject>().isDisposable = true; // Assurez-vous que la clé USB peut être ramassée
    }

    void SetQuotaForCurrentDay()
    {
        if (dailyRepairPlans.ContainsKey(currentDay))
        {
            Debug.Log("Set du quota : "+dailyRepairPlans[currentDay].Count);
            quota = dailyRepairPlans[currentDay].Count;
        }
        else
        {
            quota = 0;
        }
    }
}