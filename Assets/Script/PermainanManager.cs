using UnityEngine;
using UnityEngine.SceneManagement;

public class PermainanManager : MonoBehaviour
{
    public static PermainanManager Instance { get; private set; }

    private Vector3 checkpointPosition;
    private int lastCheckpointID = 0;

    // Helper untuk key PlayerPrefs sesuai scene
    private string CheckpointXKey => SceneManager.GetActiveScene().name + "_CheckpointX";
    private string CheckpointYKey => SceneManager.GetActiveScene().name + "_CheckpointY";
    private string CheckpointZKey => SceneManager.GetActiveScene().name + "_CheckpointZ";
    private string LastCheckpointIDKey => SceneManager.GetActiveScene().name + "_LastCheckpointID";

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
        PlayerPrefs.DeleteKey(LastCheckpointIDKey);
    }

    public void OnFinishButtonClicked()
    {
        if (ScoreManager.Instance != null)
        {
            string stageKey = SceneManager.GetActiveScene().name + "_FinalScore";
            PlayerPrefs.SetInt(stageKey, ScoreManager.Instance.CurrentScore);
            PlayerPrefs.Save();
        }
        Debug.Log("Score saved and finish button pressed!");
    }
}
