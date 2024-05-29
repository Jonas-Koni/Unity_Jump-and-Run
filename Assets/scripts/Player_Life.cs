using UnityEngine;
using UnityEngine.SceneManagement;

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
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    private void Swim() //später auslagern
    {
        _character.GetComponent<Character>().IsMovable = false;
        _animator.SetInteger("state", 0);
        _rigidbody.gravityScale = 0;
        _rigidbody.velocity = Vector3.zero;

    }
}
