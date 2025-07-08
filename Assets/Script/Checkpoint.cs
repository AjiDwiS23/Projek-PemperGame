using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointID = 0; // Set di Inspector
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite activatedSprite;

    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;

    // Static list untuk semua checkpoint di scene
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

    public static void UpdateAllCheckpointSprites()
    {
        int lastCheckpointID = 0;
        if (PermainanManager.Instance != null)
            lastCheckpointID = PermainanManager.Instance.GetLastCheckpointID();
        else if (PlayerPrefs.HasKey("LastCheckpointID"))
            lastCheckpointID = PlayerPrefs.GetInt("LastCheckpointID");

        foreach (var cp in allCheckpoints)
        {
            if (cp.checkpointID == lastCheckpointID)
            {
                if (cp.spriteRenderer != null && cp.activatedSprite != null)
                    cp.spriteRenderer.sprite = cp.activatedSprite;
                cp.isActivated = true;
            }
            else
            {
                if (cp.spriteRenderer != null && cp.defaultSprite != null)
                    cp.spriteRenderer.sprite = cp.defaultSprite;
                cp.isActivated = false;
            }
        }
    }

    private void Start()
    {
        // Panggil update di Start juga untuk jaga-jaga
        UpdateAllCheckpointSprites();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.Play("Checkpoint");
            int lastCheckpointID = 0;
            if (PermainanManager.Instance != null)
                lastCheckpointID = PermainanManager.Instance.GetLastCheckpointID();
            else if (PlayerPrefs.HasKey("LastCheckpointID"))
                lastCheckpointID = PlayerPrefs.GetInt("LastCheckpointID");

            if (PermainanManager.Instance != null && checkpointID > lastCheckpointID)
            {
                PermainanManager.Instance.SetCheckpoint(transform.position, checkpointID);

                // Update semua checkpoint setelah checkpoint baru diambil
                UpdateAllCheckpointSprites();

                // Tampilkan notifikasi checkpoint baru
                var notif = FindObjectOfType<CheckpointNotificationUI>();
                if (notif != null)
                    notif.ShowNotification("Checkpoint tersimpan!");
            }
            else if (checkpointID < lastCheckpointID)
            {
                // Tampilkan notifikasi checkpoint lama
                var notif = FindObjectOfType<CheckpointNotificationUI>();
                if (notif != null)
                    notif.ShowNotification("Ini bukan checkpoint terakhirmu!");
            }
        }
    }
}
