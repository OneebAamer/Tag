using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerMenuSettings : MonoBehaviour
{
    private int maxSens = 800;
    public PlayerData pd;
    public TMP_Text preciseSens;
    public Slider sens;
    public Camera playerCam;
    public Slider FOV;
    public TMP_Text preciseFOV;
    void Awake(){
        sens.value = PlayerPrefs.GetFloat("Sensitivity");
        FOV.value = PlayerPrefs.GetFloat("FOV");
        pd.mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity") * maxSens;
        this.gameObject.SetActive(false);
    }
    void Update(){
        preciseFOV.text = playerCam.fieldOfView.ToString();
    }
    public void changeFOV(float newFOV){
        playerCam.fieldOfView = newFOV;
        PlayerPrefs.SetFloat("FOV", newFOV);
    }
    public void changeSensitivity(float newSens){
        pd.mouseSensitivity = newSens * maxSens;
        PlayerPrefs.SetFloat("Sensitivity",newSens);
        preciseSens.text = newSens.ToString("F2");
    }
}
