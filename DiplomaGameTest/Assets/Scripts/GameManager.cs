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

    void Start()
    {
        InitializeDailyEvents();
        timerIsRunning = true;
        InitializeInteractableObjects();
        InitializeVibrationPatterns();
        UpdateQuotaDisplay();
        deskLamp.enabled = false;
        deskLamp.intensity = 100f;
    }

    void Update()
    {
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
        CameraManager.Instance.DisplayTerminalMessage($"You're fired!\nScore: {score}/{quota}");
        // Afficher un écran de fin, enregistrer le score, etc.
    }

    void HandleWin()
    {
        Debug.Log("You Win!");
        CameraManager.Instance.DisplayTerminalMessage($"Quota achieved!\nScore: {score}/{quota}");
        
        // Réinitialiser le timer et le score
        StartCoroutine(WaitForTerminalDisplay());
    }

    private IEnumerator WaitForTerminalDisplay()
    {
        // Attendre la fin de l'affichage du terminal
        yield return new WaitUntil(() => !CameraManager.Instance.IsTerminalActive);

        // Réinitialiser le timer, le score et charger un nouvel objet
        ResetGameForNextDay();
    }

    private void ResetGameForNextDay()
    {
        timeRemaining = 60; // Réinitialiser le timer
        timerIsRunning = true;
        score = 0; // Réinitialiser le score
        quota += 2; // Mettre à jour le quota pour le nouveau jour
        UpdateQuotaDisplay();
        DestroyObjects();
        ClearDiscardedObjects();
        LoadNewObject(); // Charger un nouvel objet
        AdvanceDay();
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
        LoadNewObject(); // Charge ou active un nouvel objet pour interaction
    }

    void InitializeInteractableObjects()
    {
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

        // Simulation de secouement en appuyant sur la touche "S"
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Shake simulated by pressing 'S' key");
            GameManager.Instance.SetSubState(GameManager.SubState.PhoneShaking); // Passer au substate suivant
            return true;
        }

        return false;
    }

    void InitializeVibrationPatterns() 
    {
        // Exemple de motifs de vibration
        vibrationPatterns.Add(new List<float> { 0.1f, 0.1f, 0.1f });  // Vibre 0.1 sec, pause 0.1 sec, vibre 0.1 sec
        vibrationPatterns.Add(new List<float> { 0.2f, 0.1f, 0.2f, 0.1f, 0.2f });
    }
    private void PlayVibrationPattern() 
    {
        selectedPatternIndex = UnityEngine.Random.Range(0, vibrationPatterns.Count);
        //selectedPatternIndex = 1;
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
            // Définir isRepaired à false pour l'écran initial
            oldScreen.isRepaired = false;
            oldScreen.isBeingRepaired = true;
            // Instancier le nouvel écran à remplacer
            GameObject newScreen = Instantiate(screenReplacementPrefab, oldScreen.initialPosition, oldScreen.initialRotation);
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
        if (oldBatteries.Count == 3)
        {
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

    public void AdvanceDay()
    {
        currentDay++;
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

    public void InitializeObjectSelection()
    {
        SetSubState(SubState.ObjectSelected);
    }

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
            deskLamp.enabled = true;
            isNight = true;
        }
        else if (dayProgress < 0.8f && isNight)
        {
            deskLamp.enabled = false;
            isNight = false;
        }
    }
}