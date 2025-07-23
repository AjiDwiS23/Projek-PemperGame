using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_1 : MonoBehaviour
{
    [Header("Tutorial ID")]
    public string tutorialID = "Tutorial_1"; // Set unique ID for this tutorial

    [Header("UI References")]
    public TMP_Text tutorialText;         // Drag Text UI here
    public Button nextButton;         // Drag Next Button UI here
    public Animator panelAnimator;    // (Optional) Drag Animator for panel animation

    [Header("Tutorial Content")]
    [TextArea]
    public string[] tutorialSentences;

    [Header("Animation Clips")]
    public string[] animationClipNames = { "Tutorial_1", "Tutorial_1_2", "Tutorial_1_3", "Tutorial_1_4" };
    public int[] animationStartSentenceIndexes = { 0, 2, 3, 5 }; // Contoh: animasi ke-1 mulai di sentence 0, ke-2 di 2, dst

    [Header("Typewriter Settings")]
    public float typeSpeed = 0.05f;

    [Header("Voice Over")]
    public AudioClip[] voiceOvers; // Drag audio clips here, urut sesuai sentences
    public AudioSource audioSource; // Drag AudioSource component here

    private int currentSentence = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private int currentAnimationIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Cek apakah tutorial sudah pernah selesai
        if (PlayerPrefs.GetInt(GetPlayerPrefKey(), 0) == 1)
        {
            // Jika sudah, nonaktifkan panel/tutorial
            if (panelAnimator != null)
                panelAnimator.SetTrigger("Hide");
            if (nextButton != null)
                nextButton.interactable = false;
            gameObject.SetActive(false);
            return;
        }

        nextButton.onClick.AddListener(OnNextClicked);
        ShowSentence(currentSentence);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowSentence(int index)
    {
        // Cek apakah perlu ganti animasi berdasarkan index sentence
        if (panelAnimator != null && animationClipNames.Length > 0 && animationStartSentenceIndexes.Length > 0)
        {
            // Jika index sentence >= index start animasi berikutnya, pindah ke animasi berikutnya
            if (currentAnimationIndex < animationStartSentenceIndexes.Length - 1 &&
                index >= animationStartSentenceIndexes[currentAnimationIndex + 1])
            {
                currentAnimationIndex++;
            }

            if (currentAnimationIndex < animationClipNames.Length)
                panelAnimator.Play(animationClipNames[currentAnimationIndex]);
        }

        // Play voice over if available
        if (audioSource != null && voiceOvers != null && index < voiceOvers.Length && voiceOvers[index] != null)
        {
            audioSource.Stop();
            audioSource.clip = voiceOvers[index];
            audioSource.Play();
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(tutorialSentences[index]));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        tutorialText.text = "";
        foreach (char letter in sentence)
        {
            tutorialText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }

    void OnNextClicked()
    {
        if (isTyping)
        {
            // Selesaikan typing langsung jika user klik Next saat animasi
            StopCoroutine(typingCoroutine);
            tutorialText.text = tutorialSentences[currentSentence];
            isTyping = false;
            return;
        }

        currentSentence++;
        if (currentSentence < tutorialSentences.Length)
        {
            ShowSentence(currentSentence);
        }
        else
        {
            // Tutorial selesai, bisa sembunyikan panel atau lanjut ke scene berikutnya
            if (panelAnimator != null)
                panelAnimator.SetTrigger("Hide"); // Pastikan ada trigger "Hide" di Animator
            nextButton.interactable = false;
            if (audioSource != null)
                audioSource.Stop();

            // Simpan status tutorial selesai di PlayerPrefs
            PlayerPrefs.SetInt(GetPlayerPrefKey(), 1);
            PlayerPrefs.Save();

            // Optional: Nonaktifkan GameObject setelah selesai
            gameObject.SetActive(false);
        }
    }

    private string GetPlayerPrefKey()
    {
        return $"Tutorial_{tutorialID}_Completed";
    }
}
