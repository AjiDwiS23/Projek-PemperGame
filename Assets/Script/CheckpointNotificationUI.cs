using UnityEngine;
using TMPro;

public class CheckpointNotificationUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float showDuration = 1.5f;

    private void Awake()
    {
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }

    public void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideNotification));
            Invoke(nameof(HideNotification), showDuration);
        }
    }

    private void HideNotification()
    {
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }
}
