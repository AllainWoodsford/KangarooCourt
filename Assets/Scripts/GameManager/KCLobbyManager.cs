using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class KCLobbyManager : MonoBehaviour
{
   public GameObject LabelListElement;

   public RectTransform PeopleContent,MatterContent;

   public TMP_InputField Setting_Defendant_Arg,Setting_Prosecution_Arg,Setting_Witness_Defence,Setting_Witness_Prosecution,
   Setting_Closing_Defendant,Setting_Closing_Prosecution,Setting_Trials_Max;

   public TMP_InputField Person,MatterTitle,MatterBody;

#region init
   public void Start(){
        if(KCGameManager.instance.IsVisistedLobby){
            SettingsSetupInputSettings(
                KCGameManager.instance.Defendant_Arg,
                KCGameManager.instance.Defendant_Witness,
                KCGameManager.instance.Defendant_Closing,
                KCGameManager.instance.Prosecution_Arg,
                KCGameManager.instance.Prosecution_Witness,
                KCGameManager.instance.Prosecution_Closing,
                KCGameManager.instance.Trials_Max
            );

        }else{
            KCGameManager.instance.IsVisistedLobby = true;
            SettingsSetupInputSettings(
                KCStaticEnums.TIMER_DEFENDANT_PLEA,
                KCStaticEnums.TIMER_WITNESS_STATEMENT_DEFENCE,
                 KCStaticEnums.TIMER_DEFENDANT_CLOSING_ARGUMENT,
                  KCStaticEnums.TIMER_PROSECUTOR_ARGUMENT,
                   KCStaticEnums.TIMER_WITNESS_STATEMENT_PROSECUTION,
                    KCStaticEnums.TIMER_PROSECUTOR_CLOSING_ARGUMENT,
                    KCStaticEnums.COUNTER_MAXIMUM_TRIALS_DEFAULT
            );
        }

        if( KCGameManager.instance.Matters.Any()){
            PopulateMattersListFromGameManager(KCGameManager.instance.Matters);
        }
        if(KCGameManager.instance.People.Any()){
            PopulatePeopleListFromGameManager(KCGameManager.instance.People);
        }
   }

   private void PopulateMattersListFromGameManager(List<KCMatters> mats){
    for(int i = 0; i < mats.Count ; i++){
        try{
            AddListLabelElement(
               KCStaticEnums.PopulateableLists.matters,
        mats[i].MatterTitle, i,
        MatterContent 
            );
        }catch{
            continue;
        }
    }
   }

   private void PopulatePeopleListFromGameManager(List<string> people){
    for(int i = 0; i < people.Count ; i++){
        try{
                 AddListLabelElement(
               KCStaticEnums.PopulateableLists.people,
        people[i], i,
        PeopleContent
            );
        }catch{
            continue;
        }
    }
   }
#endregion

#region sceneinterraction
public void LoadCourt(){
    PushSettingsToGameManager();
    if(KCGameManager.instance.People.Count < 2){
         KCGameManager.instance.CreateErrorMessage("Court needs at least 2 people!", 2);
        return;
    }
    if(!KCGameManager.instance.Matters.Any()){
         KCGameManager.instance.CreateErrorMessage("There are no Matters to discuss in Court! Make some.", 2);
    }
 KCGameManager.instance.LoadSceneName( KCStaticEnums.SceneNames.court);
}

public void LoadMenu(){
    PushSettingsToGameManager();
 KCGameManager.instance.LoadSceneName( KCStaticEnums.SceneNames.mainMenu);
}

public void AddPerson(){
    if(Person.text == string.Empty){
         KCGameManager.instance.CreateErrorMessage("Person field needs to be populated first!", 2);
        return;
    }
    try{
         int personCount = KCGameManager.instance.People.Count;
    
        KCGameManager.instance.AddToPeopleList(Person.text);
   
       
        if(personCount < KCGameManager.instance.People.Count){
            //added succesffuly
             AddListLabelElement( KCStaticEnums.PopulateableLists.people,
        Person.text, KCGameManager.instance.People.Count-1,
        PeopleContent
        );

         Person.text = string.Empty;
        }
    }catch{

    }
   
}

public void AddMatter(){
    
    if(MatterBody.text == string.Empty || MatterTitle.text == string.Empty){
         KCGameManager.instance.CreateErrorMessage("Fillout Matter Title and Sentancing!", 2);
        return;
    }
    try{
          int matterCount = KCGameManager.instance.Matters.Count;
    KCGameManager.instance.AddToMatterList(MatterTitle.text,MatterBody.text);
    if(matterCount < KCGameManager.instance.Matters.Count){
        //added successfully
        AddListLabelElement( KCStaticEnums.PopulateableLists.matters,
        MatterTitle.text, KCGameManager.instance.Matters.Count-1,
        MatterContent
        );

       MatterTitle.text = string.Empty;
       MatterBody.text = string.Empty;
    }
    }catch{
         KCGameManager.instance.CreateErrorMessage("Something went wrong making the Matter!", 2);
        return;
    }
  
}

public void AddMatterAuto(KCMatters matter){
     try{
          int matterCount = KCGameManager.instance.Matters.Count;
    KCGameManager.instance.AddToMatterList(matter.MatterTitle,matter.MatterSentancing);
    if(matterCount < KCGameManager.instance.Matters.Count){
        //added successfully
        AddListLabelElement( KCStaticEnums.PopulateableLists.matters,
        matter.MatterTitle, KCGameManager.instance.Matters.Count-1,
        MatterContent
        );

       
    }
    }catch{
         KCGameManager.instance.CreateErrorMessage("Something went wrong making the Matter!", 2);
        return;
    }
}

public void AutoPopulateMatter(){
    List<KCMatters> mats = KCGameManager.instance.DefaultMatters;
    for(int i = 0; i < mats.Count ; i++){
        try{
          AddMatterAuto(mats[i]);
        }catch{
            continue;
        }
    }
}

public void ClearMatters(){
    if(KCGameManager.instance.Matters.Any()){
        KCGameManager.instance.Matters.Clear();
        for(int i = MatterContent.childCount - 1; i >= 0 ; i-- ){
            Destroy(MatterContent.GetChild(i).gameObject);
        }
    }
}

/// <summary>
/// AddToListLabel
/// </summary>
/// <param name="location"></param>
/// <param name="label"></param>
/// <param name="id"></param>
/// <param name="area"></param>
private void AddListLabelElement(KCStaticEnums.PopulateableLists location
,string label, int id, RectTransform area
){
      GameObject go = Instantiate(LabelListElement,area);
       KCLabelListItem item = go.GetComponent<KCLabelListItem>();
       item.list = location;
       item.id = id;
       item.label.text = label;
}
#endregion

#region  settings


    public void PushSettingsToGameManager(){
        try{
              KCGameManager.instance.MapSettingsFromLobby(
        int.Parse(Setting_Defendant_Arg.text), 
        int.Parse(Setting_Prosecution_Arg.text), 
        int.Parse(Setting_Witness_Defence.text),
        int.Parse(Setting_Witness_Prosecution.text), 
        int.Parse(Setting_Closing_Defendant.text),
        int.Parse(Setting_Closing_Prosecution.text), 
        int.Parse(Setting_Trials_Max.text)
        );
        }catch{
            KCGameManager.instance.CreateErrorMessage("Settings should be numbers only!", 2);
        }
      
    }

    /// <summary>
    /// Setupsettings
    /// </summary>
    /// <param name="defendant_arg"></param>
    /// <param name="defendant_witness"></param>
    /// <param name="defendant_closing"></param>
    /// <param name="prosecution_arg"></param>
    /// <param name="prosecution_witness"></param>
    /// <param name="prosecution_closing"></param>
   private void SettingsSetupInputSettings(
    int defendant_arg,
    int defendant_witness,
    int defendant_closing,
    int prosecution_arg,
    int prosecution_witness,
    int prosecution_closing,
    int trials_max
   ){
    Setting_Defendant_Arg.text = defendant_arg.ToString();
    Setting_Prosecution_Arg.text = prosecution_arg.ToString();
    Setting_Witness_Defence.text = defendant_witness.ToString();
    Setting_Witness_Prosecution.text = prosecution_witness.ToString();
   Setting_Closing_Defendant.text = defendant_closing.ToString();
   Setting_Closing_Prosecution.text = prosecution_closing.ToString();
   Setting_Trials_Max.text = trials_max.ToString();
   }
   #endregion
}
