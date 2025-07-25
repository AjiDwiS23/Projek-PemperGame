using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TMP_Text answerText;
    [HideInInspector] public Transform answerPanel; // Set dari DragAndDropManager

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;

    // Simpan ukuran default untuk AnswerPanel
    private float defaultWidth = 180f;
    private float defaultHeight = 50f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalParent = transform.parent;

        // Ambil ukuran default dari LayoutElement jika ada
        var le = GetComponent<LayoutElement>();
        if (le != null)
        {
            defaultWidth = le.preferredWidth;
            defaultHeight = le.preferredHeight;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        originalParent = transform.parent;
        transform.SetParent(transform.root); // Agar di atas UI lain
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        LayoutElement le = GetComponent<LayoutElement>();

        // Jika tidak di-drop ke slot, kembalikan ke AnswerPanel dan reset ukuran
        if (transform.parent == transform.root)
        {
            transform.SetParent(answerPanel != null ? answerPanel : originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
            if (le != null)
            {
                le.preferredWidth = defaultWidth;
                le.preferredHeight = defaultHeight;
            }
        }
        else
        {
            // Jika di-drop ke DropSlot, samakan ukuran dengan slot
            LayoutElement slotLe = transform.parent.GetComponent<LayoutElement>();
            if (le != null && slotLe != null)
            {
                le.preferredWidth = slotLe.preferredWidth;
                le.preferredHeight = slotLe.preferredHeight;
            }
        }
    }
}