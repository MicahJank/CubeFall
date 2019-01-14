using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rigidBody;


    [SerializeField] private float _jumpForce = 10f;

    // Will determine if the player can rotate left or right
    enum Rotation { Left, Right, noRotation };
    Rotation rotationState;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rotationState = Rotation.noRotation;
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerInput();
    }

    private void CheckPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * _jumpForce, ForceMode.VelocityChange); // VelocityChange so the force ignores mass
        }

        if (Input.GetKey(KeyCode.A) && (rotationState != Rotation.Right))
        {
            rotationState = Rotation.Left;
            transform.Rotate(Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.D) && (rotationState != Rotation.Left))
        {
            rotationState = Rotation.Right;
            transform.Rotate(Vector3.back);
        }
        else
        {
            rotationState = Rotation.noRotation;
           
        }
       


    }
}
