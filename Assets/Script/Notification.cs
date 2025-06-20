using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notification : MonoBehaviour
{
    public int requiredScore = 12;
    [SerializeField] private TextMeshProUGUI notificationText;
    private float notificationDuration = 2f;
    private bool notificationShown = false; // Flag to ensure notification shows only once

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !notificationShown)
        {
            ShowNotification("Kumpulkan diamond minimal " + requiredScore + " untuk menyelesaikan level!");
            notificationShown = true; // Set flag to true after showing notification
        }
    }

    private void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.gameObject.SetActive(true);
            Invoke("HideNotification", notificationDuration);
        }
    }

    private void HideNotification()
    {
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }
}
