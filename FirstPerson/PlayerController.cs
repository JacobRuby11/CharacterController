using UnityEngine;

/******************
First Person Player Controller

ADD:
    Maybe lock movement controls while airborn, or dampen them
    Maybe lock sprint as well while airborn or while moving sideways

******************/
public class PlayerController : MonoBehaviour
{
    public CharacterController Player;

    public Vector3 Direction = Vector3.zero;

    public float Speed;

    public float SprintMult;

    public float JumpSpeed;

    public float Yvel;

    public float Gravity;

    int JumpTimer = 0;

    public Camera PlayerCam;

    public float Sens;
    
    public float SlopeLimit;

    public float SlopeScale;

    float Yrot;

    float Xrot;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Player = gameObject.GetComponent<CharacterController>();
        SprintMult = 1f;
        Speed = 10f;
        JumpSpeed =  10f;
        Yvel = 0f;
        Gravity = 25f;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        RotatePlayer();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Bounce"){
            JumpSpeed = 50f;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Bounce"){
            JumpSpeed = 10f;
        }
    }

    void MovePlayer(){
        
        if(Input.GetKey(KeyCode.LeftShift)){
            SprintMult = 1.5f;
        }
        else{
            SprintMult = 1.0f;
        }
        
        Direction = new Vector3(Input.GetAxisRaw("Horizontal") * 0.8f, 0.0f, Input.GetAxisRaw("Vertical")); 
        Direction = Direction.normalized * Speed;
        Direction = new Vector3(Direction.x, Direction.y, Direction.z * SprintMult);
        Direction = transform.TransformDirection(Direction);


        // Ground logic and jump logic
        if (Player.isGrounded){
            // Jump Timer so multiple jumps arent recorded from one hold
            if (Input.GetKey(KeyCode.Space) && Time.frameCount - JumpTimer >= 15){
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
        SlopeCheck();

        Player.Move(Direction * Time.deltaTime);
    }

    //Needs Fixing
    void RotatePlayer(){
        Xrot += Input.GetAxis("Mouse X") * Sens;
        Yrot += Input.GetAxis("Mouse Y") * Sens;
        Yrot = Mathf.Clamp(Yrot, -60f, 60f);
        PlayerCam.transform.localRotation = Quaternion.Euler(-Yrot,0f,0f);
        Player.transform.rotation = Quaternion.Euler(0f,Xrot,0f);
    }

    public void SlopeCheck(){
        
        RaycastHit hitInfoForward;
        Physics.Raycast(new Ray(Player.transform.position + Vector3.down + (Vector3.forward * Player.radius), Vector3.down),  out hitInfoForward, 0.5f);


        RaycastHit hitInfoBackward;
        Physics.Raycast(new Ray(Player.transform.position + Vector3.down + (Vector3.back * Player.radius), Vector3.down),  out hitInfoBackward, 0.5f);


        RaycastHit hitInfoLeft;
        Physics.Raycast(new Ray(Player.transform.position + Vector3.down + (Vector3.left * Player.radius), Vector3.down),  out hitInfoLeft, 0.5f);


        RaycastHit hitInfoRight;
        Physics.Raycast(new Ray(Player.transform.position + Vector3.down + (Vector3.right * Player.radius), Vector3.down),  out hitInfoRight, 0.5f);
        
        /**
        //Turn on for raycast lines

        Debug.DrawRay(Player.transform.position + Vector3.down + (Vector3.forward * Player.radius), Vector3.down, Color.blue);
        Debug.DrawRay(hitInfoForward.point, hitInfoForward.normal, Color.red);
        Debug.DrawRay(hitInfoForward.point, new Vector3(hitInfoForward.normal.x, Direction.y, hitInfoForward.normal.z), Color.green);
        Debug.DrawRay(Player.transform.position + Vector3.down + (Vector3.back * Player.radius), Vector3.down, Color.blue);
        Debug.DrawRay(hitInfoBackward.point, hitInfoBackward.normal, Color.red);
        Debug.DrawRay(hitInfoBackward.point, new Vector3(hitInfoBackward.normal.x, Direction.y, hitInfoBackward.normal.z), Color.green);
        Debug.DrawRay(Player.transform.position + Vector3.down + (Vector3.left* Player.radius), Vector3.down, Color.blue);
        Debug.DrawRay(hitInfoLeft.point, hitInfoLeft.normal, Color.red);
        Debug.DrawRay(hitInfoLeft.point, new Vector3(hitInfoLeft.normal.x, Direction.y, hitInfoLeft.normal.z), Color.green);
        Debug.DrawRay(Player.transform.position + Vector3.down + (Vector3.right * Player.radius), Vector3.down, Color.blue);
        Debug.DrawRay(hitInfoRight.point, hitInfoRight.normal, Color.red);
        Debug.DrawRay(hitInfoRight.point, new Vector3(hitInfoRight.normal.x, Direction.y, hitInfoRight.normal.z), Color.green);
        **/

        if(Vector3.Angle(hitInfoForward.normal, Vector3.up) > SlopeLimit){
            Direction = new Vector3(hitInfoForward.normal.x * SlopeScale, Direction.y, hitInfoForward.normal.z * SlopeScale);
            return;
        }
        if(Vector3.Angle(hitInfoBackward.normal, Vector3.up) > SlopeLimit){
            Direction = new Vector3(hitInfoBackward.normal.x * SlopeScale, Direction.y, hitInfoBackward.normal.z * SlopeScale);
            return;
        }
        if(Vector3.Angle(hitInfoLeft.normal, Vector3.up) > SlopeLimit){
            Direction = new Vector3(hitInfoLeft.normal.x * SlopeScale, Direction.y, hitInfoLeft.normal.z * SlopeScale);
            return;
        }
        if(Vector3.Angle(hitInfoRight.normal, Vector3.up) > SlopeLimit){
            Direction = new Vector3(hitInfoRight.normal.x * SlopeScale, Direction.y, hitInfoRight.normal.z * SlopeScale);
            return;
        }
    }
}
