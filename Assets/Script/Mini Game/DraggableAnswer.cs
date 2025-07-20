using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableAnswer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string answerText;
    public Transform originalParent;
    public AnswerSlot originalSlot; // Tambahkan ini
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalSlot = originalParent.GetComponent<AnswerSlot>(); // Simpan slot asal jika ada
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
}
