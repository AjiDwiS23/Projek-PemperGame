using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MateriUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject materiPanel;
    public GameObject materiUI;
    public Image materiImage;
    public TMP_Text materiText;
    public Dialogue_Materi dialogueMateri;

    [Header("Materi Data")]
    public MateriData[] materiList;

    [Header("Tombol Materi")]
    public Button[] materiButtons;

    private bool[] unlockedMateri;
    private HashSet<string> unlockedMateriIds = new HashSet<string>();

    private bool isPanelActive = false;

    void Start()
    {
        unlockedMateri = new bool[materiButtons.Length];
        // LoadUnlockedMateri(); // Uncomment if you want to use unlock system

        for (int i = 0; i < materiButtons.Length; i++)
        {
            int idx = i;
            materiButtons[i].onClick.RemoveAllListeners();
            materiButtons[i].onClick.AddListener(() => ShowMateri(idx));
            materiButtons[i].interactable = true;
            unlockedMateri[i] = true;
        }

        HideMateri();
    }

    void Update()
    {
        // Deteksi tombol B
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!isPanelActive)
            {
                ShowMateriPanelWithDialog();
                if (dialogueMateri != null)
                {
                    dialogueMateri.ShowDialogByIndex(0);
                }
            }
        }
    }

    public void UnlockMateri(int index)
    {
        if (index < 0 || index >= materiButtons.Length) return;
        unlockedMateri[index] = true;
        materiButtons[index].interactable = true;

        // Simpan ID materi ke PlayerPrefs
        if (materiList[index] != null && !string.IsNullOrEmpty(materiList[index].materiId))
        {
            unlockedMateriIds.Add(materiList[index].materiId);
            SaveUnlockedMateri();
        }
    }

    public void ShowMateri(int index)
    {
        if (index < 0 || index >= materiList.Length) return;
        if (!unlockedMateri[index]) return;

        MateriData data = materiList[index];

        if (materiPanel != null) materiPanel.SetActive(true);
        if (materiImage != null) materiImage.sprite = data.materiImage;
        if (materiText != null) materiText.text = data.materiText;
    }

    public void HideMateri()
    {
        if (materiPanel != null) materiPanel.SetActive(false);
    }

    // Fungsi untuk menampilkan panel dan trigger animator Show serta dialog
    private void ShowMateriPanelWithDialog()
    {
        if (materiUI != null){
            materiUI.SetActive(true); // Tambahan: aktifkan container UI
        }
        // Tampilkan dialog menggunakan Dialogue_Materi
        if (dialogueMateri != null)
        {
            dialogueMateri.ShowDialog();
        }
    }

    // --- PlayerPrefs Save/Load ---
    private string GetUnlockedMateriKey() => SceneManager.GetActiveScene().name + "_UnlockedMateri";

    private void SaveUnlockedMateri()
    {
        PlayerPrefs.SetString(GetUnlockedMateriKey(), string.Join(",", unlockedMateriIds));
        PlayerPrefs.Save();
    }

    private void LoadUnlockedMateri()
    {
        unlockedMateriIds.Clear();
        string saved = PlayerPrefs.GetString(GetUnlockedMateriKey(), "");
        if (!string.IsNullOrEmpty(saved))
        {
            var ids = saved.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in ids)
                unlockedMateriIds.Add(id);
        }

        // Set unlockedMateri[] berdasarkan ID yang sudah tersimpan
        for (int i = 0; i < materiList.Length; i++)
        {
            unlockedMateri[i] = materiList[i] != null && unlockedMateriIds.Contains(materiList[i].materiId);
        }
    }
}