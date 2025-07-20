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
                // Jika slot sudah terisi dan bukan jawaban yang sama, lakukan swap
                if (currentAnswer != null && currentAnswer != answer)
                {
                    // Ambil slot asal jawaban baru
                    AnswerSlot originSlot = answer.originalSlot;

                    // Pindahkan currentAnswer ke slot asal jawaban baru
                    if (originSlot != null)
                    {
                        currentAnswer.transform.SetParent(originSlot.transform);
                        currentAnswer.transform.localPosition = Vector3.zero;
                        originSlot.currentAnswer = currentAnswer;
                    }
                    else
                    {
                        // Jika asal bukan slot (misal panel jawaban), kembalikan ke parent asal
                        currentAnswer.transform.SetParent(currentAnswer.originalParent);
                        currentAnswer.transform.localPosition = Vector3.zero;
                    }
                }

                // Set jawaban baru ke slot ini
                answer.transform.SetParent(transform);
                answer.transform.localPosition = Vector3.zero;
                currentAnswer = answer;
                Mini_Game_1.Instance.CheckAllSlotsFilled();
            }
        }
    }

    public void RemoveAnswer()
    {
        currentAnswer = null;
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
}
