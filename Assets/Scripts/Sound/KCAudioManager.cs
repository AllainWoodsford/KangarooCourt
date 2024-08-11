using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KCAudioManager : MonoBehaviour
{
    public static KCAudioManager instance = null;
    public float musicVolume,sfxVolume = 1;
    public AudioSource musicSource = null;
    public List<AudioSource> sfxSource = new List<AudioSource>();
    private List<AudioClip> LoadedClips = new List<AudioClip>();
    // Start is called before the first frame update
    public void Awake()
    {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    public void AdjustMusicVolume(float vol){
        musicVolume = vol;
        if(vol <= 0){
            musicSource.Pause();
        }else{
            musicSource.volume = musicVolume;
            if(!musicSource.isPlaying){
                musicSource.Play();
            }
        }
    }

    public void AdjustSFXVolume(float vol){
        sfxVolume = vol;
        if(sfxVolume > 0){
            for(int i  =0; i < sfxSource.Count ; i++){
                try{
                    sfxSource[i].volume = sfxVolume;
                }catch{
                    continue;
                }
            }
        }
    }

    
}
