using UnityEngine;
using TMPro;

public class DragAndDropManager : MonoBehaviour
{
    public DropSlot[] dropSlots;
    public DragDropQuestionData questionData;
    public Transform sentencePanel; // Assign di Inspector
    public GameObject textPartPrefab;
    public GameObject dropSlotPrefab;
    public Dialogue_Materi dialogueMateri;
    public int dialogIndexCorrect = 2; // Index dialog untuk jawaban benar
    public int dialogIndexWrong = 3;   // Index dialog untuk jawaban salah
    public Transform answerPanel; // Assign di Inspector
    public GameObject dragItemPrefab; // Assign di Inspector

    private void Start()
    {
        // Setup quiz dari ScriptableObject
        SetupQuestion();
    }

    void SetupQuestion()
    {
        SetupSentence();
        SetupAnswerChoices();
    }

    void SetupSentence()
    {
        // Bersihkan anak-anak lama
        foreach (Transform child in sentencePanel) Destroy(child.gameObject);

        string template = questionData.sentenceTemplate;
        int slotCount = questionData.correctAnswers.Length;
        int lastIndex = 0;

        // Siapkan array dropSlots
        dropSlots = new DropSlot[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            int idx = template.IndexOf("{" + i + "}", lastIndex);
            string textPart = template.Substring(lastIndex, idx - lastIndex);

            // Tambah text statis
            var textObj = Instantiate(textPartPrefab, sentencePanel);
            textObj.GetComponent<TMPro.TMP_Text>().text = textPart;

            // Tambah slot dan simpan referensi ke array
            var slotObj = Instantiate(dropSlotPrefab, sentencePanel);
            var dropSlot = slotObj.GetComponent<DropSlot>();
            dropSlots[i] = dropSlot;

            lastIndex = idx + 3; // "{n}" panjangnya 3
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
            dragItem.answerPanel = answerPanel; // Set parent asal
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
    }
}