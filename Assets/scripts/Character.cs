using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{

    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private Animator _animator;
    private GameObject _cameraReflectionObject;
    private float _dirX;


    public Joystick Joystick;
    public static bool IsMovable;
    public float MoveSpeed;
    public float JumpForce;

    [SerializeField] private LayerMask jumpableGround;

    private enum MovementState { idle, running, jumping, falling }

    void Start()
    {

        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _cameraReflectionObject = GameObject.Find("RenderCamera");

        IsMovable = true;
    }

    void Update()
    {
        if (IsMovable)
        {
            MoveHorizontal();
        }
        UpdateAnimationState();
        //Jump();
        MoveCamera();
    }

    public void LeftPointerDown()
    {
        _dirX = -1;
    }
    public void LeftPointerUp()
    {
        _dirX = 0;
    }
    public void RightPointerDown()
    {
        _dirX = 1;
    }
    public void RightPointerUp()
    {
        _dirX = 0;
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, JumpForce);
        }
    }
    public void MoveHorizontal()
    {
        _rigidBody.velocity = new Vector2(_dirX * MoveSpeed, _rigidBody.velocity.y);
    }

    private void UpdateAnimationState()
    {
        MovementState state;
        if (_dirX > 0f)
        {
            state = MovementState.running;
            _spriteRenderer.flipX = false;
        }
        else if (_dirX < 0f)
        {
            state = MovementState.running;
            _spriteRenderer.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (_rigidBody.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (_rigidBody.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        _animator.SetInteger("state", (int)state);
    }

    public void JumpKeyboard(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Jump();
        }
    }

    public void MoveKeyboard(InputAction.CallbackContext context) //irgendwie einheitlich? -> ohne Trennung Keyboard / Touch
    {
        _dirX = context.ReadValue<Vector2>().x;
    }
    private void MoveCamera()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        _cameraReflectionObject.transform.position = new Vector3(transform.position.x, _cameraReflectionObject.transform.position.y, _cameraReflectionObject.transform.position.z);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

}
