using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : FPSController {

    public float lookRadius = 10; //trigger enemy for now, change this to 'room manager' script
    public float speed;
    public float maxDistance;
    public float minDistance;

    public bool isAgro = false;

    private bool isAlive = true;

    private bool canSeePlayer = false;

    private bool canFire = true;
    private bool willFire = true;
    public float rateOfFire;
    public float reactionTime;
    private float willFireCooldown;

    [SerializeField]
    private float stunLockChance;
    [SerializeField]
    private float stunTime;
    private float newStunTime;

    private GameObject[] HidingSpots;

    Transform target;
    NavMeshAgent agent;

    public Gun gun;
    public Camera cam;

    private Color emmisionColor;
    private Renderer renderer;

    // Use this for initialization
    void Start() {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        cam = GetComponentInChildren<Camera>();
        gun = GetComponentInChildren<Gun>();
        agent.speed = speed;
        HidingSpots = GameObject.FindGameObjectsWithTag("HidingSpot");
        renderer = GetComponentInChildren<Renderer>();
    }


    // Update is called once per frame
    void FixedUpdate() {

        //get distance to player
        float distance = Vector3.Distance(target.position, transform.position);

        //Debug.Log(distance);

        //has been activated
        if (isAgro && isAlive)
        {
            //get within range
            if (distance > maxDistance)
            {
                //Debug.Log("MOVE IN!");
                agent.SetDestination(target.position);
            }
            //if too close run away
            else if (distance < minDistance)
            {
                //move away
                Vector3 destination = this.transform.position - target.transform.position;

                Vector3 newPos = transform.position + destination;
                agent.SetDestination(newPos);
                //distance + direction

                //FindHidingSpot();
            }
            //between sweet spot
            else if(distance > minDistance && distance < maxDistance)
            {
                agent.SetDestination(this.transform.position);
            }
        }

        //agro player
        if (distance <= lookRadius)
        {
            if (agent != null)
            {
                isAgro = true;
            }
        }

        //set emmision, reset stun
        if (isAgro)
        {
            emmisionColor = Color.Lerp(emmisionColor, Color.black, Time.deltaTime * 10);
            renderer.material.SetColor("_EmissionColor", emmisionColor);

            newStunTime -= Time.deltaTime;
            if(newStunTime < 0)
            {
                canFire = true;
            }

        }

        willFireCooldown -= Time.deltaTime;
        if(willFireCooldown <= 0)
        {
            willFire = true;
        }
        if(!canSeePlayer)
        {
            willFireCooldown = reactionTime;
            willFire = false;
        }
        Fire();
    }


    //prototype
    private void FindHidingSpot()
    {
        foreach(var i in HidingSpots)
        {
            //shoot ray beetween i and player, if hidden then select that one.



        }
    }



    //face player
    public void LateUpdate()
    {
        //turn to face player
        if(isAgro && isAlive)
        {
            // transform.LookAt(target);
            var lookPos = target.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 100);
        }
    }


    public void Fire()
    {
        if (canFire)
        {
            Vector3 rayTarget = cam.transform.forward * 500.0f;
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, rayTarget, out hit))
            {
                //if can see player and alive
                if (isAlive && hit.collider.gameObject.CompareTag("Player")) // i dont want to use tags but good for now
                {
                    canSeePlayer = true;
                    if (willFire)
                    {
                        gun.TryFire();
                        willFireCooldown = UnityEngine.Random.Range(rateOfFire - 0.5f, rateOfFire + 0.5f);
                        willFire = false;
                    }
                }
            }
        } 
    }



    public void Hit()
    {
        //change emmision color, makes enemy flash white
        emmisionColor = Color.white;

        //stun
        float deltaStun = UnityEngine.Random.Range(0.0f, 100.0f);
        if(deltaStun <= stunLockChance)
        {
            newStunTime = stunTime;
            canFire = false;
        }
    }

   
    //prototype
    public override void Kill()
    {
        base.Kill();
        try
        {
            isAlive = false;
            var rb = GetComponent<Rigidbody>();
            var agent = GetComponent<NavMeshAgent>();
            Destroy(agent);
            rb.isKinematic = false;
            rb.AddForce(Vector3.forward * 400.0f);
        }
        catch { }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }


}
