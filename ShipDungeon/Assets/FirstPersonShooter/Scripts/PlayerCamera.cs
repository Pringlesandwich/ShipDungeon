//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    private Gun gun;

    private Rigidbody rb;

    private Vector3 rotation = Vector3.zero;

    [SerializeField]
    private float recoilAmount;
    [SerializeField]
    private float recoil;
    [SerializeField]
    private float recoilReturn;

    [SerializeField]
    private float InteractLenght;

    private float cameraRotationX = 0;
    private float cameraRotationY = 0;
    private float cameraRotationZ = 0;

    private float cameraRotationXDelta = 0;
    private float cameraRotationYDelta = 0; // rotate camera left/right without player, used for recoil
    private float cameraRotationZDelta = 0;

    private float camXDestination = 0;
    private float camYDestination = 0;
    private float camZDestination = 0;

    private bool isRecoiling = false;

    //private Vector3 cameraPos;
    private Quaternion cameraRot;

    public Text UIText;
    public Text Prompt;
    public FPSGameManager gm;

    // Use this for initialization
    void Start () {
        //cameraPos = cam.transform.position;
        rb = GetComponent<Rigidbody>();
        //cam = GetComponent<Camera>();
        cameraRot = cam.transform.rotation;
        try { gun = GetComponentInChildren<Gun>(); } catch { }
    }
	
	// Update is called once per frame
	void Update ()
    {
        CameraRotate();
        Interact();
        PerformRotation();
    }

    private void Interact()
    {
        //cast ray at interactlenght to llok for objects to interact with
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, InteractLenght))
        {
            if (hit.transform.gameObject.tag == "Door")
            {
                // Debug.Log("DOOOOOOOR");
                Prompt.text = "Press 'E'";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameObject door = hit.transform.root.gameObject;
                    Animation[] anime = door.GetComponentsInChildren<Animation>();
                    foreach (var a in anime)
                    {
                        a.Play();
                    }
                }
            }
            else if (hit.transform.gameObject.tag == "Treasure")
            {
                Prompt.text = "Press 'E'";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameObject Treasure = hit.transform.root.gameObject;
                    Destroy(Treasure);
                    gm.Run();
                }
            }
            else
            {
                Prompt.text = "";
            }
        }
        else
        {
            Prompt.text = "";
        }
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
        cameraRotationY = cameraRotationYDelta; // not plus eqaul or will twist cam around player
        cameraRotationZ += cameraRotationZDelta;
        
        cam.transform.localRotation = Quaternion.Euler(cameraRotationX, cameraRotationY, cameraRotationZ);      
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

        float lerpTimeX;
        lerpTimeX = isRecoiling ? recoil : recoilReturn;
        float lerpTimeY = isRecoiling ? recoil : 0.03f;

        cameraRotationXDelta = Mathf.Lerp(cameraRotationXDelta, camXDestination, lerpTimeX);
        cameraRotationYDelta = Mathf.Lerp(cameraRotationYDelta, camYDestination, lerpTimeY);
        //Debug.Log(cameraRotationXDelta);
    }




    public void Recoil()
    {
        StartCoroutine(RecoilTimer());
    }

    IEnumerator RecoilTimer()
    {
        //Debug.Log("Recoil");
        camXDestination = -gun.recoil; // get recoil from gun

        //gu accuracy tied to cam not gun
        float acc = ((-0.05f * gun.accuracy) + 5.0f); //gun accuracy: 100 = 0, 50 = 2.5, 0 = 5
        camYDestination = UnityEngine.Random.Range(-acc, acc);

        isRecoiling = true;
        //yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(1/(gun.rateOfFire * 2)); // make sure recoil time works with current rate of fire
        ResetRecoil();
    }

    //fall, like jump but negative force
    private void ResetRecoil()
    {
        //Debug.Log("DONE");
        camXDestination = 0;
        camYDestination = 0;
        isRecoiling = false;
    }

    //prototype
    public void IsDead()
    {
        UIText.enabled = true;
    }


    //method for adding camera up when holding down trigger, resets on trigger off?






}
