using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SettingsMenu : MonoBehaviour
{
    private TagGamemode gm;
    public CameraMovement cm;
    public TMP_Text preciseTimer;
    public PlayerMenu menu;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<TagGamemode>();
    }
    void Awake(){
    }
    void Update(){
        preciseTimer.text = gm.maxGameTime.ToString();
    }
    public void updateTimer(float newTime){
        gm.maxGameTime = (int) newTime;
    }
    public void updateInfected(bool isInfected){
        gm.infected.Value = isInfected;
    }
    public void back(){
        menu.busy = false;
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cm.enabled = true;
    }
}
