using UnityEngine;
using Ink.Runtime;
using TMPro;

public class InkStoryManager : MonoBehaviour
{
    public TextAsset inkJSONAsset;
    private Story story;
    public TextMeshProUGUI storyText;
    public GameObject[] choiceButtons;

    void Start()
    {
        StartStory();
    }

    void StartStory()
    {
        story = new Story(inkJSONAsset.text);
        DisplayNextLine();
    }

    void DisplayNextLine()
    {
        if (story.canContinue)
        {
            storyText.text = story.Continue();
            DisplayChoices();
        }
        else
        {
            HideChoices();
        }
    }

    void DisplayChoices()
    {
        HideChoices();

        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            Choice choice = story.currentChoices[i];
            choiceButtons[i].SetActive(true);
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            int choiceIndex = i;
            choiceButtons[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnClickChoiceButton(choiceIndex));
        }
    }

    void HideChoices()
    {
        foreach (var button in choiceButtons)
        {
            button.SetActive(false);
            button.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        }
    }

    void OnClickChoiceButton(int choiceIndex)
    {
        story.ChooseChoiceIndex(choiceIndex);
        DisplayNextLine();
    }
}
