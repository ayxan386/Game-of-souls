using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class SettingsAudio : MonoBehaviour
{

    public AudioMixer audioMixer;
    
    public void SetVolume (float volume)
    {
        audioMixer.SetFloat("Master", volume);
    }
    



}
