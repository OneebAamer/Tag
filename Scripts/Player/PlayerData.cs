using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
public class PlayerData : NetworkBehaviour
{
    public string username;
    public bool isTagged;
    public float playerSpeed = 8f;
    public float mouseSensitivity = 200f;
    public NetworkVariable<bool> serverTagged = new NetworkVariable<bool>();
    public Material tagMaterial;
    public Material untaggedMaterial;
    public GameObject playerBody;
    public PostProcessLayer vignette;
    public LayerMask taggedLayer;
    public LayerMask untaggedLayer;
    public TMP_Text crosshair;
    public NetworkVariable<int> points = new NetworkVariable<int>();

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = OwnerClientId.ToString();
        gameObject.tag = "Player";
        if(!IsLocalPlayer){
            crosshair.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        setPlayerTag(serverTagged.Value);
    }
    [ServerRpc]
    public void updateTaggedServerRpc(bool setFlag){
        setPlayerTag(setFlag);
        serverTagged.Value = setFlag;
    }
    [ServerRpc]
    public void swapTaggedServerRpc(bool setFlag, ulong clientId){
        var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerData>();
        if(client != null){
            client.serverTagged.Value = setFlag;
        }
        updateTaggedClientRpc(setFlag, new ClientRpcParams{
        Send = new ClientRpcSendParams
        {
            TargetClientIds = new ulong[] {clientId}
        }
        });
    }
    [ClientRpc]
    public void updateTaggedClientRpc(bool setFlag, ClientRpcParams clientRpcParams = default){
        if(IsOwner){
            return;
        }
        else{
            serverTagged.Value = setFlag;
            Debug.Log("Client got tagged");
        }
    }
    public void setPlayerTag(bool setTagColour){
        if(setTagColour){
            playerBody.GetComponent<MeshRenderer>().material = tagMaterial;
            vignette.volumeLayer = taggedLayer;
            if(IsLocalPlayer){
                crosshair.color = Color.red;
            }
        }
        else{
            playerBody.GetComponent<MeshRenderer>().material = untaggedMaterial;
            vignette.volumeLayer = untaggedLayer;
            if(IsLocalPlayer){
                crosshair.color = Color.white;
            }
        }
    }
}
