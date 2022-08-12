using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
public class TagGamemode : NetworkBehaviour
{
    [SerializeField] private PlayersManager pm;
    public int maxGameTime = 60;
    public NetworkVariable<int> gameTimer = new NetworkVariable<int>();
    public NetworkVariable<bool> hasStarted = new NetworkVariable<bool>();
    public NetworkVariable<bool> finished = new NetworkVariable<bool>();
    public GameObject joinCodeText;
    public GameObject[] players;
    public bool infected = false;
    public NetworkVariable<NetworkString> loser = new NetworkVariable<NetworkString>();
    // Start is called before the first frame update
    void Start()
    {
        NetworkObject.DontDestroyWithOwner = true;
    }
    // Update is called once per frame
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if(Input.GetKeyDown(KeyCode.Return) && IsOwner && !hasStarted.Value && pm.playersInGame > 1){
            startGameServerRpc();
        }
    }
    //Transfer tag status from player 1 to 2
    public void transferPlayerTagged(PlayerData player1, PlayerData player2){
        NetworkObject player2Network = player2.gameObject.GetComponent<NetworkObject>();
        player1.swapTaggedServerRpc(true, player2Network.OwnerClientId);
        if(!infected){
            player1.updateTaggedServerRpc(false);
        }
    }
    [ServerRpc]
    public void startGameServerRpc(){
        loser.Value = "";
        gameTimer.Value = maxGameTime;
        //hide join code
        joinCodeText.SetActive(false);
        hasStarted.Value = true;
        //Start a timer
        StartCoroutine(gameTimerStart());
        //Set tagged player(s)
        int randPlayerNum = Random.Range(0,players.Length);
        string randPlayer = players[randPlayerNum].name;
        PlayerData taggedPlayer = GameObject.Find(randPlayer).GetComponent<PlayerData>();
        taggedPlayer.serverTagged.Value = true;
        Debug.Log("Tagged set: " + randPlayer);
    }
    IEnumerator gameTimerStart(){
        while(gameTimer.Value > 0){
            yield return new WaitForSeconds(1);
            decrementGameTimeServerRpc();
        }
        //End game
        endGameServerRpc();
    }
    [ServerRpc]
    private void decrementGameTimeServerRpc(){
        gameTimer.Value--;
    }
    [ServerRpc]
    public void endGameServerRpc(){
        joinCodeText.SetActive(true);
        hasStarted.Value = false;
        finished.Value = true;
        foreach(GameObject player in players){
            PlayerData pd = player.GetComponent<PlayerData>();
            if(pd.isTagged){
                pd.serverTagged.Value = false;
                loser.Value = pd.username;
                break;
            }
        }
    }
}
