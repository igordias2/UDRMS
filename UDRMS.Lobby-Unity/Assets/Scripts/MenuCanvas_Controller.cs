using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCanvas_Controller : MonoBehaviour
{
    public static MenuCanvas_Controller Instance;
    void Awake(){
        if(Instance == null)
            Instance = this;
    }
    [Header("Canvas Holder")]
    public GameObject LoginGO, LobbyGO, MatchLobbyGO;
    
    [Header("Login")]
    public GameObject[] LoginObjs;
     public void ActiveLogin(){
        foreach (GameObject item in LoginObjs)
        {
            if(item.GetComponent<TMPro.TMP_InputField>()){
                item.GetComponent<TMPro.TMP_InputField>().interactable = true;
            }
            
            if(item.GetComponent<UnityEngine.UI.Button>()){
                item.GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
            //item.SetActive(true);
        }
    }
    public void SendLogin(string playerName){
        NetworkManager.Instance.Login(playerName);
    }
    
    [Header("Lobby")]
    public GameObject MatchLobbyPrefab;
    public Transform LobbyContent;
    public List<GameObject> MatchLobbys = new List<GameObject>();
    public void ClearMatchLobbys_Lobby(){
        MatchLobbys.Clear();
    }
    public void AddMatchLobby_Lobby(ushort matchID){
        GameObject ml = Instantiate(MatchLobbyPrefab, LobbyContent);
        Match_Prefab_Controller match = ml.GetComponent<Match_Prefab_Controller>();
        match.SetMatchID(matchID);
        MatchLobbys.Add(ml);
    }

   
    public void SwitchTo(ClientState state){
        LoginGO.SetActive(false);
        LobbyGO.SetActive(false); 
        MatchLobbyGO.SetActive(false);

        switch (state)
        {
            case ClientState.AtLoginScreen: 
                LoginGO.SetActive(true); break;
            case ClientState.AtLobbyScreen: 
                LobbyGO.SetActive(true);
                NetworkManager.Instance.GetLobbyInfo();
                //TODO: Gather Info on change to Lobby
                break;
            case ClientState.AtLobbyMatchScreen:
                MatchLobbyGO.SetActive(true);
                NetworkManager.Instance.GetMatchLobbyInfo();
                //TODO: Gather Info on change to Lobby Match
                break;
        }
    }
}
