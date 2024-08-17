using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KCAudioManager : MonoBehaviour
{
    public static KCAudioManager instance = null;
    public float musicVolume,sfxVolume,masterVolume = 1;
    public float rooVol = .5f;
    public AudioSource musicSource = null;
    public List<AudioSource> sfxSource = new List<AudioSource>();
    public List<AudioClip> LoadedClips = new List<AudioClip>();
    public List<string> playedClips = new List<string>();
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

    public void AdjustMasterVolume(float vol){
        masterVolume = vol;
        AdjustMusicVolume(musicVolume);
        AdjustRooVolume(rooVol);
        AdjustSFXVolume(sfxVolume);
    }

    public void AdjustMusicVolume(float vol){
        musicVolume = vol;
        if(vol <= 0 || masterVolume <= 0){
            musicSource.Pause();
        }else{
            musicSource.volume = musicVolume * masterVolume;
            if(!musicSource.isPlaying){
                musicSource.Play();
            }
        }
    }

    public void AdjustRooVolume(float vol){
        rooVol = vol * masterVolume;
    }

    public void AdjustSFXVolume(float vol){
        sfxVolume = vol;
        if(sfxVolume > 0){
            for(int i  =0; i < sfxSource.Count ; i++){
                try{
                    sfxSource[i].volume = sfxVolume * masterVolume;
                }catch{
                    continue;
                }
            }
        }
    }

    public void PlaySFX( KCStaticEnums.SoundNames sound){
        if(sfxVolume <= 0 || masterVolume <= 0){
            return;
        }
        string path = string.Concat(KCStaticEnums.PATH_RESOURCES_SFX,sound);
        Debug.Log(path);
        if(playedClips.Contains(path)){
            //already played
            SearchLoadedClipsAndPlayAudio(path);
        }else{
            try{
                 AudioClip clip = Resources.Load<AudioClip>(path);
                 if(clip != null){
                    LoadedClips.Add(clip);
                    playedClips.Add(path);
                    LookForFreeAudioSourceAndPlaySound(LoadedClips.Count -1);
                 }else{
                    Debug.Log("clip is null");
                 }
            }catch{
                Debug.Log("problem loading audio clip");
                return;
            }
           
        }
    }

    private void LookForFreeAudioSourceAndPlaySound(int i ){
        for(int j = 0; j < sfxSource.Count ; j++){
            try{
                if(!sfxSource[j].isPlaying){
                    sfxSource[j].clip = LoadedClips[i];
                    if(playedClips[i] == string.Concat(KCStaticEnums.PATH_RESOURCES_SFX,KCStaticEnums.SoundNames.murmur)){
                      sfxSource[j].volume = rooVol * masterVolume;  
                    }else{
                        sfxSource[j].volume = sfxVolume;
                    }
                    float priorPitch =   sfxSource[j].pitch;
                    float pitch = Random.Range(-0.5f,0.5f);
                    sfxSource[j].pitch += pitch;
                    sfxSource[j].Play();
                    sfxSource[j].pitch = priorPitch;
                    return;
                }
            }catch{
                continue;
            }
        }
    }

    private void SearchLoadedClipsAndPlayAudio(string clip){
        for(int i = 0; i < playedClips.Count ; i++){
            try{
                if(playedClips[i] == clip){
                    LookForFreeAudioSourceAndPlaySound(i);
                    return;
                }
            }catch{
                continue;
            }
        }
    }

    
}
