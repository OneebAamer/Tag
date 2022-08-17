using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class HitPlayer : NetworkBehaviour
{
    private PlayerData pd;
    private CameraMovement cm;
    private TagGamemode gameManager;
    public Camera playerCam;
    public PlayerMovement pm;
    public GameObject gameSettingsMenu;
    public PlayerMenu menu;
    // Start is called before the first frame update
    void Start()
    {
        cm = gameObject.GetComponentInChildren<CameraMovement>();
        pd = GetComponent<PlayerData>();
        gameManager = GameObject.Find("GameManager").GetComponent<TagGamemode>();
    }

    // Update is called once per frame
    void Update()
    {
        pd.isTagged = pd.serverTagged.Value;
        if(IsLocalPlayer){
            if(Input.GetMouseButtonDown(0)){
                if(gameManager.hasStarted.Value){
                    if(pd.isTagged){
                        hit();
                    }
                }
                else{
                    if(IsHost){
                        openGameSettings();
                    }
                }
            }
        }
    }
    private void hit(){
        if(Physics.Raycast(playerCam.transform.position,playerCam.transform.forward,out RaycastHit rayHit, 5f)){
            GameObject playerHit = rayHit.transform.gameObject;
            if (playerHit.tag.Equals("Player")){
                PlayerData pd2 = playerHit.GetComponent<PlayerData>();
                gameManager.transferPlayerTagged(pd, pd2);
            }
        }
    }
    private void openGameSettings(){
        if(Physics.Raycast(playerCam.transform.position,playerCam.transform.forward,out RaycastHit rayHit, 5f)){
            GameObject playerHit = rayHit.transform.gameObject;
            if (playerHit.name.Equals("GameSettings")){
                if(!pm.inMenu){
                    menu.busy = true;
                    cm.enabled = false;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    gameSettingsMenu.SetActive(true);
                }
            }
        }
    }
}
