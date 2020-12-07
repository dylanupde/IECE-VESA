using UnityEngine.Audio;
using System;
using UnityEngine;

/// <summary>
/// -UNIMPLEMENTED-
/// </summary>
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound thisSound in sounds)
        {
            thisSound.source = gameObject.AddComponent<AudioSource>();
            thisSound.source.clip = thisSound.clip;

            thisSound.source.volume = thisSound.volume;
            thisSound.source.pitch = thisSound.pitch;
        }
    }

    public void Play(string inputSoundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == inputSoundName);
        s.source.Play();
    }
}
