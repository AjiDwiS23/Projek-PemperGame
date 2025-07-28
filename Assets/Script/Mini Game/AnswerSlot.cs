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
                // Jika slot ini sudah ada jawaban, swap ke slot asal jawaban yang baru masuk
                if (currentAnswer != null && currentAnswer != answer)
                {
                    // Ambil slot asal dari jawaban yang baru masuk
                    AnswerSlot originSlot = answer.currentParent != null
                        ? answer.currentParent.GetComponent<AnswerSlot>()
                        : null;

                    if (originSlot != null)
                    {
                        // Pindahkan jawaban lama ke slot asal jawaban baru
                        currentAnswer.transform.SetParent(originSlot.transform, false);
                        currentAnswer.currentParent = originSlot.transform;
                        currentAnswer.transform.localPosition = Vector3.zero;
                        originSlot.currentAnswer = currentAnswer;
                    }
                    else
                    {
                        // Jika slot asal tidak ada, kembalikan ke Answer Panel (originalParent)
                        currentAnswer.transform.SetParent(currentAnswer.originalParent, false);
                        currentAnswer.currentParent = currentAnswer.originalParent;
                        currentAnswer.transform.localPosition = Vector3.zero;
                    }
                }

                // Pindahkan jawaban baru ke slot ini
                answer.transform.SetParent(transform, false);
                answer.currentParent = transform;
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
            currentAnswer.currentParent = currentAnswer.originalParent;
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
                answer.currentParent = answer.originalParent;
                answer.transform.localPosition = Vector3.zero;
                answer.gameObject.SetActive(true);
            }
        }
        currentAnswer = null;
        HideResult();
    }
}
