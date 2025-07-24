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
    public AudioClip voiceOverClip; // Isi di Inspector sesuai kebutuhan

    [Header("Dialog Settings")]
    public float typeSpeed = 0.05f;
    public float hideDelay = 1.5f;
    public float idleDelay = 0.5f; // Tambahan: delay sebelum kembali ke Idle

    [Header("Dialog Content")]
    [TextArea]
    public string inspectorDialogText = "Teks dialog default yang bisa diubah di Inspector.";

    public System.Action onDialogComplete;

    private Coroutine dialogCoroutine;

    public void ShowDialog(string text = null)
    {
        if (dialogCoroutine != null)
            StopCoroutine(dialogCoroutine);

        // Reset semua trigger sebelum mengatur ulang animasi
        dialogBoxAnimator.ResetTrigger("Hide");
        dialogBoxAnimator.ResetTrigger("Idle");
        dialogBoxAnimator.SetTrigger("Show");

        dialogBoxText.text = "";
        string dialogText = string.IsNullOrEmpty(text) ? inspectorDialogText : text;
        dialogCoroutine = StartCoroutine(TypeAndHideDialog(dialogText));

        // Mainkan audio voice-over jika ada
        if (audioSource != null && voiceOverClip != null)
        {
            audioSource.Stop();
            audioSource.clip = voiceOverClip;
            audioSource.Play();
        }
    }


    private IEnumerator TypeAndHideDialog(string text)
    {
        // Typewriter effect
        for (int i = 0; i <= text.Length; i++)
        {
            if (dialogBoxText != null)
                dialogBoxText.text = text.Substring(0, i);
            yield return new WaitForSeconds(typeSpeed);
        }

        // Wait before hiding
        yield return new WaitForSeconds(hideDelay);

        if (dialogBoxAnimator != null)
            dialogBoxAnimator.SetTrigger("Hide");

        // Callback ke MateriUI
        if (onDialogComplete != null)
            onDialogComplete.Invoke();

        // Tunggu sebelum kembali ke Idle
        yield return new WaitForSeconds(idleDelay);
        SetIdle();

        // (Opsional) Hentikan audio setelah dialog selesai
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    // Method untuk trigger Idle
    public void SetIdle()
    {
        if (dialogBoxAnimator != null)
            dialogBoxAnimator.SetTrigger("Idle");
    }
}
