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
    private float gravityValue,jumpingSpeed;
    private Vector3 playerVelocity;
    [SerializeField] private float fallMultiplier = 2.5f, lowJumpMultiplier = 2.0f;
    private Vector3 head,feet, velocity;
    [SerializeField] private LayerMask rayCollision;
    public Transform cam;
    [SerializeField] private float smoothCamMovement = 3,smoothCamRotation;
    private float mouseX,mouseY;
    private bool runningjump = false;
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

        jumpingSpeed = runSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //Get Top and Bottom Position
        head = transform.position;
        head.y += transform.localScale.y - 0.4f;
        feet = transform.position;
        feet.y -= transform.localScale.y - 0.4f;

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
        if(input.RunInput() && playerVelocity.z > 0 && isGrounded()){
            playerVelocity.z = input.MovementInput().z * runSpeed;
        }else if(playerVelocity.z > 0 && runningjump){
            playerVelocity.z = input.MovementInput().z * jumpingSpeed;
            jumpingSpeed -= 0.5f * Time.fixedDeltaTime;
        }
        else playerVelocity.z = input.MovementInput().z * walkSpeed;
    }
    private void Jump(){
        if(input.JumpInput() && isGrounded()){
            playerVelocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravityValue);
        }
        if((input.RunInput() && playerVelocity.z > 0 && isGrounded() && (input.JumpInput() || input.JumpPressed()))){
            runningjump = true;
        }
        else if(isGrounded()){
            runningjump = false;
            jumpingSpeed = runSpeed;
        }
    }
    private void Cam(){
        //Smooth Camera Follow POV
        Vector3 pov = transform.GetChild(0).position;
        Vector3 smoothPosition = Vector3.SmoothDamp(cam.position,pov,ref velocity,smoothCamMovement * Time.deltaTime);
        cam.position = smoothPosition;
        
        //Smooth Camera Rotation
        mouseX += input.MouseDeltaInput().x * manager.mouseSensiblity * 0.5f *  Time.fixedDeltaTime;
        mouseY += input.MouseDeltaInput().y * manager.mouseSensiblity * 0.5f * Time.fixedDeltaTime;
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
        return Physics.CheckSphere(head,0.5f,rayCollision);
    }
    private bool isGrounded(){
        return Physics.CheckSphere(feet,0.5f,rayCollision);
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(feet,0.5f);
    }
}
