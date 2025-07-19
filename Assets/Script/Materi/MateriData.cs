using UnityEngine;

[CreateAssetMenu(fileName = "MateriData", menuName = "Materi/Materi Data", order = 1)]
public class MateriData : ScriptableObject
{
    public Sprite materiImage;
    [TextArea(3, 10)]
    public string materiText;
}