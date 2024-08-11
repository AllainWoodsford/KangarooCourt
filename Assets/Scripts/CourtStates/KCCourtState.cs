using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CourtState", menuName = "CourtState/NewCourtState")]
public class KCCourtState : ScriptableObject
{
   public KCStaticEnums.CourtState MyState = KCStaticEnums.CourtState.none;
   public bool StateCompleted = false;
   public KCCourtState NextState = null;
    public int positioning = 0; //tracking progress through state
    public List<string> JudgeRooInputs = new List<string>();

    public virtual void InitialiseCourtState(){
        
    }
   public virtual void EvaluateCourtState(){
     if(KCCourtManager.instance.busy || KCCourtManager.instance.waiting){
        return;
     }

     try{
        KCCourtManager.instance.PassStatementToJudgeRoo(JudgeRooInputs[positioning]);
        positioning ++;
        if(positioning >= JudgeRooInputs.Count){
            StateCompleted = true;
        }
     }catch{
        StateCompleted = true;
     }
   }
}
