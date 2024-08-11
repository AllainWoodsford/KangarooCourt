using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class KCCourtClock : MonoBehaviour
{
   public TextMeshProUGUI ClockTimerText;
  
   public int expectedMaxTime = 0;

   public void InitClock(int maxTime){
    KCCourtManager.instance.waiting = true;
    expectedMaxTime = maxTime;
    StartCoroutine(TickClock());
   }

   IEnumerator TickClock(){
    yield return new WaitForSeconds(1);
    expectedMaxTime --;
    if(expectedMaxTime > 0){
        ClockTimerText.text = GetClockString();
        StartCoroutine(TickClock());
    }
    else{
         ClockTimerText.text = "00:00";
         KCCourtManager.instance.waiting = false;
    }
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
     return string.Concat(minutes,":",seconds);
   }

}
