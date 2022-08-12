using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
/// <summary>
/// Lets players set their name and synchronizes it to others.
/// </summary>
public class PlayerNameStore : NetworkBehaviour
{
    public TMP_InputField enterUsername;
    void Start(){
        enterUsername.text = PlayerPrefs.GetString("Name");
    }
    void Update(){
        PlayerPrefs.SetString("Name", enterUsername.text);
    }
}