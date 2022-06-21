using RamailoGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SoundType
{
    none,
    mainMenuSound,
    backgroundSound,
    slashSound,
    uiSound,
    pauseSound,
    explosion,
}


public class soundManager : MonoBehaviour
{
    public static soundManager instance;
    public List<AudioClip> mainMenuSound;
    public List<AudioClip> backGroundSound;

    public List<AudioClip> slashEffectSounds;
    public List<AudioClip> uiSounds;
    public AudioClip pauseResumeSound;
    public AudioClip explosionSound;
    public float backGroundAudioVolume;
    public float soundeffectVolume;

    private AudioSource UISoundSource;
    private AudioSource backGroundAudioSource;
    private AudioSource slashAudioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        MusicVolumeChanged(backGroundAudioVolume);
        SoundVolumeChanged(soundeffectVolume);
        SceneManager.LoadScene(1);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (backGroundAudioSource != null)
        {
            if(backGroundAudioSource.isPlaying == false)
            {
                switch (SceneManager.GetActiveScene().buildIndex)
                {
                    
                    case 1:
                        PlaySound(SoundType.mainMenuSound);
                        break;
                    case 2:
                        PlaySound(SoundType.backgroundSound);
                        break;
                    
                }
                MusicVolumeChanged(backGroundAudioVolume);
                SoundVolumeChanged(soundeffectVolume);
            }
        }
    }

    public void PlaySound(SoundType soundType)
    {
        AudioClip clip;
        int soundIndex;
        switch (soundType)
        {
            case SoundType.mainMenuSound:
                soundIndex = Random.Range(0, mainMenuSound.Count);
                clip = mainMenuSound[soundIndex];
                if (backGroundAudioSource == null)
                {
                    backGroundAudioSource = gameObject.AddComponent<AudioSource>();
                }
                backGroundAudioSource.clip = clip;
                backGroundAudioSource.loop = false;
                backGroundAudioSource.Play();
                break;
            case SoundType.backgroundSound:
                soundIndex = Random.Range(0, backGroundSound.Count);
                clip = backGroundSound[soundIndex];
                if (backGroundAudioSource == null)
                {
                    backGroundAudioSource = gameObject.AddComponent<AudioSource>();
                }
                backGroundAudioSource.clip = clip;
                backGroundAudioSource.loop = false;
                backGroundAudioSource.Play();
                break;
            case SoundType.slashSound:
                soundIndex = Random.Range(0, slashEffectSounds.Count);
                clip = slashEffectSounds[soundIndex];
                if (slashAudioSource == null)
                {
                    slashAudioSource = gameObject.AddComponent<AudioSource>();
                }
                slashAudioSource.clip = clip;
                slashAudioSource.loop = false;
                slashAudioSource.Play();
                break;

            case SoundType.uiSound:
                soundIndex = Random.Range(0, uiSounds.Count);
                clip = uiSounds[soundIndex];
                if (UISoundSource == null)
                {
                    UISoundSource = gameObject.AddComponent<AudioSource>();
                }
                UISoundSource.clip = clip;
                UISoundSource.loop = false;
                UISoundSource.Play();
                break;
            case SoundType.pauseSound:
                clip = pauseResumeSound;
                if (UISoundSource == null)
                {
                    UISoundSource = gameObject.AddComponent<AudioSource>();
                }
                UISoundSource.clip = clip;
                UISoundSource.loop = false;
                UISoundSource.Play();
                break;

            case SoundType.explosion:
                clip = explosionSound;
                if (backGroundAudioSource == null)
                {
                    backGroundAudioSource = gameObject.AddComponent<AudioSource>();
                }
                backGroundAudioSource.clip = clip;
                backGroundAudioSource.loop = false;
                MusicVolumeChanged(1);
                backGroundAudioSource.Play();
                break;
            default:
                break;
        }
    }

    public void MusicVolumeChanged(float volume)
    {
        if(backGroundAudioSource != null)
        {
            backGroundAudioSource.volume = volume;
        }
    }
    public void SoundVolumeChanged(float volume)
    {
        if (slashAudioSource != null)
        {
            slashAudioSource.volume = volume;
        }
    }

    public void SaveMusicVoulme(float volume)
    {
        backGroundAudioVolume = volume;
    }
    public void SaveSoundVoulme(float volume)
    {
        soundeffectVolume = volume;
    }
}
