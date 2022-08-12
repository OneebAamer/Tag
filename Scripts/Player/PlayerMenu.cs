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
    private PlayerMovement pm;
    private CameraMovement cm;
    public TagGamemode gameManager;
    // Start is called before the first frame update
    void Start()
    {
        cm = gameObject.GetComponentInChildren<CameraMovement>();
        pm = gameObject.GetComponent<PlayerMovement>();
        gameManager = GameObject.Find("GameManager").GetComponent<TagGamemode>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsLocalPlayer){
            if(Input.GetKeyDown(KeyCode.Escape)){
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
            Debug.Log("server/host attempt to leave");
            LeaveGameServerRpc();
        }
        else if(IsClient){
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
