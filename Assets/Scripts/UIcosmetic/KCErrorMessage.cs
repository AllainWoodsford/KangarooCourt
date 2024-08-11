using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class KCErrorMessage : MonoBehaviour
{
   public bool isErrorMessage = true;
   public TextMeshProUGUI Text = null;
    public float timedLife = 1;
    /// <summary>
    /// Create error message that self destroys takes time and string
    /// </summary>
    /// <param name="val"></param>
    /// <param name="tl"></param>
   public void Start(){
      if(!isErrorMessage){
        
          StartCoroutine(DestroyMe());
      }
   }
   
   public void InitMe(string val, float tl){
    Text.text = val;
    timedLife = tl;
    StartCoroutine(DestroyMe());
   }

   IEnumerator DestroyMe(){
    yield return new WaitForSeconds(timedLife);
    Destroy(gameObject);
   }
}
