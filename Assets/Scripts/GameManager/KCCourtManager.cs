using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Linq.Expressions;
public class KCCourtManager : MonoBehaviour
{
    public List<string> JudgeRoosStatements = new List<string>();
    public static KCCourtManager instance = null;
    public bool busy,waiting,awaitingJury = false; //waiting is for the timed events, busy is for Roo's statmetns
    private int judgeRooStringPositioning = 0;
    private float timeSliceJudgeRoo,timeSliceJudgeRooStatementLong = 0;
    private string judgeRooStatement = string.Empty;
    public KCStaticEnums.CourtState MyState = KCStaticEnums.CourtState.init;
    public KCStaticEnums.Verdict MyCurrentCaseVerdict = KCStaticEnums.Verdict.none;
    public KCCourtClock MyClock = null;
    public KCCourtState MyCourtState = null;
    public KCCourtState CloseCaseState = null;
    public KCCourtState EndGameState = null;
    public KCCaseSummary CaseSummaryScript = null;
    public bool RunningCheckState = false;
    public GameObject JudgeRooPortrait,TimerObject,ScribeWindow,CaseSummary,SkipCase,AcceptCase = null;

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
        yield return new WaitForSeconds(.1f);

        yield return new WaitForSeconds(.1f);
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
        yield return new WaitForSecondsRealtime(1);
        RunningCheckState = false;
         if(MyCourtState != null){
            MyState = MyCourtState.MyState;
              StartCoroutine(CheckStateRun());
             try{
                 if(!MyCourtState.StateCompleted && !awaitingJury){
                    ProcessCourtState();
                     
                     
                 }
                 else{
                    if(!awaitingJury){
                        MyCourtState = ScriptableObject.Instantiate( MyCourtState.NextState);
                        MyCourtState.InitialiseCourtState();
                    }
                
            }
             }
            catch{
                 StartCoroutine(CheckStateRun());
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
                case KCStaticEnums.CourtState.end:
                    CaseSummary.gameObject.SetActive(false);
                    TimerObject.SetActive(false);
                    JudgeRooPortrait.gameObject.SetActive(false);
                    StopCoroutine(CheckStateRun());
                break;

                case KCStaticEnums.CourtState.init:
                    waiting = false;
                    PassStatementToJudgeRoo(string.Format("We have {0} Matters to attend to lets be underway.",MattersCountMax));
                break;
                case  KCStaticEnums.CourtState.hearing:
                Debug.Log("Hearing");
                    KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.attention);
                    MyCourtState.StateCompleted = false;
                   TimerObject.SetActive(true);
                  MyClock.InitClock(KCGameManager.instance.Jury_Time);
               
                    ObjectsToReveal.Add(SkipCase);
                    ObjectsToReveal.Add(AcceptCase);
                     PassStatementToJudgeRoo("Shall we proceed with this case?");
                          StartCoroutine(RevalObjectAfterDelay());
                         
                break;

                case KCStaticEnums.CourtState.newCase:
                KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.lawAndOrder);
                     Debug.Log("New Case");
                MyCurrentCaseVerdict = KCStaticEnums.Verdict.none;
                Witness = string.Empty;
                WitnessProsecutor = string.Empty;
                    MattersCountCurrent ++;
                    if(MattersCountCurrent > MattersCountMax){
                        waiting = false;
                        MyCourtState = ScriptableObject.Instantiate(EndGameState);
                    }else{
                         CaseSummary.gameObject.SetActive(true);
                    UpdateMatterCounter();
                    CaseSummaryScript.MatterNumberTMpro.text = string.Concat("Matter: #",MattersCountCurrent);
                    CurrentMatter = ObtainARandomMatter();    
                    Defendant = ObtainRandomName();
                          Prosecutor = ObtainRandomName();
                    CaseSummaryScript.DefendantTMpro.text = string.Concat("Defendant:",Defendant);;
                    CaseSummaryScript.ProsecutorTMpro.text = string.Concat("Prosecutor:",Prosecutor);;
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
                        ObjectsToReveal.Add(CaseSummaryScript.WitnessLabelTextObject);
                     ObjectsToReveal.Add(CaseSummaryScript.WitnessDefenceText);
                      CaseSummaryScript.DefnceWitnessTMpro.text = Witness;
                        
                    }
                    if(KCGameManager.instance.Prosecution_Witness > 0 && KCGameManager.instance.People.Count >= 4){
                          WitnessProsecutor = ObtainRandomName();
                           ObjectsToReveal.Add(CaseSummaryScript.WitnessProsecLabelTextObject);
                        ObjectsToReveal.Add(CaseSummaryScript.WitnessProsecutorText);
                     CaseSummaryScript.ProsecWitnessTMpro.text = WitnessProsecutor;
                    
                    }
                  
