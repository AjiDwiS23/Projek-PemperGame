using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex; // Untuk identifikasi slot

    public void OnDrop(PointerEventData eventData)
    {
        var dragItem = eventData.pointerDrag.GetComponent<DragItem>();
        if (dragItem != null)
        {
            dragItem.transform.SetParent(transform);
            dragItem.transform.localPosition = Vector3.zero;
        }
    }
}