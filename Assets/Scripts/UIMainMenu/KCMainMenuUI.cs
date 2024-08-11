using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KCMainMenuUI : MonoBehaviour
{
    [Header("Audio")]
   public GameObject AudioPanel;
   public Slider MusicSlider,SFXSlider;
   
   #region  Audio
   public void ShowAudioPanel(bool active){
    AudioPanel.SetActive(active);
    if(active){
        MusicSlider.value = KCAudioManager.instance.musicVolume;
        SFXSlider.value = KCAudioManager.instance.sfxVolume;
    }
   }

   public void AdjustMusicVolume(float val){
    KCAudioManager.instance.AdjustMusicVolume(val);
   }

   public void AdjustSFXVolume(float val){
    KCAudioManager.instance.AdjustSFXVolume(val);
   }
   #endregion

   public void LoadLobby(){
    KCGameManager.instance.LoadSceneName( KCStaticEnums.SceneNames.lobby);
   }
}
