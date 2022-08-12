using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerMovement : NetworkBehaviour
{
     private CharacterController controller;
     public Camera playerCam;
     public GameObject playerBody;

    private float speed;
    public float gravity = -50;
    public float jumpHeight = 4f;
    //Gravity
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundmask;
    //Sliding
    private bool willSlideOnSlope = true;
    private float slopeSpeed = 0.2f;
    public Vector3 hitPointNormal;
    private Sliding slideManager;
    public bool inMenu = false;
    public bool isSlopeSliding{
        get{
            if(isGrounded && Physics.Raycast(transform.position,Vector3.down,out RaycastHit slopeHit, 2f)){
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal,Vector3.up) >= controller.slopeLimit;
            }
            else{
                return false;
            }
        }
    }

    Vector3 velocity;
    Vector3 slopeVelocity;
    public bool isGrounded;
    public NetworkVariable<Vector3> serverPosition = new NetworkVariable<Vector3>();
    void Start(){
        if(!IsLocalPlayer){
            playerCam.gameObject.SetActive(false);
        }
        else{
            playerCam.fieldOfView = PlayerPrefs.GetFloat("FOV");
            playerBody.SetActive(false);
            controller = GetComponent<CharacterController>();
            controller.gameObject.tag = "Untagged";
            speed = GetComponent<PlayerData>().playerSpeed;
            slideManager = GetComponent<Sliding>();
        }
    }

    private void Update(){
        speed = GetComponent<PlayerData>().playerSpeed;
        UpdateClient();
    }

    // Update is called once per frame
    void UpdateClient()
    {
        if(IsLocalPlayer){
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundmask) || slideManager.sliding;
            if(isGrounded && velocity.y < 0)
            {
                velocity = new Vector3(0f,-5f,0f);
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            if(!isGrounded){
                x= x/2;
            }
            Vector3 move = new Vector3();
            if(!inMenu){
                move = (transform.right * x + transform.forward * z);
                move = Vector3.ClampMagnitude(move, 1f);
            }
            if ((slideManager.isCrouching || slideManager.isStuck) && isGrounded){
                controller.Move(move * speed * .5f * Time.deltaTime);
            }
            else if(Input.GetKey(KeyCode.LeftShift)){
                controller.Move(move * speed * 1.5f * Time.deltaTime);
            }
            else{
                controller.Move(move * speed * Time.deltaTime);
            }
            if(Input.GetButtonDown("Jump") && isGrounded && !isSlopeSliding && !inMenu)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); 
            }
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            if(willSlideOnSlope && isSlopeSliding){
                slopeVelocity += new Vector3(hitPointNormal.x, -hitPointNormal.y,hitPointNormal.z) * slopeSpeed;
            }
            else{
                slopeVelocity = Vector3.zero;
            }
            controller.Move(slopeVelocity * Time.deltaTime);
            updateClientPositionServerRpc(transform.position.x, transform.position.y, transform.position.z);
        }
        else{
            transform.position = serverPosition.Value;
        }
    }
    public Vector3 getSlopeMoveDirection(Vector3 direction){
        return Vector3.ProjectOnPlane(direction, hitPointNormal).normalized;
    }
    [ServerRpc]
    public void updateClientPositionServerRpc(float x, float y, float z){
        serverPosition.Value = new Vector3(x,y,z);
    }
}