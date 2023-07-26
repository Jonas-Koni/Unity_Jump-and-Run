using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Life : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rigidbody;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("water"))
        {
            //Die();
            Swim();
        }
    }


    private void Die()
    {
        animator.SetTrigger("death");
    }
    private void Swim() //später auslagern
    {
        charakter.isMovable = false;
        animator.SetInteger("state", 0);
        rigidbody.gravityScale = 0;
        rigidbody.velocity = Vector3.zero;
        
    }
}
