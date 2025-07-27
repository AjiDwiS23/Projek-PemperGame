using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerSlot : MonoBehaviour, IDropHandler
{
    public DraggableAnswer currentAnswer;

    [Header("Status Icons")]
    public GameObject iconCorrect;
    public GameObject iconWrong;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DraggableAnswer answer = eventData.pointerDrag.GetComponent<DraggableAnswer>();
            if (answer != null)
            {
                if (currentAnswer != null && currentAnswer != answer)
                {
                    AnswerSlot originSlot = answer.originalSlot;
                    if (originSlot != null)
                    {
                        currentAnswer.transform.SetParent(originSlot.transform);
                        currentAnswer.transform.localPosition = Vector3.zero;
                        originSlot.currentAnswer = currentAnswer;
                    }
                    else
                    {
                        currentAnswer.transform.SetParent(currentAnswer.originalParent);
                        currentAnswer.transform.localPosition = Vector3.zero;
                    }
                }

                answer.transform.SetParent(transform);
                answer.transform.localPosition = Vector3.zero;
                currentAnswer = answer;
                Mini_Game_1.Instance.CheckAllSlotsFilled();
            }
        }
    }

    public void RemoveAnswer()
    {
        if (currentAnswer != null)
        {
            currentAnswer.transform.SetParent(currentAnswer.originalParent, false);
            currentAnswer.transform.localPosition = Vector3.zero;
            currentAnswer.gameObject.SetActive(true);
            currentAnswer = null;
        }
    }

    public void ShowResult(bool isCorrect)
    {
        if (iconCorrect != null) iconCorrect.SetActive(isCorrect);
        if (iconWrong != null) iconWrong.SetActive(!isCorrect);
    }

    public void HideResult()
    {
        if (iconCorrect != null) iconCorrect.SetActive(false);
        if (iconWrong != null) iconWrong.SetActive(false);
    }

    public void ResetSlot()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            var answer = child.GetComponent<DraggableAnswer>();
            if (answer != null)
            {
                answer.transform.SetParent(answer.originalParent, false);
                answer.transform.localPosition = Vector3.zero;
                answer.gameObject.SetActive(true);
            }
        }
        currentAnswer = null;
        HideResult();
    }
}
