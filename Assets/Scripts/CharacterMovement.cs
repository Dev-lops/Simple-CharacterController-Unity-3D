using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private Vector3 head,feet;
    [SerializeField] private LayerMask rayCollision;
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

        CameraRotation();
        Jump();
        Movement();
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
    private void CameraRotation(){
        Quaternion cameraRotation =  transform.rotation;
        cameraRotation.y = Camera.main.transform.rotation.y;
        cameraRotation.w = Camera.main.transform.rotation.w;
        transform.rotation = cameraRotation;

    }
    private bool hitHead(){
        return Physics.Raycast(head,transform.up,0.1f,rayCollision);
    }
    private bool isGrounded(){
        return Physics.Raycast(feet,-transform.up,0.1f,rayCollision);
    }
}
