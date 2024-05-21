using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrollingText : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField][TextArea] private string[] itemInfo;
    [SerializeField] private float textSpeed = 0.01f;
    [Header("Text Settings")]
    [SerializeField] private TextMeshProUGUI itemInfoText;
    private int currentDisplayingText = 0;
    private Coroutine currentCoroutine;
    public CameraManager cameraManager;

    public void ActivateText()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText()
    {
        itemInfoText.text = "";
        for (int i = 0; i < itemInfo[currentDisplayingText].Length + 1; i++)
        {
            itemInfoText.text = itemInfo[currentDisplayingText].Substring(0, i);
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void NextText() {
        if (currentDisplayingText < itemInfo.Length - 1) {
            currentDisplayingText++;
            ActivateText();
        } else {
            cameraManager.SwitchToRepairCamera(); // Retourne à la caméra de réparation à la fin des messages
        }
    }

    public void PreviousText()
    {
        if (currentDisplayingText > 0)
        {
            currentDisplayingText--;
            ActivateText();
        }
    }

    public void ResetTextIndex() {
        currentDisplayingText = 0;
    }
}
