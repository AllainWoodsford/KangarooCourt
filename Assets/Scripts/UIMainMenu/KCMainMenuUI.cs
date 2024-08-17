using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KCMainMenuUI : MonoBehaviour
{
    [Header("Audio")]
   public GameObject AudioPanel;
   public Slider MusicSlider,SFXSlider,RooSlider,MasterSlider;
   
   #region  Audio
   public void ShowAudioPanel(bool active){
    AudioPanel.SetActive(active);
    KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.click);
    if(active){
        MusicSlider.value = KCAudioManager.instance.musicVolume;
        SFXSlider.value = KCAudioManager.instance.sfxVolume;
        RooSlider.value = KCAudioManager.instance.rooVol;
        MasterSlider.value = KCAudioManager.instance.masterVolume;

    }
        MusicSlider.gameObject.SetActive(active);
        SFXSlider.gameObject.SetActive(active);
        RooSlider.gameObject.SetActive(active);
   }

public void AdjustMasterVolume(){
     if(MasterSlider.isActiveAndEnabled){
          KCAudioManager.instance.AdjustMasterVolume(MasterSlider.value);
     }
}
   public void AdjustMusicVolume(){
    if(MusicSlider.isActiveAndEnabled){
         KCAudioManager.instance.AdjustMusicVolume(MusicSlider.value);
    }
   
   }

   public void AdjustSFXVolume(){
    if(SFXSlider.isActiveAndEnabled){
        // KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.attention);
    KCAudioManager.instance.AdjustSFXVolume(SFXSlider.value);
    }
    
   }

   public void AdjustRooVolume(){
    if(RooSlider.isActiveAndEnabled){
           // KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.murmur);
       KCAudioManager.instance.AdjustRooVolume(RooSlider.value);
    }

   }
   #endregion

   public void LoadLobby(){
     KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.click);
    KCGameManager.instance.LoadSceneName( KCStaticEnums.SceneNames.lobby);
   }

  
}
