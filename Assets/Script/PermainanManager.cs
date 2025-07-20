using UnityEngine;

public class PermainanManager : MonoBehaviour
{
    public static PermainanManager Instance { get; private set; }

    private Vector3 checkpointPosition;
    private const string CheckpointXKey = "CheckpointX";
    private const string CheckpointYKey = "CheckpointY";
    private const string CheckpointZKey = "CheckpointZ";
    private int lastCheckpointID = 0;
    private const string LastCheckpointIDKey = "LastCheckpointID";

    //ui
    public GameObject materiPanel;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Load checkpoint dari PlayerPrefs jika ada
        if (PlayerPrefs.HasKey(CheckpointXKey))
        {
            float x = PlayerPrefs.GetFloat(CheckpointXKey);
            float y = PlayerPrefs.GetFloat(CheckpointYKey);
            float z = PlayerPrefs.GetFloat(CheckpointZKey);
            checkpointPosition = new Vector3(x, y, z);
        }
        else
        {
            // Default: posisi player saat start
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                checkpointPosition = player.transform.position;
        }

        // Load last checkpoint ID dari PlayerPrefs jika ada
        if (PlayerPrefs.HasKey(LastCheckpointIDKey))
            lastCheckpointID = PlayerPrefs.GetInt(LastCheckpointIDKey);
        else
            lastCheckpointID = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            materiPanel.SetActive(true);
        }
    }
    public void SetCheckpoint(Vector3 pos, int id)
    {
        if (id > lastCheckpointID)
        {
            checkpointPosition = pos;
            lastCheckpointID = id;
            PlayerPrefs.SetFloat(CheckpointXKey, pos.x);
            PlayerPrefs.SetFloat(CheckpointYKey, pos.y);
            PlayerPrefs.SetFloat(CheckpointZKey, pos.z);
            PlayerPrefs.SetInt(LastCheckpointIDKey, id);
            PlayerPrefs.Save();
        }
    }

    public Vector3 GetCheckpoint()
    {
        return checkpointPosition;
    }

    public int GetLastCheckpointID()
    {
        return lastCheckpointID;
    }

    public void ResetCheckpoint()
    {
        PlayerPrefs.DeleteKey(CheckpointXKey);
        PlayerPrefs.DeleteKey(CheckpointYKey);
        PlayerPrefs.DeleteKey(CheckpointZKey);
    }
}
