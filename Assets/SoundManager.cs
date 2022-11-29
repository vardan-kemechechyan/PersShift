using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioSource musicSource;

    [SerializeField] AudioClip buttonPress;
    [SerializeField] AudioClip itemTap;
    [SerializeField] AudioClip itemAquired;
    [SerializeField] AudioClip simpleGetButtonSound;
    [SerializeField] AudioClip[] updateModelInShop;

    [SerializeField] AudioClip SuccessfullDressChange;
    [SerializeField] AudioClip FailedDressChange;
    [SerializeField] AudioClip PlayerWon;
    [SerializeField] AudioClip PlayerLost;
    [SerializeField] AudioClip CrowedCheerOne;
    [SerializeField] AudioClip CrowedCheerTwo;

    [SerializeField] AudioClip[] musicTracks;

    public static bool music;
    public static bool sound;

    [SerializeField] float musicVolume = 1f;
    [SerializeField] float musicFadeSpeed = 1.0f;

    bool isMusicFading;
    float targetFadeValue;

    [SerializeField] AudioClip currentTrack;
    [SerializeField] List<AudioClip> playlist;

    bool isInitialized = false;

    public bool IsMusicPlaying()
    {
        if(musicSource.isPlaying)
            return true;

        return false;
    }

    public bool IsSoundPlaying(AudioClip sound)
    {
        if(soundSource.clip.Equals(sound) && soundSource.isPlaying)
            return true;

        return false;
    }

    public bool isSoundOn() { return sound; }

    public void Initialize()
    {
        if(!PlayerPrefs.HasKey("sound"))
            PlayerPrefs.SetInt("sound", 1);

        if(!PlayerPrefs.HasKey("music"))
            PlayerPrefs.SetInt("music", 1);

        PlayerPrefs.Save();

        LoadSettings();

        MuteSources();

        currentTrack = musicTracks[UnityEngine.Random.Range(0, musicTracks.Length)];

        PlayMusic( currentTrack );

        GeneratePlaylist();

        isInitialized = true;
    }

    private void Update()
    {
        if(!isInitialized) return;

        soundSource.pitch = Time.timeScale;

        if(!musicSource.isPlaying)
        {
            PlayMusic( GetRandomTrackFromPlaylist() );
        }

        if(isMusicFading)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, targetFadeValue, musicFadeSpeed * Time.unscaledDeltaTime);

            if(targetFadeValue <= 0)
            {
                if(musicSource.volume <= 0)
                {
                    if(musicSource.isPlaying)
                    {
                        musicSource.Stop();
                        isMusicFading = false;
                    }
                }
            }
            else
            {
                if(musicSource.volume >= targetFadeValue)
                {
                    isMusicFading = false;
                }
            }
        }
    }

    void GeneratePlaylist()
    {
        playlist = new List<AudioClip>();

        foreach(var ost in musicTracks)
		{
            if(!ost.Equals(currentTrack))
            {
                playlist.Add(ost);
            }
        }
	}

    AudioClip GetRandomTrackFromPlaylist()
    {
        if(playlist.Count == 0 || playlist == null)
            GeneratePlaylist();

        AudioClip randomClip = playlist[UnityEngine.Random.Range(0, playlist.Count)];

        playlist.Remove(randomClip);

        return randomClip;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("sound", Convert.ToInt32(sound));
        PlayerPrefs.SetInt("music", Convert.ToInt32(music));
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        sound = Convert.ToBoolean(PlayerPrefs.GetInt("sound"));
        music = Convert.ToBoolean(PlayerPrefs.GetInt("music"));

        soundSource.mute = !sound;

        foreach(var m in musicTracks)
        {
            musicSource.mute = !music;
        }
    }

    public void ToggleSound()
    {
        sound = !sound;
        music = !music;

        if(sound)
        {
            AnalyticEvents.ReportEvent("sounds_on");
        }
        else
        {
            AnalyticEvents.ReportEvent("sounds_off");
        }

        SaveSettings();
        LoadSettings();

        MuteSources();
    }

    public void PlayMusic(AudioClip track)
    {
        currentTrack = track;

        musicSource.clip = track;
        musicSource.Play();

        musicSource.volume = 0;
        targetFadeValue = musicVolume;
        isMusicFading = true;
    }

    #region Different Sound Plays

    public void PressUIButton() { PlaySound(buttonPress); }

    public void ItemAquiredInShop() { PlaySound(itemAquired); }

    public void SelectUnlockedItem( bool isBought )
    {
        PlaySound(itemTap);

        if( isBought )
        {
            float delayedSound = itemTap.length;

            PlaySound( updateModelInShop[UnityEngine.Random.Range(0, updateModelInShop.Length)], delayedSound );
		}
    }

    public void SimpleGetButtonSound() { PlaySound(simpleGetButtonSound); }

    public void ChangeDressSound(bool successfull)
    {
        if(successfull)
            PlaySound(SuccessfullDressChange);
        else
            PlaySound(FailedDressChange);
    }

    public void PlayerCrossedTheFinishLine( bool IsPlayerWinning )
    {
        PlaySound(IsPlayerWinning ? PlayerWon : PlayerLost);

        if( IsPlayerWinning )
        {
            float delayedSound = PlayerWon.length;

            PlaySound(CrowedCheerOne, delayedSound);
        }
	}

    public void PlayerGotToNextRoom()
    {
        PlaySound( CrowedCheerTwo );
	}

	#endregion

	void PlaySound(AudioClip sound)
    {
        soundSource.clip = sound;
        soundSource.Play();
    }
    public async void PlaySound( AudioClip sound, float delayTime)
    {
        await Task.Delay(Mathf.CeilToInt(delayTime * 1000));

        PlaySound(sound);
    }

    private void MuteSources()
    {
        soundSource.mute = !sound;
        musicSource.mute = !music;
    }
    public void StopMusic()
    {
        musicSource.clip = null;
        targetFadeValue = 0;
        isMusicFading = true;
    }

    public void StopSound()
    {
        soundSource.Stop();
    }

    public void StopSound(int sound)
    {
        StopSound();
    }
}
