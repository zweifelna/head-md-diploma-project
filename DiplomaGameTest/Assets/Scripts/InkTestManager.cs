using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class InkTestManager : MonoBehaviour
{
    public TextAsset inkJSONAsset;
    private Story story;
    public Button restartButton;
    public TextMeshProUGUI storyText;

    private int currentDay = 1;
    private int currentScore = 1;

    void Start()
    {
        InitializeStory("start");
        restartButton.onClick.AddListener(() => StartStoryFromKnot("quota_achieved"));
    }

    void InitializeStory(string knotName)
    {
        story = new Story(inkJSONAsset.text);
        story.onError += (message, type) => {
            Debug.LogError($"Ink Error: {message} (Type: {type})");
        };
        SetInkVariable("currentDay", currentDay);
        SetInkVariable("current_score", currentScore);
        StartStoryFromKnot(knotName);
    }

    void RestartStory()
    {
        currentDay++;
        Debug.Log("Restarting story for day: " + currentDay);
        InitializeStory("quota_achieved");
    }

    void SetInkVariable(string variableName, object value)
    {
        if (story != null && story.variablesState != null)
        {
            Debug.Log($"Setting Ink variable {variableName} to {value}");
            story.variablesState[variableName] = value;
        }
        else
        {
            Debug.LogError($"Unable to set Ink variable {variableName}. Story or variablesState is null.");
        }
    }

    void StartStoryFromKnot(string knotName)
    {
        Debug.Log("Starting story from knot: " + knotName);
        story.ChoosePathString(knotName);
        RefreshView();
    }

    void RefreshView()
    {
        storyText.text = "";
        while (story.canContinue)
        {
            string text = story.Continue();
            text = text.Trim();
            Debug.Log("Text: " + text);
            storyText.text += text + "\n";
        }

        if (story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceButton(choice.text.Trim());
                button.onClick.AddListener(() => OnClickChoiceButton(choice));
            }
        }
        else
        {
            Button button = CreateChoiceButton("Next");
            button.onClick.AddListener(() => StartStoryFromKnot("quota_achieved"));
        }
    }

    Button CreateChoiceButton(string text)
    {
        Button button = Instantiate(restartButton, restartButton.transform.parent);
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = text;
        button.gameObject.SetActive(true);
        return button;
    }

    void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshView();
    }
}