                    StartCoroutine(RevalObjectAfterDelay());
                    waiting = false;
                    }
                   
                    
                break;

                case KCStaticEnums.CourtState.defendantArg:
                  KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.attention);
                     Debug.Log("Def ARg");
                    TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Defendant_Arg);
                break;
                case KCStaticEnums.CourtState.prosecArg:
                  KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.attention);
                      TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Prosecution_Arg);
                break;
                 case KCStaticEnums.CourtState.witnessDef:
                   KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.attention);
                 if(Witness != string.Empty && KCGameManager.instance.Defendant_Witness > 0){
                        TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Defendant_Witness);
                 }else{
                    PassStatementToJudgeRoo("There are no Witnesses for the Defence");
                    waiting = false;
                 }
                  
                break;

                case KCStaticEnums.CourtState.witnessProsec:
                  KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.attention);
                   if(WitnessProsecutor != string.Empty && KCGameManager.instance.Prosecution_Witness > 0){
                        TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Prosecution_Witness);
                 }else{
                    PassStatementToJudgeRoo("There are no Witnesses for the Prosecution");
                    waiting = false;
                 }
                break;

                case KCStaticEnums.CourtState.closingDefArg:
                  KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.attention);
                     TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Defendant_Closing);
                break;
                
                case KCStaticEnums.CourtState.closingProsecArg:
                  KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.attention);
                 TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Prosecution_Closing);
                break;

                case KCStaticEnums.CourtState.jury:
                if(MyClock.ClockIsRunning){
                    MyClock.PauseClock();
                }
                awaitingJury = true;
                  KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.attention);
                    MyCourtState.StateCompleted = false;
                     Debug.Log("Jury");
                 // PassStatementToJudgeRoo(string.Concat("Its assumed if we can't come to a resolution in a timely manner the Defendant ", Defendant  ,"is Guilty!"));
                    ObjectsToReveal.Add(CaseSummaryScript.NotGuiltyButton);
                    ObjectsToReveal.Add(CaseSummaryScript.GuilityButton);
                       TimerObject.SetActive(true);
                    MyClock.InitClock(KCGameManager.instance.Jury_Time);
                    StartCoroutine(RevalObjectAfterDelay());
                 
                break;

                case KCStaticEnums.CourtState.closeCase:
                     CaseSummaryScript.NotGuiltyButton.SetActive(false);
                     CaseSummaryScript.GuilityButton.SetActive(false);
                     KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.attention);
                      Debug.Log("Close Case");
                    if(MyCurrentCaseVerdict == KCStaticEnums.Verdict.none || MyCurrentCaseVerdict == KCStaticEnums.Verdict.missTrial){
                             PassStatementToJudgeRoo(string.Concat("Let the record state that the Matter of "
                    ,CurrentMatter.MatterTitle, " for the Defendant is a Miss Trial, we will move onto more pressing Matters..."
                    ));   
                    }else{
                        if(MyCurrentCaseVerdict == KCStaticEnums.Verdict.Guilty){
                              PassStatementToJudgeRoo(string.Concat("With the power vested in me I sentance the Defendant "
                    ,Defendant, " to ", CurrentMatter.MatterSentancing, " let the Record state that Justice has been served!"
                    ));
                        }else{
                                PassStatementToJudgeRoo(string.Concat("The Defendant "
                    ,Defendant, " has been found Innocent they are free to hop away..."
                    ));
                        }
                        
                    }

                       Defendant = string.Empty;
                    Prosecutor = string.Empty;
                    Witness = string.Empty;
                    WitnessProsecutor = string.Empty;
                    CurrentMatter = null;
                    CaseSummaryScript.HideCaseSummaryObjects();
                    waiting = false;
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
            KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.click);
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
                if(KCGameManager.instance.People[i] != Defendant && KCGameManager.instance.People[i] != Witness){
                    if(!Names.Contains(KCGameManager.instance.People[i])){
                          GameObject go = Instantiate(CourtNameTypeObject, NamesList);
                        KCLabelListItem lbl = go.GetComponent<KCLabelListItem>();
                        lbl.id = i;
                        lbl.list = KCStaticEnums.PopulateableLists.people;
                        lbl.label.text = KCGameManager.instance.People[i];
                        Names.Add(KCGameManager.instance.People[i]);
                    }
                  
                }
              
            }catch{

            }
        }
    }

    public void PassStatementToJudgeRoo(string stat){
        if(JudgeRoosStatements.Any()){
            JudgeRoosStatements.Add(stat);
        }else{
            JudgeRoosStatements.Add(stat);
            PrepareStatementForSpeech(stat);
        }
       
       
    }

    private void PrepareStatementForSpeech(string stat){
         JudgeRooText.text = string.Empty;
        judgeRooStatement = stat;
        timeSliceJudgeRooStatementLong = stat.Length/50;
        timeSliceJudgeRoo = 2 * (judgeRooStatement.Length / 100) / judgeRooStatement.Length;
        judgeRooStringPositioning = 0;
        busy = true;
         StartCoroutine(JudgeRooStatementReading());
    }

    IEnumerator JudgeRooStatementReading(){
        yield return new WaitForSecondsRealtime(timeSliceJudgeRoo);
        try{
            KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.murmur);
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
        yield return new WaitForSecondsRealtime(2f + timeSliceJudgeRooStatementLong);
        try{
            if(TimerObject.activeInHierarchy && MyClock.ClockIsRunning && MyClock.expectedMaxTime > 0){
                MyClock.expectedMaxTime += (int)(2f + timeSliceJudgeRooStatementLong);
            }
                JudgeRoosStatements.RemoveAt(0);
                 if(JudgeRoosStatements.Any()){
                    PrepareStatementForSpeech(JudgeRoosStatements[0]);
                }else{
                    busy = false;
                }
        }catch{
            JudgeRoosStatements.Clear();
             busy = false; 
        }
       

         
    }

