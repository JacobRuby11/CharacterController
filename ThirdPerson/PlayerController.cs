using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   
    public CharacterController Player;
    public Vector3 Direction = Vector3.zero;
    public float Speed;
    public float JumpSpeed;
    public float Yvel;
    public float Gravity;
    int JumpTimer = 0;
    bool jumping = false;
    public float pushPower;

/******************************/
    Vector3 startPos;
    InputMaster controls;
    Vector2 move;
    public float speed;
    public bool sprinting;
    public float sprintMult;
    public float speedBonus = 1;
    public float bonusTimer = 0;
    public bool onIce;
    void Awake() {
        controls = new InputMaster();
        // These Dont Work.//////////////////////////////////////////////////////////////////
        //controls.Player.Movement.performed += context => move = context.ReadValue<Vector2>();
        //controls.Player.Movement.canceled += context => move = Vector2.zero;
        //controls.Player.Jump.started += context => jumping = true;
        //controls.Player.Jump.canceled += context => jumping = false;
        //controls.Player.Sprint.started += context => sprinting = true;
        //controls.Player.Sprint.canceled += context => sprinting = false;
        // ///////////////////////////////////////////////////////////////////////////////////
        Player = gameObject.AddComponent<CharacterController>();
        Speed = 10f;
        JumpSpeed =  10f;
        Yvel = 0f;
        Gravity = 25f;
        startPos = Player.transform.position;

    }

    public void OnMove(InputAction.CallbackContext context){
        move = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context){
        jumping = context.ReadValueAsButton();
    }

    public void OnSprint(InputAction.CallbackContext context){
        sprinting = context.ReadValueAsButton();
    }

    private void OnEnable() {
        controls.Player.Enable();
        
    }
    private void OnDisable() {
        controls.Player.Disable();
    }

    void Update(){
        MovePlayer();
        bonusTimer -= 1;
        if (bonusTimer == 0){
            speedBonus = 1;
        }
    }

    private void MovePlayer(){
        Direction = new Vector3(move.x, 0.0f, move.y); 
        if (sprinting){
            Direction = Direction.normalized * Speed * speedBonus;
        }
        else{ 
            Direction = Direction.normalized * Speed * 0.5f;
        }
        //Direction = transform.TransformDirection(Direction);

        if (Player.isGrounded){
            // Jump Timer so multiple jumps arent recorded from one hold
            if (jumping && Time.frameCount - JumpTimer >= 15){
                Yvel = JumpSpeed;
                JumpTimer = Time.frameCount;
            }
            else{
                if(Time.frameCount - JumpTimer > 20){
                    Yvel = -5f;
                }
            }
        }
        else{
            Yvel -= Gravity * Time.deltaTime;
            if (Yvel < -50f){
                Yvel = -50f;
            }
        }
        
        Direction.y = Yvel;
        if(Player.transform.position.y < -50 || Mathf.Abs(gameObject.transform.position.z) > 30){
            Player.transform.position = startPos;
        }
        else{
            if (Player.isGrounded && gameObject.scene.name == "Road"){
                Direction.z += 2;
            }
            Player.Move(Direction * Time.deltaTime);
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        sprintMult = 0.5f;
        if (sprinting){
            sprintMult = 1f;
        }
        Rigidbody body = hit.collider.attachedRigidbody;
        if (hit.collider.tag == "dash"){
            speedBonus = 2;
            bonusTimer = 500;
            return;
        }
        if (body == null || body.isKinematic || Player.transform.position.y - hit.transform.position.y > 1f){
            return;
        }
        if (Player.transform.position.y - hit.transform.position.y < -0.4 && jumping){
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 1f, hit.moveDirection.z);
            body.velocity = pushDir * (pushPower/body.mass) * sprintMult * speedBonus;
        }
        else if(Player.transform.position.y - hit.transform.position.y < -0.4){
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.3f, hit.moveDirection.z);
            body.velocity = pushDir * (pushPower/body.mass) * sprintMult * speedBonus;
        }
        else{
            Vector3 pushDir = new Vector3(hit.moveDirection.x, /**body.velocity.y**/0f, hit.moveDirection.z);
            body.velocity = pushDir * (pushPower/body.mass) * sprintMult * speedBonus;
        }
    }
}
