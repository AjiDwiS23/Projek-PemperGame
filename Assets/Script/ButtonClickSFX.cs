using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSFX : MonoBehaviour
{
    [SerializeField] private string sfxName = "Click";

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlaySFX);
    }

    private void PlaySFX()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.Play(sfxName);
    }
}