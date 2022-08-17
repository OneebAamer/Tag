using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
public class PlayerOutput : NetworkBehaviour
{
    private TagGamemode gm;
    public TMP_Text output;
    // Start is called before the first frame update
    void Start()
    {
        gm =GameObject.Find("GameManager").GetComponent<TagGamemode>();
    }
    void Update(){
        UpdateOutputServerRpc();
    }
    [ServerRpc]
    // Update is called once per frame
    private void UpdateOutputServerRpc()
    {
        if(IsLocalPlayer){
            if(gm.hasStarted.Value){
                if(gm.infected.Value){
                    output.text = $"{gm.survivors.Value} Survivor(s) Left!";
                }
                else{
                    output.text = $"{gm.loser.Value} is Tagged!";
                }
            }
            if(gm.inPregame.Value){
                Debug.Log(gm.pregameTimer.Value);
                output.text = $"Starting in {gm.pregameTimer.Value}";
            }
            else{
                if(gm.loser.Value.ToString().Length > 0){
                        output.text = $"Game ended! Loser was {gm.loser.Value}";
                }
            }
        }
    }
}
