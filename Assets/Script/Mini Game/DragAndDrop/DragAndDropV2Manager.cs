using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DragAndDropV2Manager : MonoBehaviour
{
    [Header("ScriptableObject Question Data")]
    [SerializeField] private DragDropQuestionDataV2 questionData;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI questionTextUI;
    [SerializeField] private TextMeshProUGUI questionNumberTextUI; // Tampilkan nomor soal
    [SerializeField] private Image questionImageUI;      // Untuk menampilkan gambar soal
    [SerializeField] private AudioSource questionAudio;  // Untuk memutar voice over
    [SerializeField] private List<Button> answerButtons = new List<Button>(); // 3 buttons, each has DraggableAnswerV2

    [SerializeField] private GameObject submitButton;
    [SerializeField] private DropSlotV2 dropSlot;

    [SerializeField] private GameObject answerButtonPrefab; // Prefab button yang ada DraggableAnswerV2
    [SerializeField] private Transform answerPanel;         // Panel tempat spawn button

    [Header("Dialogue References")]
    [SerializeField] private Dialogue_Materi dialogueMateri;
    [SerializeField] private int dialogIndexCorrect = 2;
    [SerializeField] private int dialogIndexWrong = 3;

    private void Start()
    {
        submitButton.SetActive(false);
        dropSlot.OnAnswerDropped += ShowSubmitButton;
        dropSlot.OnAnswerRemoved += HideSubmitButton;

        SetupQuestionUI();

        // Tambahkan event pada tombol submit
        var btn = submitButton.GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(CheckAnswer);
    }

    private void SetupQuestionUI()
    {
        if (questionData == null) return;
        if (questionTextUI != null)
            questionTextUI.text = questionData.questionText;

        // Tampilkan nomor soal
        if (questionNumberTextUI != null)
            questionNumberTextUI.text = $"Soal ke-{questionData.questionNumber}";

        // Set gambar soal
        if (questionImageUI != null)
            questionImageUI.sprite = questionData.questionImage;

        // Set dan mainkan audio voice over
        if (questionAudio != null)
        {
            questionAudio.clip = questionData.voiceOverClip;
            if (questionAudio.clip != null)
                questionAudio.Play();
        }

        // Hapus button lama jika ada
        foreach (Transform child in answerPanel)
            Destroy(child.gameObject);
        answerButtons.Clear();

        // Spawn button sesuai jumlah jawaban
        for (int i = 0; i < questionData.answerChoices.Length; i++)
        {
            var btnObj = Instantiate(answerButtonPrefab, answerPanel);
            var btn = btnObj.GetComponent<Button>();
            var draggable = btnObj.GetComponent<DraggableAnswerV2>();
            if (draggable != null)
            {
                draggable.answerIndex = i;
                draggable.SetAnswerText(questionData.answerChoices[i]);
                draggable.answerPanel = answerPanel; // setelah instantiate
                draggable.dropSlot = dropSlot;
            }
            answerButtons.Add(btn);
        }
    }

    private void ShowSubmitButton()
    {
        submitButton.SetActive(true);
    }

    private void HideSubmitButton()
    {
        submitButton.SetActive(false);
    }

    private void CheckAnswer()
    {
        int droppedIndex = dropSlot.GetDroppedAnswerIndex();
        if (droppedIndex == -1)
        {
            Debug.Log("Tidak ada jawaban yang dijatuhkan.");
            return;
        }

        if (droppedIndex == questionData.correctAnswerIndex)
        {
            Debug.Log("Jawaban benar!");
            ScoreManager.Instance.AddScore(500);
            if (dialogueMateri != null)
                dialogueMateri.ShowDialogByIndex(dialogIndexCorrect);
        }
        else
        {
            Debug.Log("Jawaban salah!");
            if (dialogueMateri != null)
                dialogueMateri.ShowDialogByIndex(dialogIndexWrong);
        }
    }
}