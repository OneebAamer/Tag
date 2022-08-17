using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Sliding : NetworkBehaviour
{
    public Transform orientation;
    public Transform player;
    private Rigidbody rb;
    private PlayerMovement pm;

    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    public float slideYScale;
    private float startYScale;

    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    public bool sliding;
    //Crouch
    private CharacterController controller;
    private Camera playerCam;
    public float crouchHeight = 1.5f;
    public float crouchTime = 0.25f;
    public float standHeight = 2f;
    private Vector3 crouchCentre = new Vector3(0,0.25f,0);
    private Vector3 standCentre = new Vector3(0,0,0);
    public bool isCrouching;
    private bool duringCrouchAnim = false;
    private bool shouldCrouch => Input.GetKeyDown(KeyCode.LeftControl) && !duringCrouchAnim;

    public bool isStuck = false;
    private bool bufferSlide = false;
    // private bool cooldown = false;
    // private float cooldownTime = 0.25f;

    void Start(){
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();
        playerCam = GetComponentInChildren<Camera>();
        startYScale = player.localScale.y;
    }
    void Update(){
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (isStuck){
            pm.isSprinting = false;
            if(!Physics.Raycast(playerCam.transform.position, Vector3.up, 0.5f)){
                StartCoroutine(setPlayerStandHeight());
                isStuck = false;
                isCrouching = false;
            }
        }
        if(bufferSlide &&pm.isGrounded){
            bufferSlide = false;
            if(Input.GetKey(slideKey) && pm.isSprinting && (horizontalInput != 0 || verticalInput != 0)){
                isCrouching = false;
                startSlide();
            }
        }
        if(Input.GetKeyDown(slideKey) && pm.isSprinting && pm.isGrounded && (horizontalInput != 0 || verticalInput != 0)){
            startSlide();
        }
        else if(Input.GetKeyDown(slideKey)){
            if(!pm.isGrounded){
                bufferSlide = true;
            }
            isCrouching = true;
            if(!duringCrouchAnim){
                StartCoroutine(setPlayerCrouchHeight());
            }
            else{
                StopCoroutine(setPlayerStandHeight());
                StartCoroutine(setPlayerCrouchHeight());
            }
        }
        if(Input.GetKeyUp(slideKey) && sliding){
            stopSlide();
        }
        else if(Input.GetKeyUp(slideKey)){
            if(!duringCrouchAnim){
                StartCoroutine(setPlayerStandHeight());
            }
            else{
                StopCoroutine(setPlayerCrouchHeight());
                StartCoroutine(setPlayerStandHeight());
            }
            isCrouching = false;
        }
    }
    private void FixedUpdate(){
        if(sliding){
            slidingMovement();
        }
    }
    private void startSlide(){
        sliding = true;
        StartCoroutine(setPlayerCrouchHeight());
        slideTimer = maxSlideTime;
    }
    private void slidingMovement(){
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if(!pm.isSlopeSliding){
            // rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
            controller.Move(inputDirection * slideForce * Time.deltaTime *slideTimer);
            slideTimer -= Time.deltaTime;
            if(slideTimer <= 0){
                stopSlide();
            }
        }
        else{
            // Vector3 slopeSlide = Vector3.ProjectOnPlane(inputDirection, pm.hitPointNormal).normalized;   
            rb.AddForce(pm.getSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
            // controller.Move(slopeSlide * Time.deltaTime);
        }
    
    }
    private void stopSlide(){
        sliding = false;
        StartCoroutine(setPlayerStandHeight());
    }
    private IEnumerator setPlayerCrouchHeight(){
        duringCrouchAnim = true;
        float currentHeight = controller.height;
        float timeElapsed = 0f;
        Vector3 currentCentre = controller.center;
        while (timeElapsed<crouchTime){
            controller.height = Mathf.Lerp(currentHeight, crouchHeight, timeElapsed/crouchTime);
            controller.center = Vector3.Lerp(currentCentre, crouchCentre, timeElapsed/crouchTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        controller.height = crouchHeight;
        controller.center = crouchCentre;
        duringCrouchAnim = false;
    }
    private IEnumerator setPlayerStandHeight(){
        if(IsLocalPlayer){
            duringCrouchAnim = true;
            float currentHeight = controller.height;
            float timeElapsed = 0f;
            Vector3 currentCentre = controller.center;
            while (timeElapsed<crouchTime){
                if (Physics.Raycast(playerCam.transform.position, Vector3.up, 0.5f)){
                    if(!isCrouching){
                        sliding = false;
                    }
                    isStuck = true;
                    isCrouching = true;
                    setPlayerCrouchHeight();
                    yield break;
                }
                controller.height = Mathf.Lerp(currentHeight, standHeight, timeElapsed/crouchTime);
                controller.center = Vector3.Lerp(currentCentre, standCentre, timeElapsed/crouchTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            rb.velocity = Vector3.zero;
            controller.height = standHeight;
            controller.center = standCentre;
            duringCrouchAnim = false;
        }
    }
}
