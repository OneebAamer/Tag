using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class CameraMovement : NetworkBehaviour
{
    private float sensitivity;
    public Transform playerBody;
    public PlayerMovement pm;
    float xRotation = 0f;

    void Start() {
        if(IsLocalPlayer){
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(IsLocalPlayer){
            sensitivity = this.gameObject.GetComponentInParent<PlayerData>().mouseSensitivity;
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation,-90f,90f);
            transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
            playerBody.Rotate(Vector3.up * mouseX);
            updateClientRotationServerRpc(playerBody.rotation);
        }
    }
    [ServerRpc]
    void updateClientRotationServerRpc(Quaternion locRotation){
        pm.serverRotation.Value = locRotation;
    }
}
