using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class KCCourtManager : MonoBehaviour
{
    public static KCCourtManager instance = null;
    public bool busy,waiting = false; //waiting is for the timed events, busy is for Roo's statmetns
    private int judgeRooStringPositioning = 0;
    private float timeSliceJudgeRoo = 0;
    private string judgeRooStatement = string.Empty;
    public KCStaticEnums.CourtState MyState = KCStaticEnums.CourtState.init;
    public KCCourtClock MyClock = null;
    public KCCourtState MyCourtState = null;
    public KCCaseSummary CaseSummaryScript = null;
    public bool RunningCheckState = false;
    public GameObject JudgeRooPortrait,TimerObject,ScribeWindow,CaseSummary = null;

    public TextMeshProUGUI JudgeRooText,TimerText,CourtStateText,MattersCountText = null;
    private bool loading = false;
    public TMP_InputField ScribeInput = null;
    public int MattersCountCurrent  = 0;
    public int MattersCountMax = 0;
    public RectTransform NamesList = null;
    public List<KCMatters> MyCourtMatters = new List<KCMatters>();
    public GameObject CourtNameTypeObject = null;
    public List<GameObject> ObjectsToReveal = new List<GameObject>();
    private List<string> Names = new List<string>();
    private KCMatters CurrentMatter = null;

    private string Defendant,Prosecutor,Witness,WitnessProsecutor,Verdict = string.Empty;
    public void Awake(){
        KCCourtManager.instance = this;
    }
    public void Start(){

       
        StartCoroutine(WaitForBanner());
    }

    public void UpdateMatterCounter(){
        MattersCountText.text = string.Concat("Matters:",MattersCountCurrent.ToString(),"/",MattersCountMax.ToString());
    }

    private void LoadMatters(){
        List<KCMatters> mats = KCGameManager.instance.Matters;
        for(int i = 0; i < mats.Count ; i++){
            try{
                MyCourtMatters.Add(mats[i]);
            }catch{
                continue;
            }
        }
    }

    IEnumerator WaitForBanner(){
        yield return new WaitForSeconds(1);

        yield return new WaitForSeconds(2.5f);
         LoadSomeNames();
         LoadMatters();
         MyCourtState = ScriptableObject.Instantiate(MyCourtState);
        MyCourtState.InitialiseCourtState();
        MattersCountMax = KCGameManager.instance.Trials_Max;
        UpdateMatterCounter();
        StartCoroutine(CheckStateRun());
    }

    IEnumerator CheckStateRun(){
        RunningCheckState = true;
        yield return new WaitForSeconds(1.03f);
        RunningCheckState = false;
         if(MyCourtState != null){
              StartCoroutine(CheckStateRun());
             try{
                 if(!MyCourtState.StateCompleted){
           ProcessCourtState();
         }else{
            MyCourtState = ScriptableObject.Instantiate( MyCourtState.NextState);
            MyCourtState.InitialiseCourtState();
         }
        }catch{

        }
         }
        
     
    }

    public void ProcessCourtState(){
        if(waiting){
            return;
        }
        CourtStateText.text = string.Concat("Court is: ",MyCourtState.MyState);
        MyCourtState.EvaluateCourtState();
        CaseSummaryScript.StatusTMpro.text = MyCourtState.MyState.ToString();
        if(MyCourtState.StateCompleted){
            waiting = true;
            switch(MyCourtState.MyState){
                case KCStaticEnums.CourtState.newCase:
                    MattersCountCurrent ++;
                    CaseSummary.gameObject.SetActive(true);
                    UpdateMatterCounter();
                    CaseSummaryScript.MatterNumberTMpro.text = string.Concat("Matter: #",MattersCountCurrent);
                    CurrentMatter = ObtainARandomMatter();    
                    Defendant = string.Concat("Defendant:",ObtainRandomName());
                          Prosecutor = string.Concat("Prosecutor:",ObtainRandomName());
                    CaseSummaryScript.DefendantTMpro.text = Defendant;
                    CaseSummaryScript.ProsecutorTMpro.text = Prosecutor;
                    ObjectsToReveal.Add(CaseSummaryScript.MatterNumberTMpro.gameObject);
                    ObjectsToReveal.Add(CaseSummaryScript.DefendantTextObject);
                    ObjectsToReveal.Add(CaseSummaryScript.ProsecutorTextObject);
                    ObjectsToReveal.Add(CaseSummaryScript.AccusationTextObject);
                    ObjectsToReveal.Add(CaseSummaryScript.MatterTitleTextObject);
                    ObjectsToReveal.Add(CaseSummaryScript.CaseStatusText);
                    ObjectsToReveal.Add(CaseSummaryScript.StatusTextObject);
                    
                    CaseSummaryScript.MatterTMpro.text = CurrentMatter.MatterTitle;
              
                    if(KCGameManager.instance.Defendant_Witness > 0 && KCGameManager.instance.People.Count >= 3){
                          Witness = ObtainRandomName();
                       // ObjectsToReveal.Add(CaseSummaryScript.WitnessLabelTextObject);
                       // ObjectsToReveal.Add(CaseSummaryScript.WitnessDefenceText);
                    }
                    if(KCGameManager.instance.Prosecution_Witness > 0 && KCGameManager.instance.People.Count >= 4){
                          WitnessProsecutor = ObtainRandomName();
                        //   ObjectsToReveal.Add(CaseSummaryScript.WitnessProsecLabelTextObject);
                      //  ObjectsToReveal.Add(CaseSummaryScript.WitnessProsecutorText);
                    }
                  
                    StartCoroutine(RevalObjectAfterDelay());

                    
                break;

                case KCStaticEnums.CourtState.defendantArg:
                    TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Defendant_Arg);
                break;
                case KCStaticEnums.CourtState.prosecArg:
                      TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Prosecution_Arg);
                break;
                 case KCStaticEnums.CourtState.witnessDef:
                 if(Witness != string.Empty){
                        TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Defendant_Witness);
                 }else{
                    PassStatementToJudgeRoo("There are no Witnesses for the Defence");
                    waiting = false;
                 }
                  
                break;

                case KCStaticEnums.CourtState.witnessProsec:
                   if(WitnessProsecutor != string.Empty){
                        TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Prosecution_Witness);
                 }else{
                    PassStatementToJudgeRoo("There are no Witnesses for the Prosecution");
                    waiting = false;
                 }
                break;

                case KCStaticEnums.CourtState.closingDefArg:
                     TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Defendant_Closing);
                break;
                
                case KCStaticEnums.CourtState.closingProsecArg:
                 TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Prosecution_Closing);
                break;

                case KCStaticEnums.CourtState.closeCase:
                    TimerObject.SetActive(true);
                    MyClock.InitClock(5);
                    //TODO put a bit here to vote for Verdict
                break;

                default:
                waiting = false;
                break;
            }
        }
    }

    private string ObtainRandomName(){
        if(!Names.Any()){
            LoadSomeNames();
        }
        string name = string.Empty;
        try{
              int rand = Random.Range(0,Names.Count);
         name = Names[rand];
        Destroy(NamesList.GetChild(rand).gameObject);
        Names.RemoveAt(rand);
        }catch{

        }
      
        return name;
    }

    public KCMatters ObtainARandomMatter(){
        if(!MyCourtMatters.Any()){
            LoadMatters();
        }
       
        try{
            int rand = Random.Range(0,MyCourtMatters.Count);
            KCMatters mat = MyCourtMatters[rand];
            MyCourtMatters.RemoveAt(rand);
            return mat;

        }catch{

        }
        return null;
    }

    #region  SCENEINTERACTION
    public void LoadLobby(){
        if(loading){
            return;
        }
        loading = true;
        if(RunningCheckState){
            StopCoroutine(CheckStateRun());
        }
        KCGameManager.instance.LoadSceneName( KCStaticEnums.SceneNames.lobby);
    }
    #endregion

    private void LoadSomeNames(){
        for(int i = 0; i < KCGameManager.instance.People.Count ; i++)
        {
            try{
                GameObject go = Instantiate(CourtNameTypeObject, NamesList);
                KCLabelListItem lbl = go.GetComponent<KCLabelListItem>();
                lbl.id = i;
                lbl.list = KCStaticEnums.PopulateableLists.people;
                lbl.label.text = KCGameManager.instance.People[i];
                Names.Add(KCGameManager.instance.People[i]);
            }catch{

            }
        }
    }

    public void PassStatementToJudgeRoo(string stat){
         JudgeRooText.text = string.Empty;
        judgeRooStatement = stat;
        timeSliceJudgeRoo = 2 * (judgeRooStatement.Length / 100) / judgeRooStatement.Length;
        judgeRooStringPositioning = 0;
        busy = true;
        StartCoroutine(JudgeRooStatementReading());
    }

    IEnumerator JudgeRooStatementReading(){
        yield return new WaitForSeconds(timeSliceJudgeRoo);
        try{
             JudgeRooText.text = string.Concat(JudgeRooText.text,judgeRooStatement[judgeRooStringPositioning]);
            judgeRooStringPositioning ++;
            if(judgeRooStringPositioning < judgeRooStatement.Length){
                StartCoroutine(JudgeRooStatementReading());
            }else{
                StartCoroutine(JudgeRooStatementWashup());
               
            }
        }catch{
            MyCourtState = MyCourtState.NextState; 
        }
       
    }

    IEnumerator JudgeRooStatementWashup(){
        yield return new WaitForSeconds(2f);
        busy = false;
         
    }


    #region NewCase

    IEnumerator RevalObjectAfterDelay(){
        yield return new WaitForSeconds(.5f);
        if(ObjectsToReveal.Any()){
            ObjectsToReveal[0].gameObject.SetActive(true);
            ObjectsToReveal.RemoveAt(0);
            StartCoroutine(RevalObjectAfterDelay());
        }else{
            waiting = false;
        }
    }

    #endregion
}
