using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
    public bool isLeftAvatar = true;
    public Sprite extraImage;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public GameObject interactIcon; // Drag icon interaksi di atas NPC ke sini

    private bool playerInRange = false;
    private bool dialogueStarted = false;

    private void Start()
    {
        if (interactIcon != null)
            interactIcon.SetActive(false); // Pastikan icon hilang di awal
    }

    private void Update()
    {
        if (playerInRange && !dialogueStarted && Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
        dialogueStarted = true;
        if (interactIcon != null)
            interactIcon.SetActive(false); // Sembunyikan icon saat dialog mulai
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            dialogueStarted = false;
            if (interactIcon != null)
                interactIcon.SetActive(true); // Tampilkan icon saat player masuk trigger
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactIcon != null)
                interactIcon.SetActive(false); // Sembunyikan icon saat player keluar trigger
        }
    }
}