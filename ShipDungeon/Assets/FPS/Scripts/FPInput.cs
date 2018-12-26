/////////////////////////////////////////////////////////
//
//  FPInput.cs
//
//  Physic methods and physic management controller
//
//
//
/////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPInput : MonoBehaviour {

    //need reference to player


    public void Update()
    {
        // interaction
        InputInteract();

        // manage input for moving
        InputMove();
        InputJump();

        // manage input for weapons
        InputAttack();

    }

    //handles interaction
    private void InputAttack()
    {
        throw new NotImplementedException();
    }


    private void InputJump()
    {
        throw new NotImplementedException();
    }


    private void InputMove()
    {
        throw new NotImplementedException();
    }


    private void InputInteract()
    {
        throw new NotImplementedException();
    }
}
