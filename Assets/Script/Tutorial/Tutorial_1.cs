using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_1 : MonoBehaviour
{
    [Header("Tutorial Data List")]
    public TutorialData[] tutorialDataList; // Drag multiple ScriptableObjects here

    [Header("UI References")]
    public TMP_Text tutorialText;
    public Button nextButton;
    public Animator panelAnimator;

    [Header("Voice Over")]
    public AudioSource audioSource;

    private int currentSentence = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private int currentAnimationIndex = 0;
    private int currentTutorialIndex = 0;

    void Start()
    {
        if (tutorialDataList == null || tutorialDataList.Length == 0)
            return;

        if (PlayerPrefs.GetInt(GetPlayerPrefKey(), 0) == 1)
        {
            if (panelAnimator != null)
                panelAnimator.SetTrigger("Hide");
            if (nextButton != null)
                nextButton.interactable = false;
            // Hapus atau komentari baris di bawah ini agar GameObject tidak dinonaktifkan:
            // gameObject.SetActive(false);
            return;
        }

        nextButton.onClick.AddListener(OnNextClicked);
        // ShowSentence(currentSentence); // Tetap dikomen jika ingin tutorial hanya muncul lewat trigger
    }

    void ShowSentence(int index)
    {
        var tutorialData = tutorialDataList[currentTutorialIndex];

        if (panelAnimator != null && tutorialData.animationClipNames.Length > 0 && tutorialData.animationStartSentenceIndexes.Length > 0)
        {
            if (currentAnimationIndex < tutorialData.animationStartSentenceIndexes.Length - 1 &&
                index >= tutorialData.animationStartSentenceIndexes[currentAnimationIndex + 1])
            {
                currentAnimationIndex++;
            }

            if (currentAnimationIndex < tutorialData.animationClipNames.Length)
                panelAnimator.Play(tutorialData.animationClipNames[currentAnimationIndex]);
        }

        if (audioSource != null && tutorialData.voiceOvers != null && index < tutorialData.voiceOvers.Length && tutorialData.voiceOvers[index] != null)
        {
            audioSource.Stop();
            audioSource.clip = tutorialData.voiceOvers[index];
            audioSource.Play();
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(tutorialData.tutorialSentences[index], tutorialData.typeSpeed));
    }

    IEnumerator TypeSentence(string sentence, float typeSpeed)
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
        var tutorialData = tutorialDataList[currentTutorialIndex];

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            tutorialText.text = tutorialData.tutorialSentences[currentSentence];
            isTyping = false;
            return;
        }

        currentSentence++;
        if (currentSentence < tutorialData.tutorialSentences.Length)
        {
            ShowSentence(currentSentence);
        }
        else
        {
            if (panelAnimator != null)
                panelAnimator.SetTrigger("Hide");
            nextButton.interactable = false;
            if (audioSource != null)
                audioSource.Stop();

            PlayerPrefs.SetInt(GetPlayerPrefKey(), 1);
            PlayerPrefs.Save();

            // Hapus atau komentari baris di bawah ini:
            // gameObject.SetActive(false);
            return;
        }
    }

    private string GetPlayerPrefKey()
    {
        return $"Tutorial_{tutorialDataList[currentTutorialIndex].tutorialID}_Completed";
    }

    [ContextMenu("Start Animation From Index 0")]
    public void TriggerStartAnimation()
    {
        currentAnimationIndex = 0;
        var tutorialData = tutorialDataList[currentTutorialIndex];
        if (panelAnimator != null && tutorialData.animationClipNames.Length > 0)
        {
            panelAnimator.Play(tutorialData.animationClipNames[0]);
        }
    }

    // Trigger tutorial by index (which tutorial SO, and which sentence)
    public void TriggerTutorialAtIndex(int tutorialIndex, int sentenceIndex)
    {
        if (tutorialIndex < 0 || tutorialIndex >= tutorialDataList.Length)
            return;

        var tutorialData = tutorialDataList[tutorialIndex];
        if (sentenceIndex < 0 || sentenceIndex >= tutorialData.tutorialSentences.Length)
            return;

        // Reset PlayerPrefs agar tutorial bisa diulang
        PlayerPrefs.SetInt($"Tutorial_{tutorialData.tutorialID}_Completed", 0);
        PlayerPrefs.Save();

        // Aktifkan panel dan tombol next jika perlu
        if (nextButton != null)
            nextButton.interactable = true;

        // Reset typing state
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        isTyping = false;

        currentTutorialIndex = tutorialIndex;
        currentSentence = sentenceIndex;
        currentAnimationIndex = 0;
        for (int i = 0; i < tutorialData.animationStartSentenceIndexes.Length; i++)
        {
            if (sentenceIndex >= tutorialData.animationStartSentenceIndexes[i])
                currentAnimationIndex = i;
            else
                break;
        }

        ShowSentence(currentSentence);
    }
}
