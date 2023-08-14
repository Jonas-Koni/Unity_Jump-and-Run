using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Diagnostics;
using UnityEngine.UIElements;

public class charakter : MonoBehaviour
{

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private BoxCollider2D bc;
    private Animator animator;
    private GameObject CameraReflection;

    private float old;
    private float New;
    private float time;

    public Joystick joystick;

    public static bool isMovable;

    private float dirX;
    [SerializeField] public float jumpForce;
    [SerializeField] public float moveSpeed;

    [SerializeField] private LayerMask jumpableGround;



    private enum MovementState { idle, running, jumping, falling }

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        CameraReflection = GameObject.Find("RenderCamera");

        isMovable = true;

        moveSpeed = 7f;
        jumpForce = 14f;

    }

    void Update()
    {
        if(joystick.Horizontal > .2f)
        {
            dirX = 1;
        } else if(joystick.Horizontal < -.2f)
        {
            dirX = -1;
        } else { dirX = 0; }
        if(joystick.Vertical > .4f && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            time = Time.time;
        }

        if (isMovable)
        {
            moveHorizontal();
        }
        UpdateAnimationState();
        //Jump();
        moveCamera();
        if(!isGrounded())
        {
            New = rb.position.y;
            float deltaTime = Time.time - time;
            old = New;

        }
    }
    public void changeDirX(InputAction.CallbackContext context)
    {
        dirX = context.ReadValue<Vector2>().x;
    }
    public void moveHorizontal()
    {
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
    }

    private void UpdateAnimationState()
    {
        MovementState state;
        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        animator.SetInteger("state", (int)state);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        //if (Input.GetButtonDown("Jump") && isGrounded())
        if (isGrounded() && context.started)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            time = Time.time;
            
        }
    }
    private void moveCamera()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        CameraReflection.transform.position = new Vector3(transform.position.x, CameraReflection.transform.position.y, CameraReflection.transform.position.z);
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

}
