using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;

public class Gun : MonoBehaviour
{

    public float rateOfFire; //make this shots per second??
    public float recoil;
    [Range(0f, 100.0f)]
    public float accuracy; //radial accuracy from crosshair   
    //public float handling; //gun spread outside of accuracy

    private float gunSpread = 0;
    [SerializeField]
    [Range(0.01f, 5.0f)]
    private float maxGunSpread;
    [SerializeField]
    [Range(0.01f, 0.5f)]
    private float handling; //time it takes to go back to zero spread

    private bool canFire = true;

    public float damage;

    private Vector3 screenSpaceCenter = new Vector3(0.5f, 0.5f, 0); //for cast to screen center
    public GameObject muzzle;

    private Camera cam;

    public LayerMask camMask;

    public bool isPlayersGun = false;


    //prototype firing methods
    public bool isLaserGun;
    public bool isSlugGun;
    public GameObject Bullet;
    public float SlugSpeed;


    //prototypes
    public Light flash;
    private LineDrawer lineDrawer;

    public Image leftImage;
    public Image rightImage;
    public Image topImage;
    public Image bottomImage;


    public void Start()
    {
        lineDrawer = new LineDrawer();
        cam = GetComponentInParent<Camera>();
        if(muzzle == null)
        {
            Debug.Log("MUZZLE NOT SET");
            Debug.Break();
        }
    }


    private void Update()
    {
        gunSpread = canFire 
            ? Mathf.Lerp(gunSpread, 0.0f, handling) //if not firing then lerp to 0
            : Mathf.Lerp(gunSpread, maxGunSpread, handling); // if firing lerp to max


        if (isPlayersGun)
        {
            //prototype - UI crosshair - needs to go into another class!
            bool doIt = false;
            if (doIt)
            {
                if (Input.GetKey(KeyCode.A)) { gunSpread = maxGunSpread; }
                if (Input.GetKey(KeyCode.D)) { gunSpread = maxGunSpread; }
                if (Input.GetKey(KeyCode.W)) { gunSpread = maxGunSpread; }
                if (Input.GetKey(KeyCode.S)) { gunSpread = maxGunSpread; }
            }

            Vector3 leftMove = new Vector3((-6.0f - gunSpread * 7.0f), 0, 0);
            Vector3 rightMove = new Vector3((6.0f + gunSpread * 7.0f), 0, 0);
            Vector3 topMove = new Vector3(0, (6.0f + gunSpread * 7.0f), 0);
            Vector3 bottomMove = new Vector3(0, (-6.0f - gunSpread * 7.0f), 0);

            leftImage.rectTransform.anchoredPosition = leftMove;
            rightImage.rectTransform.anchoredPosition = rightMove;
            topImage.rectTransform.anchoredPosition = topMove;
            bottomImage.rectTransform.anchoredPosition = bottomMove;
        }
        else
        {
            gunSpread = maxGunSpread;
        }
    }



    public bool TryFire()
    {
        if (canFire)
        {
            //fire (raycast)
            canFire = false;
            Fire();
            StartCoroutine(Cooldown());
            return true;
        }
        else { return false; }
    }


    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1/rateOfFire); //converts rate of fire to a wait time
        canFire = true;
    }

    //method for raycast (for now, maybe add projectile later, will need for enemy)
    public void Fire()
    {
        if (isLaserGun)
        {
            LineDrawer newLineDrawer = new LineDrawer();

            Vector3 hitEnd;
            RaycastHit hit;
            bool isHit = false;

            //prototype accuracy 
            Vector3 rayTarget = cam.transform.forward * 500.0f;

            //need to set range to increase with each shot, then go down to zero over time.
            // this would be easier if the ray came from the screen cast
            float x = Random.Range(-gunSpread, gunSpread);
            float y = Random.Range(-gunSpread, gunSpread);
            float z = Random.Range(-gunSpread, gunSpread);
            rayTarget = Quaternion.Euler(x, y, z) * rayTarget;

            //TODO calculate end point of ray based on accuracy. unless the first shot, then 100% accurate

            //need to set a good lenght based on environment
            if (Physics.Raycast(cam.transform.position, rayTarget, out hit))
            {
                hitEnd = hit.point;
                //Debug.Log("HIT: " + hit.transform.name);
                isHit = true;
            }
            else
            {
                hitEnd = cam.transform.forward * 500.0f;
                //Debug.Log("Miss");
            }

            try
            {
                //prototypes
                newLineDrawer.DrawLineInGameView(muzzle.transform.position, hitEnd, Color.red);
                StartCoroutine(destoryLine(newLineDrawer));
                flash.enabled = true;
                StartCoroutine(LightOff());
            }
            catch { }


            //did we hit an actor or nothing?
            if (isHit)
            {
                //check if hit object has damage handler class, and call damage method         
                try
                {
                    Transform actor = hit.transform;
                    DamageHandler damageHandler = actor.GetComponent<DamageHandler>();
                    damageHandler.Damage(damage, hit.point);
                }
                catch
                {

                    //here you can put particle spawning, like a bullet hole or something

                    //Debug.Log("oops!"); 
                }
            }
        }
        else if(isSlugGun)
        {
            //spawn projectile
            GameObject TempBulletHandler;
            TempBulletHandler = Instantiate(Bullet, muzzle.transform.position, muzzle.transform.rotation) as GameObject;

            Rigidbody tempRb;
            tempRb = TempBulletHandler.GetComponent<Rigidbody>();

            tempRb.AddForce(transform.forward * SlugSpeed);

            Destroy(TempBulletHandler, 10.0f);
            
        }
    }

    IEnumerator destoryLine(LineDrawer lineIn)
    {
        yield return new WaitForSeconds(0.05f);
        lineIn.Destroy();
    }




    //prototype
    IEnumerator LightOff()
    {
        yield return new WaitForSeconds(0.03f);
        flash.enabled = false;
    }








    //prototype line drawing for debug
    public struct LineDrawer
    {
        private LineRenderer lineRenderer;
        private float lineSize;

        public LineDrawer(float lineSize = 0.1f)
        {
            GameObject lineObj = new GameObject("LineObj");
            lineRenderer = lineObj.AddComponent<LineRenderer>();
            //Particles/Additive
            lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

            this.lineSize = lineSize;
        }

        private void init(float lineSize = 0.1f)
        {
            if (lineRenderer == null)
            {
                GameObject lineObj = new GameObject("LineObj");
                lineRenderer = lineObj.AddComponent<LineRenderer>();
                //Particles/Additive
                lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

                this.lineSize = lineSize;
            }
        }

        //Draws lines through the provided vertices
        public void DrawLineInGameView(Vector3 start, Vector3 end, Color color)
        {
            if (lineRenderer == null)
            {
                init(0.2f);
            }

            //Set color
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            //Set width
            lineRenderer.startWidth = lineSize;
            lineRenderer.endWidth = lineSize;

            //Set line count which is 2
            lineRenderer.positionCount = 2;

            //Set the postion of both two lines
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        public void Destroy()
        {
            if (lineRenderer != null)
            {
                UnityEngine.Object.Destroy(lineRenderer.gameObject);
            }
        }
    }





}
