using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public KeyCode JumpKeyCode = KeyCode.Space;
    public float JumpStrength = 1;
    public CharacterController controller;

    public float speed = 12f;

    //public Transform groundCheck;
   // public float groundDistance = 0.4f;
    //public LayerMask groundMask;

    private Vector3 physicsVelocity;

    Vector3 velocity;
    bool isGrounded;

    // Start is called before the first frame update
    void Start() { }


    // Update is called once per frame
    void Update() {

        if (controller.isGrounded && physicsVelocity.y < 0) {
            physicsVelocity.y = 0f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 userInputMovement = transform.right * x + transform.forward * z;

            //physicsVelocity.y = 0;
            if (Input.GetKeyDown(JumpKeyCode) && controller.isGrounded) {
                physicsVelocity.y += JumpStrength;
            }

            //apply gravity
            physicsVelocity.y += Physics.gravity.y * Time.fixedDeltaTime;

        controller.Move(
            ((userInputMovement * speed)  + //User input movement
            physicsVelocity)  //Physics movement
            * Time.deltaTime); //scale by deltatime for framerate reasons

        //controller.Move(velocity * Time.deltaTime);
    }
}