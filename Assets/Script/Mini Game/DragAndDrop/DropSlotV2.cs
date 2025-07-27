using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DropSlotV2 : MonoBehaviour, IDropHandler
{
    public event Action OnAnswerDropped;
    public event Action OnAnswerRemoved;

    private GameObject currentAnswer;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            // Remove previous answer if exists
            if (currentAnswer != null && currentAnswer != eventData.pointerDrag)
            {
                var draggablePrev = currentAnswer.GetComponent<DraggableAnswerV2>();
                if (draggablePrev != null)
                    draggablePrev.ResetToOriginalParent();
            }

            currentAnswer = eventData.pointerDrag;
            currentAnswer.transform.SetParent(transform);
            currentAnswer.transform.localPosition = Vector3.zero;
            OnAnswerDropped?.Invoke();
        }
    }

    public void RemoveAnswer()
    {
        if (currentAnswer != null)
        {
            var draggable = currentAnswer.GetComponent<DraggableAnswerV2>();
            if (draggable != null)
                draggable.ResetToOriginalParent();
        }
        currentAnswer = null;
        OnAnswerRemoved?.Invoke();
    }

    public int GetDroppedAnswerIndex()
    {
        if (currentAnswer == null) return -1;
        var draggable = currentAnswer.GetComponent<DraggableAnswerV2>();
        return draggable != null ? draggable.answerIndex : -1;
    }

    public void ResetSlot()
    {
        if (currentAnswer != null)
        {
            var draggable = currentAnswer.GetComponent<DraggableAnswerV2>();
            if (draggable != null)
                draggable.ResetToOriginalParent();
            currentAnswer = null;
            OnAnswerRemoved?.Invoke();
        }
    }
}