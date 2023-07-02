using UnityEngine;
using UnityEngine.Audio;

public class SettingsAudio : MonoBehaviour
{
    [SerializeField] private string groupName = "Master";
    public AudioMixer audioMixer;

    private void Start()
    {
        if (PlayerPrefs.HasKey(groupName))
        {
            SetVolume(PlayerPrefs.GetFloat(groupName));
        }
    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat(groupName, volume);
        audioMixer.SetFloat(groupName, volume);
    }
}