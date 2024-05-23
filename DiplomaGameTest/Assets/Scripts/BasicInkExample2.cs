using System;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    void Awake () {
        RemoveChildren();
    }

    // Creates a new Story object with the compiled story which we can then play!
    public void StartStory (int currentDay) {
        story = new Story (inkJSONAsset.text);
        story.variablesState["currentDay"] = currentDay; // Set the currentDay variable in the Ink story
        if(OnCreateStory != null) OnCreateStory(story);
        Debug.Log("Starting story with currentDay: " + currentDay);
        RefreshView();
    }

    // This is the main function called every time the story changes. It does a few things:
    // Destroys all the old content and choices.
    // Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
    void RefreshView () {
        // Remove all the UI on screen
        RemoveChildren ();
        
        // Read all the content until we can't continue any more
        while (story.canContinue) {
            // Continue gets the next line of the story
            string text = story.Continue ();    
            Debug.Log("Story text: " + text);

            // Check for debug tags
            foreach (string tag in story.currentTags) {
                Debug.Log("Tag: " + tag);
            }        

            // This removes any white space from the text.
            text = text.Trim();
            // Display the text on screen!
            CreateContentView(text);
        }

        // Display all the choices, if there are any!
        if(story.currentChoices.Count > 0) {
            for (int i = 0; i < story.currentChoices.Count; i++) {
                Choice choice = story.currentChoices [i];
                Button button = CreateChoiceView (choice.text.Trim ());
                // Tell the button what to do when we press it
                button.onClick.AddListener (delegate {
                    OnClickChoiceButton (choice);
                });
            }
        }
        // If we've read all the content and there's no choices, the story is finished!
        else {
            Debug.Log("No more choices, end of story.");
            Button choice = CreateChoiceView("End of story.\nRestart?");
            choice.onClick.AddListener(delegate{
                StartStory(GameManager.Instance.currentDay); // Restart story with currentDay
            });
        }
    }

    // When we click the choice button, tell the story to choose that choice!
    void OnClickChoiceButton (Choice choice) {
        story.ChooseChoiceIndex (choice.index);
        RefreshView();
    }

    // Creates a textbox showing the the line of text
    void CreateContentView (string text) {
        if (textPrefab == null) {
            Debug.LogError("textPrefab is not assigned in the inspector");
            return;
        }

        TextMeshProUGUI storyText = Instantiate (textPrefab) as TextMeshProUGUI;
        storyText.text = text;
        storyText.transform.SetParent (canvas.transform, false);
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
}