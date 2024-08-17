using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class KCCaseSummary : MonoBehaviour
{
   public GameObject
   DefendantTextObject,
   ProsecutorTextObject,
   AccusationTextObject,
   MatterTitleTextObject,
   StatusTextObject,
   CaseStatusText,
   WitnessLabelTextObject,
   WitnessDefenceText,
   WitnessProsecLabelTextObject,
   WitnessProsecutorText,
   NotGuiltyButton,
   GuilityButton
   ;

   public TextMeshProUGUI
   DefendantTMpro,
   ProsecutorTMpro,
   MatterTMpro,
   StatusTMpro,
   DefnceWitnessTMpro,
   ProsecWitnessTMpro,
   MatterNumberTMpro
   ;

   public void HideCaseSummaryObjects(){
      DefendantTextObject.SetActive(false);
    
   ProsecutorTextObject.SetActive(false);
   AccusationTextObject.SetActive(false);
   MatterTitleTextObject.SetActive(false);
   StatusTextObject.SetActive(false);
   CaseStatusText.SetActive(false);
   WitnessLabelTextObject.SetActive(false);
   WitnessDefenceText.SetActive(false);
   WitnessProsecLabelTextObject.SetActive(false);
   WitnessProsecutorText.SetActive(false);
   NotGuiltyButton.SetActive(false);
      GuilityButton.SetActive(false);
   }
}
