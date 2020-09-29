using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class Match_Player_Prefab_Controller : MonoBehaviour
{
    string playerName;
    bool matchOwner;
    [SerializeField] TMP_Text playerName_TXT;
    [SerializeField] Button playerRemoval_BTN;
    void Awake(){
        if(playerName_TXT == null){
            playerName_TXT = GetComponentInChildren<TMP_Text>();
            playerRemoval_BTN = GetComponentInChildren<Button>();
        }
    }
    public void Setup(string playerName){
        this.playerName = playerName;
        playerName_TXT.SetText(playerName);
    }

}
