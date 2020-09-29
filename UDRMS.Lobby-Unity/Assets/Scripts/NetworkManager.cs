
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.Net.Sockets;


using UDRMS.Shared;

using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    void Awake(){
        if(NetworkManager.Instance == null)
            Instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }
    public UnityClient client;
    public IPAddress iP;

    void Start(){
        if(client == null)
            client = GetComponent<UnityClient>();

        
        client.ConnectInBackground(iP, 4296, 4297, true);
        
        client.MessageReceived += Client_MessageReceived;
    }

    void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage() as Message)
        {
            using (DarkRiftReader r = message.GetReader())
            {
                if(message.Tag == UDRMS_Tags.connectedToMS){
                    //TODO: Login
                    MenuCanvas_Controller.Instance.ActiveLogin();
                }
                if(message.Tag == UDRMS_Tags.LoginInfo){
                    bool canLogin = r.ReadBoolean();

                    if(canLogin)
                    {
                        Player_Manager.Instance.SetState(ClientState.AtLobbyScreen);
                    }
                    else
                    {
                        //TODO: Refresh Login
                    }

                } 
                if(message.Tag == UDRMS_Tags.getLobbyMatchs){
                    MenuCanvas_Controller.Instance.ClearMatchLobbys_Lobby();
                    while(r.Position < r.Length){
                        ushort matchID = r.ReadUInt16();
                        ushort matchOwnerID = r.ReadUInt16();
                        ushort actualPlayers = r.ReadUInt16();
                        ushort maxPlayersInMatch = r.ReadUInt16();

                        MenuCanvas_Controller.Instance.AddMatchLobby_Lobby(matchID);
                    }
                }

                if(message.Tag == UDRMS_Tags.createLobbyMatch){

                }

                if(message.Tag == UDRMS_Tags.getLobbyMatchInfo){

                }
               
                //Connectou-se ao Lobby da Partida
                if(message.Tag == UDRMS_Tags.connectLobbyMatch)
                {
                    bool canJoin = r.ReadBoolean();
                    ushort matchID = r.ReadUInt16();

                    if(canJoin){
                        //TODO: Move to Room
                        Player_Manager.Instance.SetState(ClientState.AtLobbyMatchScreen);
                    }
                }
            }
        }
    }


    internal void GetLobbyInfo(ushort page = 1)
    {        
        using (DarkRiftWriter w = DarkRiftWriter.Create())
        {
            w.Write(page);
            using (Message m = Message.Create(UDRMS_Tags.getLobbyMatchs, w))
                client.SendMessage(m, SendMode.Reliable);
        }
    }

    internal void GetMatchLobbyInfo()
    {
        throw new NotImplementedException();
    }
    public void Match_JoinRoom(ushort roomID){
        using (DarkRiftWriter w = DarkRiftWriter.Create())
        {
            w.Write(roomID);
            using (Message m = Message.Create(UDRMS_Tags.connectLobbyMatch, w))
                client.SendMessage(m, SendMode.Reliable);
        }
    }
    public void Match_CreateRoom(){
        using (DarkRiftWriter w = DarkRiftWriter.Create())
            using (Message m = Message.CreateEmpty(UDRMS_Tags.createLobbyMatch))
                client.SendMessage(m, SendMode.Reliable);
    }
    public void Login(string playerName){
        using (DarkRiftWriter w = DarkRiftWriter.Create())
        {
            w.Write(playerName);
            using (Message m = Message.Create(UDRMS_Tags.LoginInfo, w))
                client.SendMessage(m, SendMode.Reliable);
        }
    }
}
public enum ClientState{
    AtLoginScreen = 0, // Logando
    AtLobbyScreen = 1, //Main Menu
    AtLobbyMatchScreen = 2, // On Lobby Match
    AtPlayingMatch = 3 // Playing Match
}