//

using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMotor : MonoBehaviour {

    private Vector3 velocity = Vector3.zero;

    private Rigidbody rb;

    private void Start()
    {
        try { rb = GetComponent<Rigidbody>(); }
        catch { }
    }

    //gets a movement vector from controller script
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void FixedUpdate()
    {
        PerformMovement();
    }

    //perform movement based on velocity variable
    public void Jump(float jump)
    {
        //Vector3 jumpUp = new Vector3(0, jump, 0);
        rb.AddForce(0, jump, 0);
    }

    private void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }



    //PROTOTYPE
    public void Boost(float deltaForce)
    {
        //Debug.Log("HEYYYYYY " + deltaForce);
        rb.AddForce(deltaForce, 100, 0);
    }
}
