using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Assurez-vous d'inclure ceci si vous utilisez l'UI de Unity
using TMPro; // Assurez-vous d'inclure cette directive pour utiliser TextMeshPro

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 60;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText; // Référence à un composant TextMeshProUGUI pour afficher le temps

    private void Start()
    {
        // Commencez le timer automatiquement
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Temps écoulé!");
                timeRemaining = 0;
                timerIsRunning = false;

                // Actions à effectuer lorsque le temps est écoulé
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
