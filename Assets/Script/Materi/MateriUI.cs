using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MateriUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject materiPanel;
    public Image materiImage;
    public TMP_Text materiText;

    [Header("Materi Data")]
    public MateriData[] materiList; // Isi di Inspector, urutkan sesuai tombol

    [Header("Tombol Materi")]
    public Button[] materiButtons; // Isi di Inspector, urutkan sesuai materiList
    public GameObject[] lockIcons; // Icon kunci, urutkan sesuai tombol

    private bool[] unlockedMateri; // Status unlock tiap materi
    private HashSet<string> unlockedMateriIds = new HashSet<string>();

    void Start()
    {
        unlockedMateri = new bool[materiButtons.Length];
        LoadUnlockedMateri();

        for (int i = 0; i < materiButtons.Length; i++)
        {
            int idx = i;
            materiButtons[i].onClick.RemoveAllListeners();
            materiButtons[i].onClick.AddListener(() => ShowMateri(idx));
            materiButtons[i].interactable = unlockedMateri[i];
            if (lockIcons != null && i < lockIcons.Length && lockIcons[i] != null)
                lockIcons[i].SetActive(!unlockedMateri[i]);
        }

        HideMateri();
    }

    public void UnlockMateri(int index)
    {
        if (index < 0 || index >= materiButtons.Length) return;
        unlockedMateri[index] = true;
        materiButtons[index].interactable = true;
        if (lockIcons != null && index < lockIcons.Length && lockIcons[index] != null)
            lockIcons[index].SetActive(false);

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