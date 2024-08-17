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
            KCAudioManager.instance.PlaySFX( KCStaticEnums.SoundNames.click);
        if(list == KCStaticEnums.PopulateableLists.people){
            KCGameManager.instance.RemovePeopleByName(label.text);
            KCLobbyManager.instance.UpdateSizeRect(list);
            
        }else{
            KCGameManager.instance.RemoveFromMattersList(id);
        }
        Destroy(gameObject);
    }
}
