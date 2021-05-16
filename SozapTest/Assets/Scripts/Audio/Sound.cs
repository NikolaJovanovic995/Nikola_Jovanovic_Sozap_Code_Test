using UnityEngine;

/// <summary>
/// Data class for sound, usefull for easy adding of new sound effects
/// </summary>
[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip AudioClip;
    [Range(0f, 1f)]
    public float Volume;
    public bool Loop;
    [HideInInspector]
    public AudioSource AudioSource;
}
