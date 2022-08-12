using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Tag.Core.Singleton;
using TMPro;
public class PlayersManager : Singleton<PlayersManager>
{
    public GameObject hideMenu;
    public GameObject hidePlayerGUI;
    private NetworkVariable<int> numPlayers = new NetworkVariable<int>();
    public int playersInGame
    {
        get{
            return numPlayers.Value +1;
        }
    }
    private void Start(){
        NetworkObject.DontDestroyWithOwner = true;
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if(IsServer){
                numPlayers.Value++;
                Debug.Log($"{id} just connected...");
            }
            DisplayMenu(false);
        };
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if(IsServer){
                numPlayers.Value--;
                Debug.Log($"{id} just disconnected...");
            }
            Debug.Log("Client disconnected");
            DisplayMenu(true);
        };
    }
    void Update(){
        if(NetworkManager.Singleton.ShutdownInProgress){
            Debug.Log("Server shutdown");
            DisplayMenu(true);
        }
    }
    void DisplayMenu(bool val){
        hideMenu.SetActive(val);
        hidePlayerGUI.SetActive(!val);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = val;
    }
}
