using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using System;
using UnityEngine;
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
        if (isMovable)
        {
            moveHorizontal();
        }
        UpdateAnimationState();
        Jump();
        moveCamera();
        if(!isGrounded())
        {
            
            New = rb.position.y;
            float deltaTime = Time.time - time;
            //UnityEngine.Debug.Log(rb.velocity.y + " time: " + deltaTime);
            //UnityEngine.Debug.Log(rb.position.x + " time: " + deltaTime);
            old = New;
            //UnityEngine.Debug.Log();

        }
    }

    private void moveHorizontal()
    {
        dirX = Input.GetAxisRaw("Horizontal");
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

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded())
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
