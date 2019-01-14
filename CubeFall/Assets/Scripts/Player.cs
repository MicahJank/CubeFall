using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rigidBody;

    AudioSource jumpSound;

    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float cubeRotation = 100f;

    // Will determine if the player can rotate left or right
    enum Rotation { Left, Right, noRotation };
    Rotation rotationState;

    // Start is called before the first frame update
    void Start()
    {
        jumpSound = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody>();
        rotationState = Rotation.noRotation;     
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        Jumping();
    }

    private void Rotate()
    {
        float rotationOnFrame = cubeRotation * Time.deltaTime;

        if (Input.GetKey(KeyCode.A) && (rotationState != Rotation.Right))
        {
            rotationState = Rotation.Left;
            transform.Rotate(Vector3.forward * rotationOnFrame, Space.World);
        }
        else if (Input.GetKey(KeyCode.D) && (rotationState != Rotation.Left))
        {
            rotationState = Rotation.Right;
            transform.Rotate(Vector3.back * rotationOnFrame, Space.World);
        }
        else
        {
            rotationState = Rotation.noRotation;
        }
    }

    private void Jumping() // TODO make better Jump - longer spacebar held the higher the jump
    {
        float jumpSpeedOnFrame = _jumpForce * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidBody.velocity = new Vector3(0f, 0f, 0f);
            //rigidBody.freezeRotation;

            jumpSound.Play();
            rigidBody.AddRelativeForce(Vector3.up * jumpSpeedOnFrame, ForceMode.VelocityChange); // VelocityChange so the force ignores mass
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Friendly": Debug.Log("OK");
                break;

            default: Debug.Log("Dead"); // TODO kill player
                break;
        }
    }
}
