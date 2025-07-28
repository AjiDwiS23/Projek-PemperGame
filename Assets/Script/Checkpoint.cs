using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointID = 0; // Set di Inspector
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite activatedSprite;

    private SpriteRenderer spriteRenderer;
    private static List<Checkpoint> allCheckpoints = new List<Checkpoint>();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        allCheckpoints.Add(this);
    }

    private void OnDestroy()
    {
        allCheckpoints.Remove(this);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateAllCheckpointSprites();
    }

    private static string GetCheckpointKey()
    {
        return SceneManager.GetActiveScene().name + "_LastCheckpointID";
    }

    public static void UpdateAllCheckpointSprites()
    {
        int lastCheckpointID = 0;
        string checkpointKey = GetCheckpointKey();
        if (PermainanManager.Instance != null)
            lastCheckpointID = PermainanManager.Instance.GetLastCheckpointID();
        else if (PlayerPrefs.HasKey(checkpointKey))
            lastCheckpointID = PlayerPrefs.GetInt(checkpointKey);

        foreach (var cp in allCheckpoints)
        {
            if (cp.checkpointID == lastCheckpointID)
            {
                if (cp.spriteRenderer != null && cp.activatedSprite != null)
                    cp.spriteRenderer.sprite = cp.activatedSprite;
            }
            else
            {
                if (cp.spriteRenderer != null && cp.defaultSprite != null)
                    cp.spriteRenderer.sprite = cp.defaultSprite;
            }
        }
    }

    private void Start()
    {
        UpdateAllCheckpointSprites();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.Play("Checkpoint");
            int lastCheckpointID = 0;
            string checkpointKey = GetCheckpointKey();
            if (PermainanManager.Instance != null)
                lastCheckpointID = PermainanManager.Instance.GetLastCheckpointID();
            else if (PlayerPrefs.HasKey(checkpointKey))
                lastCheckpointID = PlayerPrefs.GetInt(checkpointKey);

            if (PermainanManager.Instance != null && checkpointID > lastCheckpointID)
            {
                PermainanManager.Instance.SetCheckpoint(transform.position, checkpointID);

                // Simpan checkpoint ke PlayerPrefs dengan key sesuai scene
                PlayerPrefs.SetInt(checkpointKey, checkpointID);
                PlayerPrefs.Save();

                UpdateAllCheckpointSprites();

                var notif = Object.FindFirstObjectByType<CheckpointNotificationUI>();
                if (notif != null)
                    notif.ShowNotification("Checkpoint tersimpan!");
            }
            else if (checkpointID < lastCheckpointID)
            {
                var notif = Object.FindFirstObjectByType<CheckpointNotificationUI>();
                if (notif != null)
                    notif.ShowNotification("Ini bukan checkpoint terakhirmu!");
            }
        }
    }
}
