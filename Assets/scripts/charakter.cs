using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Diagnostics;
using UnityEngine.UIElements;

public class charakter : MonoBehaviour
{

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private BoxCollider2D bc;
    private Animator animator;
    private GameObject CameraReflection;

    public Joystick joystick;

    public static bool isMovable;

    private float dirX;
    [SerializeField] public float jumpForce;
    //[SerializeField] public float moveSpeed;
    public float moveSpeed;

    [SerializeField] private LayerMask jumpableGround;

    private enum MovementState { idle, running, jumping, falling }

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        CameraReflection = GameObject.Find("RenderCamera");

        isMovable = true;

        moveSpeed = 8f;
        jumpForce = 14f;

    }

    void Update()
    {
        if (isMovable)
        {
            moveHorizontal();
        }
        UpdateAnimationState();
        //Jump();
        moveCamera();
    }
    
    public void LeftPointerDown()
    {
        dirX = -1;
    }
    public void LeftPointerUp()
    {
        dirX = 0;
    }
    public void RightPointerDown()
    {
        dirX = 1;
    }
    public void RightPointerUp()
    {
        dirX = 0;
    }

    public void jump()
    {
        if (isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
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

    public void jumpKeyboard(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jump();
        }
    }

    public void moveKeyboard(InputAction.CallbackContext context) //irgendwie einheitlich? -> ohne Trennung Keyboard / Touch
    {
        dirX = context.ReadValue<Vector2>().x;
    }
    private void moveCamera()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        CameraReflection.transform.position = new Vector3(transform.position.x, CameraReflection.transform.position.y, CameraReflection.transform.position.z);
    }

    private void setToPosition() { 
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(-5f, -2f, 0);
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

}
