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

    void Awake()
    {
        if (originalParent == null)
            originalParent = transform.parent; // Menyimpan parent awal (Answer Panel)
        currentParent = originalParent;
        canvasGroup = GetComponent<CanvasGroup>();
        if (answerTextUI == null)
            answerTextUI = GetComponentInChildren<TMP_Text>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalSlot = originalParent.GetComponent<AnswerSlot>();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        if (transform.parent == originalParent)
        {
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
