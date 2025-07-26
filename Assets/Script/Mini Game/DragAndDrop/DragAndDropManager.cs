using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class DragAndDropManager : MonoBehaviour
{
    public DropSlot[] dropSlots;
    public DragDropQuestionData[] questionDataArray;
    public DragDropQuestionData questionData;
    public Transform sentencePanel;
    public GameObject textPartPrefab;
    public GameObject dropSlotPrefab;
    public Dialogue_Materi dialogueMateri;
    public int dialogIndexCorrect = 2;
    public int dialogIndexWrong = 3;
    public Transform answerPanel;
    public GameObject dragItemPrefab;

    [Header("Panel Settings")]
    public GameObject panelObject; // Assign panel GameObject di Inspector

    [Header("Finish UI Settings")]
    public GameObject finishUIPanel; // Assign FinishUI GameObject di Inspector
    public TextMeshProUGUI finishUIScoreText; // Assign TMP_Text di FinishUI untuk skor

    private Action onMiniGameComplete;
    [Header("Score Settings")]
    public int scoreOnCorrect = 10;
    public float closePanelDelay = 2f;

    [Header("Audio Settings")]
    public AudioSource audioSource; // Assign di Inspector atau lewat script


    public void SetQuestionByIndex(int index)
    {
        if (questionDataArray != null && index >= 0 && index < questionDataArray.Length)
        {
            questionData = questionDataArray[index];
        }
        else
        {
            Debug.LogWarning("Index soal DragAndDrop tidak valid!");
        }
    }

    void SetupQuestion()
    {
        SetupSentence();
        SetupAnswerChoices();
        PlayVoiceOver(); // Putar audio setelah soal ditampilkan
    }

    void SetupSentence()
    {
        foreach (Transform child in sentencePanel) Destroy(child.gameObject);

        string template = questionData.sentenceTemplate;
        int slotCount = questionData.correctAnswers.Length;
        int lastIndex = 0;

        dropSlots = new DropSlot[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            int idx = template.IndexOf("{" + i + "}", lastIndex);
            if (idx == -1)
            {
                Debug.LogWarning($"Placeholder {{{i}}} tidak ditemukan di sentenceTemplate: \"{template}\"");
                continue; // Skip jika placeholder tidak ditemukan
            }

            int length = idx - lastIndex;
            if (length < 0)
            {
                Debug.LogWarning($"Substring length negatif untuk slot {i}. lastIndex={lastIndex}, idx={idx}");
                length = 0;
            }

            string textPart = template.Substring(lastIndex, length);

            var textObj = Instantiate(textPartPrefab, sentencePanel);
            textObj.GetComponent<TMPro.TMP_Text>().text = textPart;

            var slotObj = Instantiate(dropSlotPrefab, sentencePanel);
            var dropSlot = slotObj.GetComponent<DropSlot>();
            dropSlots[i] = dropSlot;

            lastIndex = idx + 3;
        }
        if (lastIndex < template.Length)
        {
            var textObj = Instantiate(textPartPrefab, sentencePanel);
            textObj.GetComponent<TMPro.TMP_Text>().text = template.Substring(lastIndex);
        }
    }

    void SetupAnswerChoices()
    {
        foreach (Transform child in answerPanel) Destroy(child.gameObject);

        foreach (var answer in questionData.answerChoices)
        {
            var dragItemObj = Instantiate(dragItemPrefab, answerPanel);
            var dragItem = dragItemObj.GetComponent<DragItem>();
            dragItem.answerText.text = answer;
            dragItem.answerPanel = answerPanel;
        }
    }

    public void CheckAnswers()
    {
        bool allCorrect = true;
        for (int i = 0; i < dropSlots.Length; i++)
        {
            var slot = dropSlots[i];
            if (slot.transform.childCount == 0)
            {
                allCorrect = false;
                break;
            }
            var dragItem = slot.transform.GetChild(0).GetComponent<DragItem>();
            if (dragItem == null || dragItem.answerText.text != questionData.correctAnswers[i])
            {
                allCorrect = false;
                break;
            }
        }

        if (dialogueMateri != null)
        {
            if (allCorrect)
                dialogueMateri.ShowDialogByIndex(dialogIndexCorrect);
            else
                dialogueMateri.ShowDialogByIndex(dialogIndexWrong);
        }

        // Jika benar, tambah score, mainkan SFX, dan tampilkan FinishUI
        if (allCorrect)
        {
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(scoreOnCorrect);

            // Play SFX Quiz_Finish
            if (AudioManager.instance != null)
                AudioManager.instance.Play("Quiz_Finish");

            ShowFinishUI();
        }

        // Invoke callback if correct
        if (allCorrect && onMiniGameComplete != null)
        {
            onMiniGameComplete.Invoke();
            onMiniGameComplete = null;
        }
    }

    private void ShowFinishUI()
    {
        // Tutup panel drag and drop
        if (panelObject != null)
            panelObject.SetActive(false);
        else
            gameObject.SetActive(false);

        // Tampilkan FinishUI dan set skor
        if (finishUIPanel != null)
        {
            finishUIPanel.SetActive(true);
            if (finishUIScoreText != null && ScoreManager.Instance != null)
                finishUIScoreText.text = "Score: " + ScoreManager.Instance.CurrentScore.ToString();
        }
    }

    // Fungsi untuk tombol selesai di FinishUI
    public void CloseFinishUI()
    {
        if (finishUIPanel != null)
            finishUIPanel.SetActive(false);
    }

    public void StartMiniGame(Action onComplete)
    {
        if (panelObject != null)
            panelObject.SetActive(true);
        else
            gameObject.SetActive(true);

        if (finishUIPanel != null)
            finishUIPanel.SetActive(false);

        SetupQuestion();
        onMiniGameComplete = onComplete;
    }

    private void PlayVoiceOver()
    {
        if (audioSource != null && questionData != null && questionData.voiceOverClip != null)
        {
            audioSource.Stop();
            audioSource.clip = questionData.voiceOverClip;
            audioSource.Play();
        }
    }
}