#region  Buttons

    public void ProceedWithCasePressed(bool result){
        SkipCase.SetActive(false);
        AcceptCase.SetActive(false);
            KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.click);
           MyCourtState.StateCompleted = true;
        if(result){
            //Proceed with case
            waiting = false;
            PassStatementToJudgeRoo("Very well we will Proceed with the Case!");
        }else{
            MyState = KCStaticEnums.CourtState.closeCase;

           MyCourtState = ScriptableObject.Instantiate(CloseCaseState );
            waiting = false;
             PassStatementToJudgeRoo("Case Dismissed!");
        }
    }
    public void PressVerdictButton(int i){
        CaseSummaryScript.GuilityButton.SetActive(false);
        CaseSummaryScript.NotGuiltyButton.SetActive(false);
        awaitingJury = false;
        if(MyCurrentCaseVerdict != KCStaticEnums.Verdict.none){
                KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.error);
            return;
        }
            KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.click);
        KCStaticEnums.Verdict verdict = KCStaticEnums.Verdict.none;
        switch(i){
            default:
                verdict = KCStaticEnums.Verdict.Guilty;
                                CaseSummaryScript.MatterNumberTMpro.text = string.Concat(CaseSummaryScript.MatterNumberTMpro.text, " - Verdict GUILTY");
            break;

            case 0:
                verdict = KCStaticEnums.Verdict.NotGuilty;
                                CaseSummaryScript.MatterNumberTMpro.text = string.Concat(CaseSummaryScript.MatterNumberTMpro.text, " - Verdict NOT GUILTY!");
            break;

            case 2:
                verdict = KCStaticEnums.Verdict.missTrial;
                CaseSummaryScript.MatterNumberTMpro.text = string.Concat(CaseSummaryScript.MatterNumberTMpro.text, " - Verdict MISS TRIAL");
            break;
        }

        if(verdict == KCStaticEnums.Verdict.missTrial || verdict == KCStaticEnums.Verdict.none){
             MyClock.PauseClock();
              PassStatementToJudgeRoo(string.Concat(
            "In the Matter of ", CurrentMatter.MatterTitle, 
            " The Defendant: ", Defendant , " is exhonerated, this is a miss trial!"
        ));
        }else{
            MyCurrentCaseVerdict = verdict;
            MyClock.PauseClock();
        PassStatementToJudgeRoo(string.Concat(
            "In the Matter of ", CurrentMatter.MatterTitle, 
            " The Defendant: ", Defendant , " has been found..."
        ));
        PassStatementToJudgeRoo(verdict.ToString().ToUpper());
        waiting = false;
        }

           MyCourtState.StateCompleted = true;
      
    }

#endregion

    #region NewCase

    IEnumerator RevalObjectAfterDelay(){
        yield return new WaitForSeconds(.5f);
        if(ObjectsToReveal.Any()){
            ObjectsToReveal[0].gameObject.SetActive(true);
            ObjectsToReveal.RemoveAt(0);
            StartCoroutine(RevalObjectAfterDelay());
        }else{
            if(MyCourtState.MyState != KCStaticEnums.CourtState.hearing && MyCourtState.MyState != KCStaticEnums.CourtState.jury ){
                waiting = false;
            }
           
            
        }
    }

    #endregion
}
