using UnityEngine;

[CreateAssetMenu(fileName = "MateriData", menuName = "Materi/Materi Data", order = 1)]
public class MateriData : ScriptableObject
{
    public string materiId; // Automatically generated ID
    public Sprite materiImage;
    [TextArea(3, 10)]
    public string materiText;

    private void OnEnable()
    {
        // Generate a unique ID if it hasn't been set
        if (string.IsNullOrEmpty(materiId))
        {
            materiId = System.Guid.NewGuid().ToString();
        }
    }
}
