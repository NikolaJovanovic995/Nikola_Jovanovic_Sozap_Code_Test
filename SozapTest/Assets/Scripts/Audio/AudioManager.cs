using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Singleton class used to control all sound effects
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] _soundsArray;
    [SerializeField] private AudioMixerGroup _audioMixerGroup;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound lSound in _soundsArray)
        {
            lSound.AudioSource = gameObject.AddComponent<AudioSource>();
            lSound.AudioSource.clip = lSound.AudioClip;
            lSound.AudioSource.volume = lSound.Volume;
            lSound.AudioSource.loop = lSound.Loop;
            lSound.AudioSource.outputAudioMixerGroup = _audioMixerGroup;
        }
    }

    public void SetVolume(float pSliderValue)
    {
        float lValue = Mathf.Clamp(pSliderValue, 0.0001f, 1f);
        _audioMixerGroup.audioMixer.SetFloat("Volume", Mathf.Log10(lValue) * 20);
    }

    public void Play(string pName)
    {
        Sound lSound = Array.Find(_soundsArray, sound => sound.Name == pName);
        if(lSound == null)
        {
            Debug.LogWarning("Sound: "+pName+" not found!");
            return;
        }
        lSound.AudioSource.Play();
    }
}
