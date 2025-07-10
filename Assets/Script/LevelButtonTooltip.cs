using UnityEngine;
using UnityEngine.EventSystems;

public class LevelButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string levelName;
    [TextArea] public string levelDescription;

    // Tambahkan field untuk key PlayerPrefs
    public string starsKey = "Quiz_Stars";
    public string scoreKey = "Quiz_FinalScore";
    public string keysKey = "Quiz_PlayerKeys";
    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.Instance.Show(levelName, levelDescription, starsKey, scoreKey, keysKey);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.Hide();
    }
}