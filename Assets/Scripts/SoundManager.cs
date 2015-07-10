using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SoundManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float SFXVolume = 1;
    [Range(0f, 1f)]
    public float MusicVolume = 1;

    public AudioListener Listener;

    public AudioSource Source1, Source2;

    public AudioSource MusicSource;

    public AudioClip[] MusicClips;

    public AudioClip[] Clips;

    void Start()
    {
        ApplyVolume();
    }

    public void ApplyVolume()
    {
        Source1.volume = SFXVolume;
        Source2.volume = SFXVolume;
        MusicSource.volume = MusicVolume;
    }

    public void Play(string ID)
    {
        if (GameManager.Instance.MuteSFX)
            return;

        AudioClip _clip = Clips.FirstOrDefault(x => x.name == ID);
        if (_clip != null)
        {
            AudioSource _source = GetFreeSource();
            _source.clip = _clip;
            _source.volume = SFXVolume;
            _source.Play();
        }
        else
        {
            Debug.LogError("Tried to play unknown sound: " + ID);
        }
    }

    AudioSource GetFreeSource()
    {
        if (Source1.isPlaying)
            return Source2;

        return Source1;
    }

    public void PlayMusic(string ID, float Speed = 1)
    {
        if (GameManager.Instance.MuteMusic)
            return;

        AudioClip _clip = MusicClips.FirstOrDefault(x => x.name == ID);
        MusicSource.clip = _clip;
        MusicSource.pitch = Speed;
        MusicSource.volume = MusicVolume;
        MusicSource.Play();
    }

    public void StopMusic()
    {
        MusicSource.Stop();
    }

    public string GetCurrentMusicClip()
    {
        return MusicSource.clip.name;
    }

    public void MusicFadeOff()
    {
        StartCoroutine(FadeOff());
    }

    IEnumerator FadeOff()
    {
        while (MusicSource.volume > 0.1f)
        {
            MusicSource.volume -= 0.025f;
            yield return new WaitForEndOfFrame();
        }

        MusicSource.Stop();
        MusicSource.volume = MusicVolume;
    }

}
