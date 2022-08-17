using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
public class TagGamemode : NetworkBehaviour
{
    public Vector3 gameSpawn;
    public Vector3 lobbySpawn;
    [SerializeField] private PlayersManager pm;
    public int maxGameTime = 60;
    public NetworkVariable<int> gameTimer = new NetworkVariable<int>();
    public NetworkVariable<int> pregameTimer = new NetworkVariable<int>();
    public NetworkVariable<bool> inPregame = new NetworkVariable<bool>();
    public NetworkVariable<bool> hasStarted = new NetworkVariable<bool>();
    public NetworkVariable<bool> finished = new NetworkVariable<bool>();
    public GameObject joinCodeText;
    public GameObject[] players;
    public NetworkVariable<bool> infected = new NetworkVariable<bool>();
    public NetworkVariable<int> tagged = new NetworkVariable<int>();
    public NetworkVariable<NetworkString> loser = new NetworkVariable<NetworkString>();
    public NetworkVariable<int> survivors = new NetworkVariable<int>();
    public TMP_Text scoreboard;
    // Start is called before the first frame update
    void Start()
    {
        NetworkObject.DontDestroyWithOwner = true;
    }
    // Update is called once per frame
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if(Input.GetKeyDown(KeyCode.Return) && IsOwner && !hasStarted.Value && !inPregame.Value && getSurvivorCount() > 1){
            startPregameServerRpc();
        }
        if(hasStarted.Value){
            if(!infected.Value){
                foreach(GameObject player in players){
                    PlayerData pd = player.GetComponent<PlayerData>();
                    if(pd.isTagged){
                        loser.Value = pd.username;
                    }
                }
            }
            else{
                if(hasStarted.Value){
                    if(IsHost){
                        survivors.Value = getSurvivorCount();
                        if(survivors.Value == 0){
                            endGameServerRpc();
                        }
                    }
                }
            }
        }
        else{
            string scores = "Scoreboard \n";
            foreach(GameObject player in players){
                PlayerData pd = player.GetComponent<PlayerData>();
                scores += $"{pd.username}: {pd.points.Value} \n";
            }
            scoreboard.text = scores;
        }
    }
    //Transfer tag status from player 1 to 2
    public void transferPlayerTagged(PlayerData player1, PlayerData player2){
        NetworkObject player2Network = player2.gameObject.GetComponent<NetworkObject>();
        player1.swapTaggedServerRpc(true, player2Network.OwnerClientId);
        if(!infected.Value){
            player1.updateTaggedServerRpc(false);
        }
        else{
            survivors.Value--;
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
        survivors.Value = getSurvivorCount();
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
    public void startPregameServerRpc(){
        pregameTimer.Value = 10;
        teleportPlayersClientRpc(gameSpawn);
        StartCoroutine(pregameTimerStart());
    }
    IEnumerator pregameTimerStart(){
        setPregameServerRpc(true);
        while(pregameTimer.Value > 0){
            yield return new WaitForSeconds(1);
            decrementPregameTimeServerRpc();
        }
        startGameServerRpc();
        setPregameServerRpc(false);
    }
    [ServerRpc]
    private void setPregameServerRpc(bool flag){
        inPregame.Value = flag;
    }
    [ServerRpc]
    private void decrementGameTimeServerRpc(){
        gameTimer.Value--;
    }
    [ServerRpc]
    private void decrementPregameTimeServerRpc(){
        pregameTimer.Value--;
    }
    [ServerRpc]
    public void endGameServerRpc(){
        StopAllCoroutines();
        foreach(GameObject player in players){
            PlayerData pd = player.GetComponent<PlayerData>();
            if(pd.isTagged){
                pd.points.Value++;
                pd.serverTagged.Value = false;
            }
        }
        teleportPlayersClientRpc(lobbySpawn);
        joinCodeText.SetActive(true);
        hasStarted.Value = false;
        finished.Value = true;
    }
    [ClientRpc]
    public void teleportPlayersClientRpc(Vector3 pos){
        StartCoroutine(enableMovement(false));
        foreach(GameObject player in players){
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.enabled = false;
            playerMovement.updateClientPositionServerRpc(pos.x, pos.y, pos.z);
            player.transform.position = pos;
        }
        StartCoroutine(enableMovement(true));
    }
    IEnumerator enableMovement(bool flag){
        yield return new WaitForSeconds(1);
        foreach(GameObject player in players){
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.enabled = flag;
        }
    }
    public int getTaggedCount(){
        int taggedCount = 0;
        foreach(GameObject player in players){
            PlayerData pd = player.GetComponent<PlayerData>();
            if(pd.isTagged){
                taggedCount++;
            }
        }
        return taggedCount;
    }
    public int getSurvivorCount(){
        int survivorCount = 0;
        foreach(GameObject player in players){
            PlayerData pd = player.GetComponent<PlayerData>();
            if(!pd.isTagged){
                survivorCount++;
            }
        }
        return survivorCount;
    }
}
