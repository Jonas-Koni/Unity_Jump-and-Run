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
    private const float JUMP_PRESSED_REMEMBER_TIME = 0.15f;
    private const float NOT_GROUNDED_REMEMBER_TIME = 0.05f;


    public float JumpPressedRemember;
    public float NotGroundedRemember;
    public Joystick Joystick;
    public bool IsMovable;
    public float MoveSpeed = 7;
    public float JumpForce = 14f;

    public static float gravityScale = 3;

    [SerializeField] private LayerMask jumpableGround;

    private enum MovementState { idle, running, jumping, falling }

    void Start()
    {

        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _cameraReflectionObject = GameObject.Find("RenderCamera");
        gravityScale = _rigidBody.gravityScale * 9.81f;

        IsMovable = true;
    }

    void Update()
    {

        if (IsMovable)
        {
            MoveHorizontal();
        }
        UpdateAnimationState();
        MoveCamera();

        if(JumpPressedRemember >= -1) 
        {
            JumpPressedRemember -= Time.deltaTime;
        }
        NotGroundedRemember -= Time.deltaTime;


        if (IsGrounded())
        {
            NotGroundedRemember = NOT_GROUNDED_REMEMBER_TIME;
        }

        if (NotGroundedRemember > 0 && JumpPressedRemember > 0)
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, JumpForce);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "sticky")
        {
            transform.parent = collision.transform;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "sticky")
        {
            transform.parent = null;
        }
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
        JumpPressedRemember = JUMP_PRESSED_REMEMBER_TIME;
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
        else if (_rigidBody.velocity.y < -0.2f && NotGroundedRemember < 0)
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
