using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class PlayerMenu : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playersInGameText;
    public GameObject menuCanvas;
    public PlayerMovement pm;
    public CameraMovement cm;
    public TagGamemode gameManager;
    public PlayerData pd;
    public bool busy;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<TagGamemode>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsLocalPlayer){
            if(Input.GetKeyDown(KeyCode.Escape) && !busy){
                if(pm.inMenu){
                    hideMenu();
                }
                else{
                    showMenu();
                }
            }
        }
    }
    void showMenu(){
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuCanvas.SetActive(true);
        cm.enabled = false;
        pm.inMenu = true;
    }
    void hideMenu(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuCanvas.SetActive(false);
        cm.enabled = true;
        pm.inMenu = false;
    }
    public void LeaveGame(){
        if(IsHost || IsServer){
            if(gameManager.hasStarted.Value){
                gameManager.endGameServerRpc();
            }
            Debug.Log("server/host attempt to leave");
            LeaveGameServerRpc();
        }
        else if(IsClient){
            if(pd.serverTagged.Value){
                if(gameManager.getTaggedCount() == 0){
                    gameManager.endGameServerRpc();
                }
            }
            Debug.Log("Client attempt to leave");
            NetworkManager.Singleton.Shutdown();
        }
        else{
            Debug.Log("Error");
        }
    }
    [ClientRpc]
    public void LeaveGameClientRpc(){
        NetworkManager.Singleton.Shutdown(discardMessageQueue: true);
        gameManager.hasStarted.Value = false;
        gameManager.finished.Value = false;
    }
    [ServerRpc]
    public void LeaveGameServerRpc(){
        NetworkManager.Singleton.Shutdown(discardMessageQueue: true);
    }
}
