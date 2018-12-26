/////////////////////////////////////////////////////////
//
//  FirstPersonInput.cs
//
//  input methods and player reaction
//
//
//
/////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    //movement
    public float movementSpeed;
    public float mouseSensitivity;
    private float forwardSpeed;
    private float sideSpeed;

    //camera
    private float verticalRotation = 0;
    public float upDownRange = 60.0f;

    //jumping
    public float jumpSpeed;
    private float verticalVelocity;

    //player references
    private CharacterController characterController;
    public Camera playerCam;
    

    public void Start()
    {
        //Screen.lockCursor = true;
        characterController = GetComponent<CharacterController>();
       
        //get camera automatically
        //playerCam = GetComponent<Camera>();
    }

    public void Update()
    {
        // interaction
       // InputInteract();

        // manage input for moving
        InputMove();
        InputJump();

        // manage input for mouse look
        InputLook();

        // manage input for weapons
       // InputAttack();

    }

    //interaction
    private void InputInteract()
    {
        
    }

    //move
    private void InputMove()
    {
        forwardSpeed = Input.GetAxis("Vertical") * movementSpeed;
        sideSpeed = Input.GetAxis("Horizontal") * movementSpeed;

        Vector3 speed = new Vector3(sideSpeed, verticalVelocity, forwardSpeed);

        ////speed = transform.rotation * speed;

        //////float damp = 0.2f;

        //////speed.z /= (speed.z  Time.deltaTime);
        //////speed.x /= (speed.x * Time.deltaTime);

        ////characterController.Move(speed * Time.deltaTime);

        //float m_MotorAirSpeedModifier = 1.0f;

        //Vector3 m_MotorThrottle = new Vector3(0,0,0);
        ////Vector3 xVector = new Vector3(0, 0, 0);
        ////Vector3 zVector = new Vector3(0, 0, 0);




        // --- apply forces ---
        Vector3 m_MoveDirection = new Vector3(sideSpeed, verticalVelocity, forwardSpeed); ;
        //m_MoveDirection += m_MotorThrottle;


        // --- move the charactercontroller ---

  
        // move on our own
        characterController.Move(vp_MathUtility.NaNSafeVector3(m_MoveDirection * Time.deltaTime * Time.timeScale));




    }

    //handles interaction
    private void InputAttack()
    {
       
    }

    //jump
    private void InputJump()
    {
        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded && Input.GetButton("Jump"))
        {
            Debug.Log("Jump");
            verticalVelocity = jumpSpeed;
        }
    }

    //mouse look
    private void InputLook()
    {
        float rotLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, rotLeftRight, 0);


        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);

        playerCam.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }


    
}
