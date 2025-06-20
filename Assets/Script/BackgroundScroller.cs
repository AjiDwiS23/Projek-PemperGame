using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public Transform[] backgrounds;  // Array untuk menyimpan background
    public float backgroundWidth;    // Lebar background
    public float triggerOffset = 5f; // Jarak dari tepi background untuk memicu perpindahan

    public Transform playerTransform;
    private int currentBackgroundIndex = 0;

    void Start()
    {
        // Jika backgroundWidth belum diset, hitung otomatis
        if (backgroundWidth == 0)
        {
            backgroundWidth = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;
        }

        // Atur posisi awal background
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].position = new Vector3(i * backgroundWidth, backgrounds[i].position.y, backgrounds[i].position.z);
        }
    }

    void Update()
    {
        // Titik pemicu untuk tepi kanan
        float rightTriggerPoint = backgrounds[currentBackgroundIndex].position.x + backgroundWidth - triggerOffset;

        // Titik pemicu untuk tepi kiri
        float leftTriggerPoint = backgrounds[currentBackgroundIndex].position.x + triggerOffset;

        if (playerTransform.position.x >= rightTriggerPoint)
        {
            MoveBackgroundToFront();
        }
        else if (playerTransform.position.x <= leftTriggerPoint)
        {
            MoveBackgroundToBack();
        }
    }

    void MoveBackgroundToFront()
    {
        // Pindahkan background saat ini ke depan
        Vector3 newPosition = backgrounds[currentBackgroundIndex].position;
        newPosition.x += backgroundWidth * backgrounds.Length;
        backgrounds[currentBackgroundIndex].position = newPosition;

        // Update index background saat ini
        currentBackgroundIndex = (currentBackgroundIndex + 1) % backgrounds.Length;
    }

    void MoveBackgroundToBack()
    {
        // Pindahkan background saat ini ke belakang
        Vector3 newPosition = backgrounds[currentBackgroundIndex].position;
        newPosition.x -= backgroundWidth * backgrounds.Length;
        backgrounds[currentBackgroundIndex].position = newPosition;

        // Update index background saat ini
        currentBackgroundIndex = (currentBackgroundIndex - 1 + backgrounds.Length) % backgrounds.Length;
    }
}
