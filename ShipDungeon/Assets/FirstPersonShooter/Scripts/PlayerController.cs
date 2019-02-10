//

using System.Collections;
using UnityEngine;
//testing
using UnityEngine.UI;

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(PlayerCamera))]
public class PlayerController : FPSController {

    //objects / scripts
    private CharacterMotor Motor;
    private CharacterController characterController;
    private PlayerCamera playerCam;
    public Gun gun;

    //movement
    [Header("General Movement")]
    [SerializeField]
    private float speed = 5.0f;
    [SerializeField]
    private float mouseSensitivity = 3.0f;
    [SerializeField]
    private float accelerationSpeed;
    [SerializeField]
    private float deccelerationSpeed;

    //camera
    [Space(5)]
    [Header("Camera")]
    [SerializeField]
    private float lookRange = 88.0f;
    private float _cameraRotation = 0;
    
    //jump
    [Space(5)]
    [Header("Jump")]
    [Range(0f, 1f)] 
    [SerializeField]
    private float hangTime;
    [SerializeField]
    private float jumpSpeed;
    //[SerializeField]
    //private float gravity;
    private float verticalVelocity;

    private float directionFwd;
    private float directionRight;
    private float CurrentSpeedFwd;
    private float CurrentSpeedRight;
    private bool isMovingFwd = false;
    private bool isMovingRight = false;
    private Vector3 _Velocity = new Vector3(0,0,0);

    //testing!!!!!
    private Vector3 _rotation = new Vector3(0, 0, 0);
    private float _yrot;

    //jump
    [Space(5)]
    [Header("Jump")]
    [SerializeField]
    private float boostSpeed;
    [SerializeField]
    private float airControlSpeed;
    [SerializeField]
    private float jumpBuffer;
    private bool Jumping = false;
    private bool inputRight = false;
    private bool inputLeft = false;
    private bool inputForward = false;
    private bool inputBack = false;
    private float sphereCastHeight;
    private bool isGrounded = false;
    private bool hanging = false;

    //other
    private RaycastHit hit;
    private Vector3 playerCenter;

    private bool isAlive = true;

    //for dubug visual only
    private int debugCount = 0;


    //melee prototype
    private Melee melee;
    bool canMelee = true;
    bool isMelee = false;
    public Animation meleeAnimation;
    

    public void Start()
    {
        Motor = GetComponent<CharacterMotor>();
        characterController = GetComponent<CharacterController>();
        playerCam = GetComponent<PlayerCamera>();
        sphereCastHeight = (characterController.height / 2) - 0.1f;
        try { gun = GetComponentInChildren<Gun>(); } catch { }
        try {
            melee = GetComponent<Melee>();
            meleeAnimation["MeleeSwingAnimation"].time = 0.35f;
            meleeAnimation.Play("MeleeSwingAnimation");
        } catch { }
    }

    public void Update()
    {
        //check if grounded everyframe
        playerCenter = transform.position + this.characterController.center;
        isGrounded = (Physics.SphereCast(playerCenter, characterController.radius, -transform.up, out hit, sphereCastHeight));

        PlayerRotate();

        PlayerMove();
        
        TiltCamera();

        //PlayerBoost(); // not happy, moving on for now

        PlayerJump();

        Fire();


        //Melee Prototype
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {          
            if (melee && canMelee)
            {
                meleeAnimation.Play();
                //Debug.Log("RightClick!");
                melee.attack();
                StartCoroutine(MeleeCooldown());
            }
        }


    }

    //Melee Prototype
    IEnumerator MeleeCooldown()
    {
        canMelee = false;
        yield return new WaitForSeconds(0.8f);
        canMelee = true;
    }


