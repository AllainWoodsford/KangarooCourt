using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class KCCourtClock : MonoBehaviour
{
   public TextMeshProUGUI ClockTimerText;
  
   public int expectedMaxTime = 0;
  public bool ClockIsRunning = false;
   public void InitClock(int maxTime){
    
    KCCourtManager.instance.waiting = true;
    expectedMaxTime = maxTime;
    StartCoroutine(TickClock());
   }

   IEnumerator TickClock(){
    ClockIsRunning = true;
    yield return new WaitForSeconds(1);
    ClockIsRunning = false;
    expectedMaxTime --;
    try{
      
    if(expectedMaxTime > 0){

        ClockTimerText.text = GetClockString();
        StartCoroutine(TickClock());
    }
    else{
         ClockTimerText.text = "00:00";
        KCCourtManager.instance.waiting = false;
        if(KCCourtManager.instance.MyCourtState.MyState == KCStaticEnums.CourtState.jury && KCCourtManager.instance.awaitingJury){
          KCCourtManager.instance.PressVerdictButton( 1);
        }else if(KCCourtManager.instance.MyCourtState.MyState == KCStaticEnums.CourtState.hearing){
          KCCourtManager.instance.ProceedWithCasePressed(true);
        }
         

    }
    }catch{
      StartCoroutine(TickClock());
    }
    
   }

   public void PauseClock(){
    StopCoroutine(TickClock());
    expectedMaxTime = 0;
        ClockTimerText.color = Color.white;
    
   }

   private string GetClockString(){
     int seconds = expectedMaxTime % 60;
     int minutes = expectedMaxTime /60;
    if(expectedMaxTime <= 4){
        ClockTimerText.color = Color.red;
    }else if(expectedMaxTime <= 7)
    {
          ClockTimerText.color = Color.yellow;
    }else
    {
          ClockTimerText.color = Color.white;
    }
     string minsString =  minutes <= 9 ? ("0" + minutes.ToString()): minutes.ToString();
     string secondString = seconds <= 9 ? ("0" + seconds.ToString()) : seconds.ToString();
     return string.Concat(minsString,":", secondString );
   }

}
