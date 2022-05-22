using System.Collections;


using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public Animator animator;
    public float speed = 6;
    public float gravity = -9.81f;
    public float jumpHeight = 3;
    Vector3 velocity;
    public bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    private int airJump = 1;

    // Update is called once per frame
    void Update()
    {
        //jump
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            airJump = 1;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jump.");
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        if (Input.GetButtonDown("Jump") && !isGrounded) {
            if(airJump > 0){
                airJump--;
                Debug.Log("AirJump.");
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }
            
        }
        //gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        //walk
        float horizontal = Input.GetAxisRaw("Horizontal");
        /*float vertical = Input.GetAxisRaw("Vertical");*/
        Vector3 direction = new Vector3(horizontal, 0f, 0f/*vertical*/).normalized;

        if(direction.magnitude >= 0.1f)
        {
            animator.SetBool("isMoving", true);
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        else{
            animator.SetBool("isMoving", false);
        }
    }

    public void Respawn(){
        Debug.Log("Respawn");
        Debug.Log(transform.position);
        controller.enabled = false;
        transform.position = new Vector3(0f,10f,0f);
        Debug.Log(transform.position);
        controller.enabled = true;
        velocity = Vector3.zero;
    }
}