    //handles the raw vector3 that is passed onto the motor
    public void PlayerMove()
    {
        //get inputs as bools
        isMovingFwd = (Input.GetAxisRaw("Vertical") > 0.1 || Input.GetAxisRaw("Vertical") < -0.1);
        isMovingRight = (Input.GetAxisRaw("Horizontal") > 0.1 || Input.GetAxisRaw("Horizontal") < -0.1);

        
        float TargetSpeedFwd = 0;
        float TargetSpeedRight = 0;

        if (isGrounded)
        {
            //make each direction lerp to a desired speed, dependent on if they are accelerating or deccelerating
            TargetSpeedFwd = isMovingFwd ?
                Mathf.Lerp(_Velocity.z, (Input.GetAxisRaw("Vertical") * speed), accelerationSpeed) :
                Mathf.Lerp(_Velocity.z, 0.0f, deccelerationSpeed);

            TargetSpeedRight = isMovingRight ?
                Mathf.Lerp(_Velocity.x, (Input.GetAxisRaw("Horizontal") * speed), accelerationSpeed) :
                Mathf.Lerp(_Velocity.x, 0.0f, deccelerationSpeed);
        }
        else
        {
            //add airControlSpeed to the current input axis, and carry on momentum if no input
            if (Input.GetKey(KeyCode.W)) { TargetSpeedFwd = _Velocity.z += airControlSpeed; }
            else if (Input.GetKey(KeyCode.S)) { TargetSpeedFwd = _Velocity.z -= airControlSpeed; }
            else { TargetSpeedFwd = _Velocity.z; }

            if (Input.GetKey(KeyCode.D)) { TargetSpeedRight = _Velocity.x += airControlSpeed; }
            else if (Input.GetKey(KeyCode.A)) { TargetSpeedRight = _Velocity.x -= airControlSpeed; }
            else { TargetSpeedRight = _Velocity.x; }

            TargetSpeedFwd = Mathf.Clamp(TargetSpeedFwd, -speed, speed);
            TargetSpeedRight = Mathf.Clamp(TargetSpeedRight, -speed, speed);
        }

        _Velocity = new Vector3(TargetSpeedRight, 0.0f, TargetSpeedFwd);

        // if moving in a diagonal, reduce both speeds so speed is constant
        if (isMovingFwd && isMovingRight)
        {
            _Velocity = _Velocity * 0.95f; // TODO need to work maths out to get a correct number
            //_Velocity = new Vector3((TargetSpeedRight * 0.707f), 0.0f, (TargetSpeedFwd * 0.707f));
        }

        //rotate _Velocity around local rotation
        var playerRotY = this.transform.rotation.eulerAngles.y;
        Vector3 _Velocity2 = Quaternion.AngleAxis(playerRotY, Vector3.up) * _Velocity;

        Motor.Move(_Velocity2);
    }

    //camera tilt
    public void TiltCamera()
    {
        if(Input.GetAxisRaw("Horizontal") > 0.1) { playerCam.TiltCam(1.0f); }
        else if (Input.GetAxisRaw("Horizontal") < -0.1) { playerCam.TiltCam(-1.0f); }
        else { playerCam.TiltCam(0.0f); }
    }


    public void PlayerRotate()
    {
        //Calculate rotation as a 3D vector
        _yrot = Input.GetAxis("Mouse X") * mouseSensitivity;
        _rotation = new Vector3(0f, _yrot, 0f);

        //calculate camera rotation
        _cameraRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        _cameraRotation = Mathf.Clamp(_cameraRotation, -lookRange, lookRange);

        //apply rotations
        playerCam.Rotate(_rotation);
        playerCam.RotateCamera(_cameraRotation);
    }

    //jump function
    public void PlayerJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //TEMP REMOVE - IS JUMP NEEDED?????
            //Motor.Jump(jumpSpeed);
            //hanging = true;
            //StartCoroutine(FallTimer());
        }
    }

    //fall timer, time before fall force is applied
    IEnumerator FallTimer()
    {
        yield return new WaitForSeconds(hangTime);
        hanging = false;
        //Fall();
    }


    //fall, like jump but negative force
   private void Fall()
    {
        hanging = false;
        if (!isGrounded)
        {
            //Motor.Jump(-100.0f);
        }
    }


    private void Fire()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            if (gun.TryFire())
            {
                playerCam.Recoil();
            }
        }

    }

    public override void Kill()
    {
        base.Kill();
        playerCam.IsDead();
        //Destroy(Motor); //VERY TEMP!!!!!!!!!!!!!!
        Destroy(this);
        
    }




    //prototype boost method
    public void PlayerBoost()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift)) // change this to an input
        {
            if(inputLeft) { Motor.Boost(-boostSpeed); }
            if(inputRight) { Motor.Boost(boostSpeed); }
        }
    }

}
