using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDRMS.Shared;

public class Player_Manager : MonoBehaviour
{
    public static Player_Manager Instance;
    public Lobby_Player lobby_Player;
    void Awake(){
        if(Instance == null){
            Instance = this;
        }
    }
    ushort ID;
    string playerName;
    ClientState state;

    public void SetState(ClientState state){
        this.state = state;
        MenuCanvas_Controller.Instance.SwitchTo(this.state);
    }
    public ClientState GetState(){
        return this.state;
    }
    public void SetPlayer(Lobby_Player lby_Player){
        this.lobby_Player = lby_Player;
    }
}
