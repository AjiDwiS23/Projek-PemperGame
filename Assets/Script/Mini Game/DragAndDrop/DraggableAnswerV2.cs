using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableAnswerV2 : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Reference")]
    [SerializeField] public TextMeshProUGUI answerTextUI;

    public Transform answerPanel; // Tambahkan ini
    public DropSlotV2 dropSlot; // Tambahkan ini

    private Transform originalParent;
    public int answerIndex;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Auto-assign if not set in Inspector
        if (answerTextUI == null)
            answerTextUI = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetAnswerText(string text)
    {
        if (answerTextUI != null)
            answerTextUI.text = text;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        // Jika mulai drag dari DropSlotV2, panggil RemoveAnswer
        if (originalParent != null && originalParent.GetComponent<DropSlotV2>() != null)
        {
            var slot = originalParent.GetComponent<DropSlotV2>();
            slot.RemoveAnswer();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        // Jika parent masih root (belum di-drop ke slot), kembalikan ke panel jawaban
        if (transform.parent == transform.root)
        {
            if (answerPanel != null)
            {
                transform.SetParent(answerPanel, false);
                gameObject.SetActive(true); // pastikan aktif di panel
            }
            else
            {
                transform.SetParent(originalParent, false);
                gameObject.SetActive(true);
            }
        }
        else
        {
            // Jika di-drop ke slot, sembunyikan dari panel (opsional, jika instance ganda)
            gameObject.SetActive(true); // atau false jika ingin hilang dari panel
        }
    }

    public void ResetToOriginalParent()
    {
        if (answerPanel != null)
        {
            transform.SetParent(answerPanel, false);
            gameObject.SetActive(true);
        }
        else
        {
            transform.SetParent(originalParent, false);
            gameObject.SetActive(true);
        }
    }
}