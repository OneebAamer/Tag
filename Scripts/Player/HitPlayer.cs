using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class HitPlayer : NetworkBehaviour
{
    private PlayerData pd;
    private TagGamemode gameManager;
    public Camera playerCam;
    // Start is called before the first frame update
    void Start()
    {
        pd = GetComponent<PlayerData>();
        gameManager = GameObject.Find("GameManager").GetComponent<TagGamemode>();
    }

    // Update is called once per frame
    void Update()
    {
        pd.isTagged = pd.serverTagged.Value;
        if(IsLocalPlayer){
            if(Input.GetMouseButtonDown(0) && pd.isTagged){
                hit();
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
}
