using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image leftCharacterIcon;
    public Image rightCharacterIcon;

    public GameObject extraImageBody; // <-- Tambahkan ini, drag GameObject "Body" di Inspector
    public Image extraImageUI;        // <-- Drag child Image ("Gambar_Source") di Inspector

    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.2f;

    public Animator animator;

    private bool isTyping = false;
    private string currentFullLine = "";
    private Coroutine typingCoroutine;

    // Tambahkan AudioSource untuk voice over
    public AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;

        animator.Play("show");

        lines.Clear();

        // Nonaktifkan body dan gambar di awal dialog
        if (extraImageBody != null)
            extraImageBody.SetActive(false);
        if (extraImageUI != null)
            extraImageUI.gameObject.SetActive(false);

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        // Avatar kiri/kanan
        if (currentLine.isLeftAvatar)
        {
            leftCharacterIcon.gameObject.SetActive(true);
            rightCharacterIcon.gameObject.SetActive(false);
            leftCharacterIcon.sprite = currentLine.character.icon;
        }
        else
        {
            leftCharacterIcon.gameObject.SetActive(false);
            rightCharacterIcon.gameObject.SetActive(true);
            rightCharacterIcon.sprite = currentLine.character.icon;
        }

        characterName.text = currentLine.character.name;

        // Body dan gambar: aktif hanya jika ada gambar
        if (extraImageBody != null)
            extraImageBody.SetActive(currentLine.extraImage != null);

        if (extraImageUI != null)
        {
            if (currentLine.extraImage != null)
            {
                extraImageUI.gameObject.SetActive(true);
                extraImageUI.sprite = currentLine.extraImage;
            }
            else
            {
                extraImageUI.gameObject.SetActive(false);
            }
        }

        // === Tambahkan pemutaran voice over di sini ===
        if (audioSource != null)
        {
            audioSource.Stop();
            if (currentLine.voiceOver != null)
            {
                audioSource.clip = currentLine.voiceOver;
                audioSource.Play();
            }
        }

        StopAllCoroutines();
        currentFullLine = currentLine.line;
        typingCoroutine = StartCoroutine(TypeSentence(currentLine.line));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueArea.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    public void OnNextButtonPressed()
    {
        if (!isDialogueActive)
            return;

        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogueArea.text = currentFullLine;
            isTyping = false;
        }
        else
        {
            DisplayNextDialogueLine();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        animator.Play("hide");
        leftCharacterIcon.gameObject.SetActive(false);
        rightCharacterIcon.gameObject.SetActive(false);
        if (extraImageUI != null)
            extraImageUI.gameObject.SetActive(false);
        if (extraImageBody != null)
            extraImageBody.SetActive(false);

        // Stop voice over saat dialog selesai
        if (audioSource != null)
            audioSource.Stop();
    }
}