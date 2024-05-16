using UnityEngine;

public class Player_Life : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rigidbody;

    private GameObject _character;
    private GameObject _levelGenerator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        _character = GameObject.Find("Character");
        _levelGenerator = GameObject.Find("LevelGenerator");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("water"))
        {
            Die();
            //Swim();
        }
    }

    private void Die()
    {
        _character.transform.position = new Vector3(-5f, -2f, 0);
        _character.GetComponent<Character>().MoveSpeed = 7f; //fester Wert?
        _character.GetComponent<Character>().NotGroundedRemember = 0;
        _character.GetComponent<Character>().JumpPressedRemember = 0;
        _levelGenerator.GetComponent<LevelGenerator>().DeadPlayer();

        //animator.SetTrigger("death");
    }
    private void Swim() //später auslagern
    {
        Character.IsMovable = false;
        _animator.SetInteger("state", 0);
        _rigidbody.gravityScale = 0;
        _rigidbody.velocity = Vector3.zero;

    }
}
