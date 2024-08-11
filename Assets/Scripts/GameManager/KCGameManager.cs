using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KCGameManager : MonoBehaviour
{
    public static KCGameManager instance = null;
    // Start is called before the first frame update
    [Header("StringLists")]
    public List<string> People = new List<string>();
    [Header("Matters")]
    public List<KCMatters> Matters = new List<KCMatters>();
    public List<KCMatters> DefaultMatters = new List<KCMatters>();

    [Header("Misc")]
    private bool isLoading = false;
    public int Defendant_Arg = 0;
    public int Defendant_Witness = 0;
    public int Defendant_Closing = 0;
    public int Prosecution_Arg = 0;
    public int Prosecution_Witness = 0;
    public int Prosecution_Closing = 0;
    public int Trials_Max = 0;
    public bool IsVisistedLobby = false;
    public GameObject ErrorMessage = null;
    #region  init
    public void Awake()
    {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    #endregion

    #region  SceneLoadingDelegation
    public void OnEnable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        isLoading = false;
    }

    public void OnDisable(){
        SceneManager.sceneLoaded  -= OnSceneLoaded;
    }

    public void LoadSceneName(KCStaticEnums.SceneNames name){
        if(isLoading){
            return;
        }
        isLoading = true;
        SceneManager.LoadScene(name.ToString());
    }


   
    #endregion

   #region Listops
   /// <summary>
   /// 
   /// </summary>
   /// <param name="id"></param>
     public void RemoveFromPeopleList(int id){
        try{
            People.RemoveAt(id);
        }catch{
            return;
        }
    }
     /// <summary>
     /// 
     /// </summary>
     /// <param name="id"></param>
      public void RemoveFromMattersList(int id){
        try{
            Matters.RemoveAt(id);
        }catch{
            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public void AddToPeopleList(string name){
        if(People.Contains(name)){
            return;
        }
        People.Add(name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="sentancing"></param>
    public void AddToMatterList(string name, string sentancing){
        try{
            KCMatters mat = new KCMatters(name,sentancing);
            Matters.Add(mat);  
        }catch{

        }
     
    }

   
   #endregion

   #region Settings
    public void MapSettingsFromLobby(
          int defendant_arg,
        int defendant_witness,
        int defendant_closing,
        int prosecution_arg,
        int prosecution_witness,
        int prosecution_closing,
        int trials_max
    ){
        Defendant_Arg = defendant_arg;
        Defendant_Witness = defendant_witness;
        Defendant_Closing = defendant_closing;
        Prosecution_Arg = prosecution_arg;
        Prosecution_Witness = prosecution_witness;
        Prosecution_Closing = prosecution_closing;
        Trials_Max = trials_max;
    }

   #endregion

   #region  ErrorMessages
    public void CreateErrorMessage(string msg, float time){
        try{
              GameObject go = Instantiate(ErrorMessage,StsKCCanvasHolder.holder.MyRect);
        
            KCErrorMessage er = go.GetComponent<KCErrorMessage>();
            er.InitMe(msg,time);
        }catch{

        }
      
    }

   #endregion
   
}
