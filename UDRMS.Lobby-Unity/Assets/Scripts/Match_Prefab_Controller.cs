using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Match_Prefab_Controller : MonoBehaviour
{
    ushort matchID;
    public TMP_Text matchID_TXT;
    public Button matchJoin_BTN;
    void Awake(){
        if(matchID_TXT != null)
            return;
        
        matchID_TXT = GetComponentInChildren<TMP_Text>();
        matchJoin_BTN = GetComponentInChildren<Button>();
        //matchJoin_BTN.interactable = false;
    }
    public void SetMatchID(ushort id){
        matchID = id;
        matchJoin_BTN.onClick.AddListener(()=>{
            NetworkManager.Instance.Match_JoinRoom(this.matchID);
        });
        matchID_TXT.SetText(id.ToString());
    }

}
