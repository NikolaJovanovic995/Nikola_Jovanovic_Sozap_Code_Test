using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] _soundsArray;
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
            lSound.AudioSource.pitch = lSound.Pitch;
            lSound.AudioSource.loop = lSound.Loop;
        }
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
