using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KCLabelListItem : MonoBehaviour
{
    public KCStaticEnums.PopulateableLists list = KCStaticEnums.PopulateableLists.none;
    public int id = 0;
    public TextMeshProUGUI label = null;

    public void DeleteFromList(){
        if(list == KCStaticEnums.PopulateableLists.people){
            KCGameManager.instance.RemoveFromPeopleList(id);
        }else{
            KCGameManager.instance.RemoveFromMattersList(id);
        }
        Destroy(gameObject);
    }
}
