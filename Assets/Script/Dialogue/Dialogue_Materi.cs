using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue_Materi : MonoBehaviour
{
    [Header("Dialog Box References")]
    public TMP_Text dialogBoxText;
    public Animator dialogBoxAnimator;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Dialog Settings")]
    public float typeSpeed = 0.05f;
    public float idleDelay = 0.5f;

    [Header("Dialog Content")]
    public DialogueData[] dialogueDataArray;
    public DialogueData dialogueData;

    public System.Action onDialogComplete;

    private Coroutine dialogCoroutine;

    // Fungsi trigger dialog berdasarkan index
    public void ShowDialogByIndex(int index)
    {
        DialogueData dataToShow = null;
        if (dialogueDataArray != null && index >= 0 && index < dialogueDataArray.Length)
            dataToShow = dialogueDataArray[index];
        else
            dataToShow = dialogueData;

        ShowDialog(dataToShow);
    }

    // Method agar bisa dipanggil dari Inspector (UI Button)
    public void TriggerDialogByIndex(int index)
    {
        ShowDialogByIndex(index);
    }

    public void ShowDialog(DialogueData data = null)
    {
        DialogueData usedData = data != null ? data : dialogueData;
        if (usedData == null) return;

        // Cek apakah dialog sudah pernah muncul
        if (!string.IsNullOrEmpty(usedData.dialogId) && PlayerPrefs.GetInt(GetDialogKey(usedData.dialogId), 0) == 1)
            return;

        if (dialogCoroutine != null)
            StopCoroutine(dialogCoroutine);

        dialogBoxAnimator.ResetTrigger("Hide");
        dialogBoxAnimator.ResetTrigger("Idle");
        dialogBoxAnimator.SetTrigger("Show");

        dialogBoxText.text = "";

        string dialogText = usedData.dialogText;
        AudioClip voiceClip = usedData.voiceOverClip;
        float hideDelayValue = usedData.hideDelay;

        dialogCoroutine = StartCoroutine(TypeAndHideDialog(dialogText, voiceClip, hideDelayValue));

        // Simpan status dialog sudah tampil
        if (!string.IsNullOrEmpty(usedData.dialogId))
            PlayerPrefs.SetInt(GetDialogKey(usedData.dialogId), 1);
    }

    private IEnumerator TypeAndHideDialog(string text, AudioClip voiceOverClip, float hideDelay)
    {
        if (audioSource != null && voiceOverClip != null)
        {
            audioSource.Stop();
            audioSource.clip = voiceOverClip;
            audioSource.Play();
        }

        for (int i = 0; i <= text.Length; i++)
        {
            if (dialogBoxText != null)
                dialogBoxText.text = text.Substring(0, i);
            yield return new WaitForSeconds(typeSpeed);
        }

        yield return new WaitForSeconds(hideDelay);

        if (dialogBoxAnimator != null)
            dialogBoxAnimator.SetTrigger("Hide");

        if (onDialogComplete != null)
            onDialogComplete.Invoke();

        yield return new WaitForSeconds(idleDelay);
        SetIdle();

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    public void SetIdle()
    {
        if (dialogBoxAnimator != null)
            dialogBoxAnimator.SetTrigger("Idle");
    }

    // Helper untuk key PlayerPrefs
    private string GetDialogKey(string dialogId)
    {
        return "Dialogue_Materi_Shown_" + dialogId;
    }
}
