using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    void Start()
    {
        // Assign listener ke setiap tombol
        for (int i = 0; i < materiButtons.Length; i++)
        {
            int idx = i; // Capture index
            materiButtons[i].onClick.RemoveAllListeners();
            materiButtons[i].onClick.AddListener(() => ShowMateri(idx));
        }

        HideMateri();
    }

    public void ShowMateri(int index)
    {
        if (index < 0 || index >= materiList.Length) return;
        MateriData data = materiList[index];

        if (materiPanel != null) materiPanel.SetActive(true);
        if (materiImage != null) materiImage.sprite = data.materiImage;
        if (materiText != null) materiText.text = data.materiText;
    }

    public void HideMateri()
    {
        if (materiPanel != null) materiPanel.SetActive(false);
    }
}