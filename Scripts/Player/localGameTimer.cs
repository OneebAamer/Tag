using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
public class localGameTimer : NetworkBehaviour
{
    public TextMeshProUGUI gameTimerText;
    private TagGamemode gameManager;
    public TMP_Text displayLoser;
    public TMP_Text waitForHostText;
    void Start(){
        gameManager = GameObject.Find("GameManager").GetComponent<TagGamemode>();
    }
    // Update is called once per frame
    void Update()
    {
        if(IsLocalPlayer){
            if(gameManager.hasStarted.Value){
                gameTimerText.text =  gameManager.gameTimer.Value.ToString();
                waitForHostText.text = "";
            }
            else{
                if(gameManager.finished.Value == true){
                    gameTimerText.text = "";
                }
                if(IsHost){
                    waitForHostText.text = "Press Enter to start the game.";
                }
                else{
                    waitForHostText.text = "Waiting for host to start the game.";
                }
            }
        }
    }
}
