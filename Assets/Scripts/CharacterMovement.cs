using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    private CharacterController character;
    private InputManager input;
    private GameManager manager;
    [SerializeField] private float walkSpeed = 5, runSpeed = 10, jumpForce = 2;
    private float gravityValue;
    private Vector3 playerVelocity;
    [SerializeField] private float fallMultiplier = 2.5f, lowJumpMultiplier = 2.0f;
    private Vector3 head,feet, velocity;
    [SerializeField] private LayerMask rayCollision;
    [SerializeField] private Transform cam;
    [SerializeField] private float smoothCamMovement = 3,smoothCamRotation;
    private float mouseX,mouseY;
    private void Awake() {
        character = GetComponent<CharacterController>();
        input = InputManager.inst;
        manager = GameManager.inst;
    }
    // Start is called before the first frame update
    void Start()
    {
        gravityValue = Physics.gravity.y;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Get Top and Bottom Position
        head = transform.position;
        head.y += transform.localScale.y;
        feet = transform.position;
        feet.y -= transform.localScale.y;

        Jump();
        Movement();
    }
    private void LateUpdate() {
        Cam();
    }
    private void FixedUpdate() {
        //Applying Forces
        character.Move(transform.TransformVector(playerVelocity) * Time.fixedDeltaTime);
        Gravity();
    }
    private void Gravity(){
        //Stop Falling
        if(isGrounded() && playerVelocity.y < 0) playerVelocity.y = 0f;
        //Stop jumpForce when hitting something
        else if(hitHead() && playerVelocity.y > 0) playerVelocity.y = 0f;
        //Increasing Gravity
        else playerVelocity.y += gravityValue * Time.fixedDeltaTime;
        
        //Better Jump Ref: https://www.youtube.com/watch?v=7KiK0Aqtmzc&pp=ugMICgJwdBABGAE%3D
        if(playerVelocity.y < 0f){
            playerVelocity.y += gravityValue * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }else if(playerVelocity.y > 0f && !input.JumpPressed()){
            playerVelocity.y += gravityValue * (lowJumpMultiplier -1) * Time.fixedDeltaTime;
        }
    }
    private void Movement(){
        //Right and Left Movement
        playerVelocity.x = input.MovementInput().x * walkSpeed;
        //Forward and Back Movement
        if(input.RunInput() && playerVelocity.z > 0){
            playerVelocity.z = input.MovementInput().z * runSpeed;
        }
        else playerVelocity.z = input.MovementInput().z * walkSpeed;
    }
    private void Jump(){
        if(input.JumpInput() && isGrounded()){
            playerVelocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravityValue);
        }
    }
    private void Cam(){
        //Smooth Camera Follow POV
        Vector3 pov = transform.GetChild(0).position;
        Vector3 smoothPosition = Vector3.SmoothDamp(cam.position,pov,ref velocity,smoothCamMovement * Time.deltaTime);
        cam.position = smoothPosition;
        
        //Smooth Camera Rotation
        mouseX += input.MouseDeltaInput().x * manager.mouseSensiblity * Time.deltaTime;
        mouseY += input.MouseDeltaInput().y * manager.mouseSensiblity * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY,-90,90);
        Quaternion desiredRotation = Quaternion.Euler(-mouseY,mouseX,0);
        Quaternion smoothRotation = Quaternion.Slerp(cam.rotation, desiredRotation ,smoothCamRotation * Time.deltaTime);
        cam.rotation = smoothRotation;
        
        Quaternion camRotation = transform.rotation;
        camRotation.y = cam.rotation.y;
        camRotation.w = cam.rotation.w;
        transform.rotation = camRotation;

    }
    private bool hitHead(){
        return Physics.Raycast(head,transform.up,0.1f,rayCollision);
    }
    private bool isGrounded(){
        return Physics.Raycast(feet,-transform.up,0.1f,rayCollision);
    }
}
