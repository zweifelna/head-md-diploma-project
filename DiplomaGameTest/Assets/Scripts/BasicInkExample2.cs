using System;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BasicInkExample2 : MonoBehaviour {
    public static event Action<Story> OnCreateStory;

    [SerializeField]
    private TextAsset inkJSONAsset = null;
    public Story story;

    [SerializeField]
    private Canvas canvas = null;

    // UI Prefabs
    [SerializeField]
    private TextMeshProUGUI textPrefab = null;
    [SerializeField]
    private Button buttonPrefab = null;
    [SerializeField]
    private float textSpeed = 0.05f; // Vitesse d'affichage du texte
    public AudioManager audiomanager;
    private GameObject lastTextGameObject;

    void Awake () {
        RemoveChildren();
    }

    // Creates a new Story object with the compiled story which we can then play!
    public void StartStory (int currentDay) {
        story = new Story (inkJSONAsset.text);
        story.variablesState["currentDay"] = currentDay; // Set the currentDay variable in the Ink story
        if(OnCreateStory != null) OnCreateStory(story);
        Debug.Log("Starting story with currentDay: " + currentDay);
        story.BindExternalFunction("EndTutorial", () => {
            GameManager.Instance.EndTutorial();
        });
    }

    // This is the main function called every time the story changes. It does a few things:
    // Destroys all the old content and choices.
    // Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
    void RefreshView () {
        // Remove all the UI on screen
        RemoveChildren ();
        
        // Start coroutine to handle text display
        StartCoroutine(DisplayTextAndChoices());
    }

// Coroutine to handle text display and choices
IEnumerator DisplayTextAndChoices() {
    // Read all the content until we can't continue any more
    Debug.Log("DisplayTextAndChoices started.");
    while (story.canContinue) {
        // Continue gets the next line of the story
        string text = story.Continue();
        Debug.Log("Story text: " + text);

        // This removes any white space from the text.
        text = text.Trim();
        // Check for special tag
        bool isLoadKnot = false;
        bool isUpdateKnot = false;
        foreach (string tag in story.currentTags) {
            if (tag == "LOAD_KNOT") {
                isLoadKnot = true;
                break;
            } else if (tag == "UPDATE_KNOT") {
                isUpdateKnot = true;
                break;
            }
        }

        if (isLoadKnot) {
            // Display the text on screen with slow animation
            yield return StartCoroutine(AnimateTextSlowly(CreateTextView(text), text));
        } else if (isUpdateKnot) {
            // Display the text on screen with slow animation
            yield return StartCoroutine(DisplayRefreshingText(CreateTextView(text), text));
        } else {
            // Display the text on screen with normal animation
            yield return StartCoroutine(AnimateText(CreateTextView(text), text));
        }
    }

    // Check for end of knot tag
    foreach (string tag in story.currentTags) {
        if (tag == "END_KNOT") {
            CameraManager.Instance.SwitchToRepairCamera();
            GameManager.Instance.StartTimer();
        }else if (tag == "START_TUTO") {
            Debug.Log("Tuto lancé pour l'écran");
            GameManager.Instance.HandleTutorial();
            GameManager.Instance.SpawnTutoObject();
        }else if (tag == "ENDKNOT_TUTO") {
            CameraManager.Instance.SwitchToRepairCamera();
        }
    }

    // Display all the choices, if there are any!
    if (story.currentChoices.Count > 0) {
        for (int i = 0; i < story.currentChoices.Count; i++) {
            Choice choice = story.currentChoices[i];
            Button button = CreateChoiceView(choice.text.Trim());
            Debug.Log("Affiche un BOUTOOON");
            // Tell the button what to do when we press it
            button.onClick.AddListener(delegate {
                OnClickChoiceButton(choice);
            });
        }
    }
    // If we've read all the content and there's no choices, the story is finished!
    else {
        Debug.Log("No more choices, end of story.");
        Button choice = CreateChoiceView("End of story.\nRestart?");
        choice.onClick.AddListener(delegate {
            StartStory(GameManager.Instance.currentDay); // Restart story with currentDay
        });
    }

    yield return null;
    Debug.Log("DisplayTextAndChoices finished.");
}
    

    // When we click the choice button, tell the story to choose that choice!
    void OnClickChoiceButton (Choice choice) {
        audiomanager.Play("keytap");
        story.ChooseChoiceIndex (choice.index);
        RefreshView();
    }

    TextMeshProUGUI CreateTextView(string text) {
    if (textPrefab == null) {
        Debug.LogError("textPrefab is not assigned in the inspector");
        return null;
    }

    TextMeshProUGUI storyText = Instantiate(textPrefab) as TextMeshProUGUI;
    storyText.text = "";
    storyText.transform.SetParent(canvas.transform, false);
    lastTextGameObject = storyText.gameObject; // Keep track of the last text GameObject
    return storyText;
}

    // Coroutine for animating the text
    IEnumerator AnimateText(TextMeshProUGUI textComponent, string text) {
        textComponent.text = "";
        audiomanager.Play("datatext");

        int length = text.Length;
        System.Text.StringBuilder displayedText = new System.Text.StringBuilder();

        for (int i = 0; i < length; i++) {
            displayedText.Append(text[i]);
            textComponent.text = displayedText.ToString();
            if (i % 3 == 0) { // Ajoute un petit délai toutes les trois lettres
                yield return new WaitForSeconds(textSpeed);
            }
        }

        audiomanager.Stop("datatext");
    }

    IEnumerator AnimateTextSlowly(TextMeshProUGUI textComponent, string text) {
        textComponent.text = "";
        foreach (char c in text) {
            textComponent.text += c;
            yield return new WaitForSeconds(0.8f); // Affiche chaque caractère avec une pause de 1 seconde
        }
    }

    IEnumerator DisplayRefreshingText(TextMeshProUGUI textComponent, string text) {
        textComponent.text = "";
        System.Text.StringBuilder displayedText = new System.Text.StringBuilder();

        int length = text.Length;
        for (int i = 0; i < length; i++) {
            displayedText.Append(text[i]);
            textComponent.text = displayedText.ToString();
            if (i % 3 == 0) { // Ajoute un petit délai toutes les trois lettres
                yield return new WaitForSeconds(textSpeed);
            }

            // Si c'est la dernière itération, ne pas supprimer le texte immédiatement
            if (i == length - 1) {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1f));  // Attendre un peu pour montrer le texte final
            }
        }

        // Ensuite, supprimer le texte actuel pour le remplacer par le suivant
        if (lastTextGameObject != null) {
            Destroy(lastTextGameObject);
        }

        // Mettre à jour la référence au dernier GameObject de texte
        lastTextGameObject = textComponent.gameObject;
    }


    // Creates a button showing the choice text
    Button CreateChoiceView (string text) {
        if (buttonPrefab == null) {
            Debug.LogError("buttonPrefab is not assigned in the inspector");
            return null;
        }

        // Creates the button from a prefab
        Button choice = Instantiate (buttonPrefab) as Button;
        choice.transform.SetParent (canvas.transform, false);
        
        // Gets the text from the button prefab
        TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
        if (choiceText == null) {
            Debug.LogError("No TextMeshProUGUI component found in button prefab");
            return null;
        }
        choiceText.text = text;

        // Make the button expand to fit the text
        HorizontalLayoutGroup layoutGroup = choice.GetComponent <HorizontalLayoutGroup> ();
        layoutGroup.childForceExpandHeight = false;

        return choice;
    }

    // Destroys all the children of this gameobject (all the UI)
    void RemoveChildren () {
        int childCount = canvas.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i) {
            Destroy (canvas.transform.GetChild (i).gameObject);
        }
    }

    public void StartStoryFromKnot(string knotName)
    {
        Debug.Log("Ink commence l'histoire au noeud"+knotName);
        story.ChoosePathString(knotName);
        RefreshView();
    }

    public void SetInkVariable(string variableName, object value)
    {
        Debug.Log("Ink définit la variable état comme"+value);
        if (story != null && story.variablesState != null)
        {
            story.variablesState[variableName] = value;
        }
    }

    public void RestartStory()
    {
        Debug.Log("Restarting Ink story.");
        story = new Story(inkJSONAsset.text);
        story.onError += (message, type) => {
            Debug.LogError($"Ink Error: {message} (Type: {type})");
        };

        if (OnCreateStory != null) OnCreateStory(story);
        RefreshView();
    }
}
