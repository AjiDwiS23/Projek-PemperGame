using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DraggableAnswer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string answerText;
    public TMP_Text answerTextUI;
    public Transform originalParent;
    public Transform currentParent;
    public AnswerSlot originalSlot;
    private CanvasGroup canvasGroup;
    public Vector3 originalLocalPosition;

    void Awake()
    {
        if (originalParent == null)
            originalParent = transform.parent; // Menyimpan parent awal (Answer Panel)
        currentParent = originalParent;
        originalLocalPosition = transform.localPosition; // Simpan posisi awal
        canvasGroup = GetComponent<CanvasGroup>();
        if (answerTextUI == null)
            answerTextUI = GetComponentInChildren<TMP_Text>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalSlot = originalParent.GetComponent<AnswerSlot>();
        canvasGroup.blocksRaycasts = false;

        // Mainkan SFX "Click" saat mulai drag
        if (AudioManager.instance != null)
            AudioManager.instance.Play("Click");
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Jika parent-nya bukan AnswerSlot, kembalikan ke originalParent dan posisi awal
        if (transform.parent.GetComponent<AnswerSlot>() == null)
        {
            transform.SetParent(originalParent, false);
            currentParent = originalParent;
            transform.localPosition = originalLocalPosition; // Kembali ke posisi awal
        }
        else
        {
            // Jika di slot, biarkan di tengah slot
            transform.localPosition = Vector3.zero;
        }
    }

    public void SetAnswerText(string text)
    {
        answerText = text;
        if (answerTextUI != null)
            answerTextUI.text = text;
    }
}
