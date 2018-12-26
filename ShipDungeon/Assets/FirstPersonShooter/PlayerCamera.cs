//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    private Rigidbody rb;

    private Vector3 rotation = Vector3.zero;

    [SerializeField]
    private float recoilAmount;
    [SerializeField]
    private float recoil;
    [SerializeField]
    private float recoilReturn;

    private float cameraRotationX = 0;
    private float cameraRotationZ = 0;

    private float cameraRotationXDelta = 0;
    private float cameraRotationZDelta = 0;

    private float camXDestination = 0;
    private float camZDestination = 0;

    private bool isRecoiling = false;

    //private Vector3 cameraPos;
    private Quaternion cameraRot;

    // Use this for initialization
    void Start () {
        //cameraPos = cam.transform.position;
        rb = GetComponent<Rigidbody>();
        //cam = GetComponent<Camera>();
        cameraRot = cam.transform.rotation;
    }
	
	// Update is called once per frame
	void Update ()
    {
        CameraRotate();
	}


    private void FixedUpdate()
    {
        PerformRotation();
    }

    //gets a movement vector from controller script
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    //gets a movement vector from controller script
    public void RotateCamera(float _cameraRotation)
    {
        cameraRotationX = _cameraRotation;
    }

    //perform movement based on float
    private void PerformRotation()
    {
        //rotate rb dependent on rotation
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        cameraRotationX += cameraRotationXDelta;
        cameraRotationZ += cameraRotationZDelta;

        cam.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, cameraRotationZ);      
    }

    //cam tilt
    public void TiltCam(float deltaAngle)
    {
        float angle = Mathf.MoveTowardsAngle(cameraRotationZ, deltaAngle, 0.05f);
        cameraRotationZ = Mathf.Clamp(angle, -1.5f, 1.5f);
    }



    private void CameraRotate()
    {
        //update all delta methods in hear, they are added to the rotation before transform takes them

        //cameraRotationXDelta

        float lerpTime;

        lerpTime = isRecoiling ? recoil : recoilReturn;

        cameraRotationXDelta = Mathf.Lerp(cameraRotationXDelta, camXDestination, lerpTime);

        Debug.Log(cameraRotationXDelta);

    }




    public void Recoil()
    {
        StartCoroutine(RecoilTimer());
    }

    IEnumerator RecoilTimer()
    {
        //Debug.Log("Recoil");
        camXDestination = -recoilAmount;
        isRecoiling = true;
        yield return new WaitForSeconds(0.1f);     
        ResetRecoil();
    }

    //fall, like jump but negative force
    private void ResetRecoil()
    {
        Debug.Log("DONE");
        camXDestination = 0;
        isRecoiling = false;
    }

}
