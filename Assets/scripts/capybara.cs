using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class capybara : MonoBehaviour
{
    private float _maxVelocity;
    private float _acceleration;
    private float _velocity;
    private float _posX;
    private float diffPos;

    private GameObject _capybara;
    private GameObject _character;
    void Start()
    {
        _maxVelocity = 0.11f;
        _posX = 0;
        _velocity = 0;
    }

    private void Awake()
    {
        _capybara = GameObject.Find("capybara");
        _character = GameObject.Find("Character");
    }

    private void FixedUpdate()
    {
        diffPos = _character.transform.position.x - _capybara.transform.position.x;
        _acceleration = diffPos;
        _velocity += _acceleration / 30f;
        if (Mathf.Abs(_velocity) > _maxVelocity)
        {
            _velocity /= Mathf.Abs(_velocity / _maxVelocity);
        }
        _posX += _velocity;
    }
    void Update()
    {
        _capybara.transform.position = new Vector3(_posX, -4.91f, 0);
        if(diffPos > 4f)
        {
            _maxVelocity = 0.17f;
        } else
        {
            _maxVelocity = 0.11f;
        }

        if(_velocity > 0)
        {
            _capybara.transform.eulerAngles = new Vector3(0, 180, 0);
        } else
        {
            _capybara.transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
}
