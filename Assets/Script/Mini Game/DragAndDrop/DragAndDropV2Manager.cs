using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class DragAndDropV2Manager : MonoBehaviour
{
    [Header("ScriptableObject Question Data")]
    [SerializeField] public DragDropQuestionDataV2 questionData;
    [SerializeField] public List<DragDropQuestionDataV2> allQuestions;

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

    [SerializeField] private GameObject questionPanel; // Panel utama yang ingin ditutup

    [Header("Dialogue References")]
    [SerializeField] private Dialogue_Materi dialogueMateri;
    [SerializeField] private int dialogIndexCorrect = 2;
    [SerializeField] private int dialogIndexWrong = 3;

    public event Action<bool> OnMiniGameCompleted;

    private void Start()
    {
        submitButton.SetActive(false);
        dropSlot.OnAnswerDropped += ShowSubmitButton;
        dropSlot.OnAnswerRemoved += HideSubmitButton;

        // Tambahkan event pada tombol submit
        var btn = submitButton.GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(CheckAnswer);
    }

    private void SetupQuestionUI()
    {
        if (questionData == null) return;
        if (dropSlot != null)
            dropSlot.ResetSlot(); // Tambahkan ini untuk reset jawaban di slot

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
            OnMiniGameCompleted?.Invoke(true);
            AudioManager.instance.Play("Quiz_Finish");
            if (dialogueMateri != null)
                dialogueMateri.ShowDialogByIndex(dialogIndexCorrect);

            // Mulai coroutine untuk menutup panel setelah 2 detik
            StartCoroutine(HidePanelAfterDelay(2f));
        }
        else
        {
            Debug.Log("Jawaban salah!");
            OnMiniGameCompleted?.Invoke(false);
            AudioManager.instance.Play("Wrong");
            if (dialogueMateri != null)
                dialogueMateri.ShowDialogByIndex(dialogIndexWrong);
        }
    }

    private IEnumerator HidePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (questionPanel != null)
            questionPanel.SetActive(false);
    }

    public void ShowMiniGame()
    {
        if (questionPanel != null)
            questionPanel.SetActive(true); // Tampilkan panel mini game
        SetupQuestionUI();
    }
}