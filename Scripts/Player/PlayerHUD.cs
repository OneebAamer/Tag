using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
public class PlayerHUD : NetworkBehaviour
{
    private Transform mainCamTransform;
    public GameObject nametag;
    public PlayerData pd;
    public override void OnNetworkSpawn()
    {
        if(IsLocalPlayer){
            nametag.SetActive(false);
        }
    }
    private void Update(){
        transform.LookAt(transform.position + mainCamTransform.rotation * Vector3.forward, mainCamTransform.rotation * Vector3.up);
        SendNameServerRpc(PlayerPrefs.GetString("Name"));
    }
    void Start(){
        mainCamTransform = Camera.main.transform;
    }
    [ServerRpc]
    void SendNameServerRpc(string nameToSend){
        SetPlayerNameClientRpc(nameToSend);
    }
    [ClientRpc]
    void SetPlayerNameClientRpc(string name){
        nametag.GetComponentInChildren<TextMeshProUGUI>().text = name;
        pd.username = name;
    }
}
