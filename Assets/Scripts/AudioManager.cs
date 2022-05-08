using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioSource _source;
    [SerializeField] AudioSource _musicSource;
    [SerializeField] List<AudioClip> _clips;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void Play(string clip)
    {
        //mozna predelat na enum jmen soundu kvuli vykonu
        AudioClip sound = _clips.Find(m => m.name == clip);
        if (sound == null)
            return;

        _source.PlayOneShot(sound);
    }

    public void ChangeMusic(string songName)
    {
        AudioClip sound = _clips.Find(m => m.name == songName);
        if (sound == null)
            return;
        _musicSource.clip = sound;
        _musicSource.Play();
    }
}
