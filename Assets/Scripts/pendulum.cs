using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Pendulum : Physic
{
    private GameObject _levelGenerator;
    private LevelGenerator _levelGeneratorScript;

    public CircleCollider2D _circleCollider;
    public SpriteRenderer spriteRenderer;


    //material

    public int PendulumId;

    public Vector2 PendulumStart;
    public Vector2 PendulumEnd;
    public Vector2 PendulumPosCenter;
    public Vector2 PendulumPosCenterBall;
    private Vector2 PendulumOriginBallStart;

    public Rigidbody2D RigidBodyScript;
    public Character CharacterScriptScript;

    private GameObject LineToOrigin;
    private LineRenderer _lineToOrigin;


    private float _angleMax;
    public float _radiusBall;
    public float _lengthString; //from Origin to OriginBall
    private float _mass;

    private float acceleration;
    private float velocity;
    private float _accelerationConstant;
    private float _currentAngle;
    private const float STARTANGLE_PROPORTION_QUARTER_CIRCLE = 0.25f;
    private const float MAX_FACTOR_RADIUS_BALL = 1f;
    private const float MIN_FACTOR_RADIUS_BALL = 0.5f;
    private const float TOP_LIMIT_LENGTH_PENDULUM = 12f;
    private const float BOTTOM_LIMIT_LENGTH_PENDULUM = 4f;
    private const float MAGNITUDE_RANDOM_MAGNITUDE_LENGTH_STRING = 1f;
    private const float MINIMUM_RANDOM_MAGNITUDE_LENGTH_STRING = 2f;
    private const float MAX_RANDOM_MAGNITUDE_LENGTH_STRING = MAGNITUDE_RANDOM_MAGNITUDE_LENGTH_STRING + MINIMUM_RANDOM_MAGNITUDE_LENGTH_STRING;

    private const float WIND_FORCE_AMPLITUDE = 70f;
    private const float SCALE_NOISE_POSX = 0.03f;
    private const float SCALE_NOISE_TIME = 0.0003f;
    public Texture PendulumTexture;

    public Physic _physic;

    private void Start()
    {
        _levelGenerator = GameObject.Find("LevelGenerator");
        _levelGeneratorScript = _levelGenerator.GetComponent<LevelGenerator>();


        //material
    }

    public void GenerateSectionPendulum()
    {
        SetupPendulumComponents();
        SetupPendulumCoordinates();
        SetupPendulumLine();
        CalcPendulumConstants();
    }

    public void UpdatePendulum()
    {
        float tick = 0.01f;
        gameObject.transform.position = new Vector3(PendulumPosCenterBall.x, PendulumPosCenterBall.y, 0);

        acceleration = -_accelerationConstant * _currentAngle; //a = F/m
        velocity += acceleration * tick;

        float deltaLength = velocity * tick;

        float deltaX = deltaLength * (float)Math.Cos(_currentAngle);
        float deltaY = deltaLength * (float)Math.Sin(_currentAngle);

        Vector3 movePendulum = new Vector3(deltaX, deltaY, 0);
        PendulumPosCenterBall += new Vector2(movePendulum.x, movePendulum.y + 0.00004f);
        spriteRenderer.transform.position += movePendulum;

        _currentAngle = -(float)Math.Atan((PendulumPosCenter.x - PendulumPosCenterBall.x) / (PendulumPosCenter.y - PendulumPosCenterBall.y));

        _lineToOrigin.SetPosition(0, new Vector3(PendulumPosCenterBall.x, PendulumPosCenterBall.y, 0));
        _lineToOrigin.SetPosition(1, new Vector3(PendulumPosCenter.x, PendulumPosCenter.y, 0));

        //Debug.Log(_velocity);
        if (Math.Abs(velocity) < 0.1f && _currentAngle < 0)
        {
            PendulumPosCenterBall = PendulumOriginBallStart;
        }
    }

    public void UpdatePendulum2()
    {
        gameObject.transform.position = new Vector3(PendulumPosCenterBall.x, PendulumPosCenterBall.y, 0);

        _currentAngle = GetCurrentAngle();
        float forceWind = 2f * WIND_FORCE_AMPLITUDE * (Mathf.PerlinNoise(SCALE_NOISE_POSX * PendulumPosCenterBall.x, SCALE_NOISE_TIME * LevelGenerator.Time) - 0.5f);

        _mass = 10;
        float alpha = -_currentAngle;

        float beta = Mathf.PI / 2 - alpha;
        float forceGrav = Character.gravityScale * _mass;
        float tick = 0.01f;

        float forceNormal = (-Mathf.Tan(alpha) * forceWind + forceGrav) / (Mathf.Sin(beta) + Mathf.Tan(alpha) * Mathf.Cos(beta));

        float forceX = forceWind + forceNormal * Mathf.Cos(beta);
        float forceY = -forceGrav + forceNormal * Mathf.Sin(beta);

        float force = (float)Math.Sqrt(forceX * forceX + forceY * forceY);
        if (_currentAngle <= -Math.PI / 2)
        {
            force *= -1;
        }
        if (forceX < 0)
        {
            force *= -1;
        }

        float acceleration = (force) / _mass;


        velocity += acceleration * tick;

        float deltaLength = velocity * tick;

        float deltaX = deltaLength * (float)Math.Cos(_currentAngle);
        float deltaY = deltaLength * (float)Math.Sin(_currentAngle);


        Vector3 movePendulum = new Vector3(deltaX, deltaY, 0);
        PendulumPosCenterBall += new Vector2(movePendulum.x, movePendulum.y + 0.00004f);
        spriteRenderer.transform.position += movePendulum;

        _currentAngle = GetCurrentAngle();

        _lineToOrigin.SetPosition(0, new Vector3(PendulumPosCenterBall.x, PendulumPosCenterBall.y, 0));
        _lineToOrigin.SetPosition(1, new Vector3(PendulumPosCenter.x, PendulumPosCenter.y, 0));

        Vector2 distanceOrigin = PendulumPosCenter - PendulumPosCenterBall;
        Vector2 PosCenterOnCircle = new Vector2(_lengthString * Mathf.Cos(_currentAngle + 1.5f * (float)Math.PI), _lengthString * Mathf.Sin(_currentAngle + 1.5f * (float)Math.PI));
        Vector2 VectorError = distanceOrigin + PosCenterOnCircle;
        PendulumPosCenterBall += VectorError;

    }

    public void DestroyPendulum()
    {
        Destroy(this.gameObject);
    }


    public void SetupPendulumComponents()
    {
        _levelGenerator = GameObject.Find("LevelGenerator");
        _levelGeneratorScript = _levelGenerator.GetComponent<LevelGenerator>();

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        _circleCollider = gameObject.AddComponent<CircleCollider2D>();
        _circleCollider.radius = 2;

        spriteRenderer.sprite = _levelGeneratorScript.SpritePendulum;
        _radiusBall = _levelGeneratorScript.SpritePendulum.texture.width * 0.01f / 2;
    }

    public void SetupPendulumCoordinates()
    {
        SetLengthString();
        float slopeFunctionAdjustRadius = 
            (MAX_FACTOR_RADIUS_BALL - MIN_FACTOR_RADIUS_BALL) / 
            (TOP_LIMIT_LENGTH_PENDULUM - BOTTOM_LIMIT_LENGTH_PENDULUM);
        float interceptFunctionAdjustRadius = MAX_FACTOR_RADIUS_BALL - TOP_LIMIT_LENGTH_PENDULUM * slopeFunctionAdjustRadius;
        float factorAdjustRadius = slopeFunctionAdjustRadius * _lengthString + interceptFunctionAdjustRadius;
        //_circleCollider.radius *= factorAdjustRadius;
        _radiusBall *= factorAdjustRadius;
        spriteRenderer.transform.localScale = new Vector3(factorAdjustRadius, factorAdjustRadius, 1f);

        float jumpX = Mathf.Abs(Mathf.Sin(UnityEngine.Random.value * Mathf.PI)) * 2f + 1f;
        float jumpY = -RigidBodyScript.gravityScale * 9.81f * Mathf.Pow(jumpX, 2) * 0.5f * Mathf.Pow(1 / CharacterScriptScript.MoveSpeed, 2) + CharacterScriptScript.JumpForce * jumpX / CharacterScriptScript.MoveSpeed - 0.9f - 2 * _radiusBall;


        PendulumPosCenterBall = new Vector2(PendulumStart.x + jumpX, PendulumStart.y + jumpY - _radiusBall);
        float startAngle = UnityEngine.Random.value * 0.5f * (Mathf.PI / 2 * (STARTANGLE_PROPORTION_QUARTER_CIRCLE)) + (Mathf.PI / 2 * STARTANGLE_PROPORTION_QUARTER_CIRCLE);

        /*PendulumPosCenter = new Vector2(
            PendulumPosCenterBall.x + Mathf.Abs(Mathf.Sin(UnityEngine.Random.value * Mathf.PI)) * 1f + 3f,
            PendulumPosCenterBall.y + Mathf.Abs(Mathf.Sin(UnityEngine.Random.value * Mathf.PI)) * 2f + 3f);*/

        PendulumPosCenter = new Vector2(
            PendulumPosCenterBall.x + _lengthString * Mathf.Cos(startAngle),
            PendulumPosCenterBall.y + _lengthString * Mathf.Sin(startAngle));

        PendulumOriginBallStart = PendulumPosCenterBall;
    }

    public void SetupPendulumLine()
    {
        LineToOrigin = new GameObject("OriginLine");
        LineToOrigin.tag = "lineToOrigin";
        _lineToOrigin = LineToOrigin.AddComponent<LineRenderer>();
        _lineToOrigin.SetPosition(0, new Vector3(PendulumPosCenterBall.x, PendulumPosCenterBall.y, 0));
        _lineToOrigin.SetPosition(1, new Vector3(PendulumPosCenter.x, PendulumPosCenter.y, 0));
        _lineToOrigin.widthMultiplier = 0.2f;
        //_lengthString = (PendulumPosCenterBall - PendulumPosCenter).magnitude;
    }

    public void CalcPendulumConstants()
    {
        _angleMax = Mathf.Asin((PendulumPosCenter.x - PendulumPosCenterBall.x) / (_lengthString));
        _currentAngle = _angleMax;

        float lengthLevel = 2f * _lengthString * (float)Math.Sin(_angleMax) + _radiusBall;
        PendulumEnd = new Vector2(PendulumStart.x + lengthLevel + 5f, PendulumStart.y);
        _accelerationConstant = Mathf.Abs(Mathf.Sin(UnityEngine.Random.value * Mathf.PI)) * 20f + 35f;


    }

    private float GetCurrentAngle()
    {
        float _currentAngle = -(float)Math.Atan((PendulumPosCenter.x - PendulumPosCenterBall.x) / (PendulumPosCenter.y - PendulumPosCenterBall.y));
        if (PendulumPosCenterBall.y > PendulumPosCenter.y)
        {
            return _currentAngle - (float)Math.PI;
        }
        return _currentAngle;
    }
    private void SetLengthString()
    {
        if (PendulumId == 0)
        {
            float length = Mathf.Abs(Mathf.Sin(UnityEngine.Random.value * Mathf.PI)) * 2f + 3f;
            _lengthString = length;
            return;
        }
        float lengthStringPendulumBefore = (_physic._pendulumList[PendulumId - 1].GetComponent<Pendulum>())._lengthString;
        float randomSign = (UnityEngine.Random.value * 2) - 1;
        if (lengthStringPendulumBefore > TOP_LIMIT_LENGTH_PENDULUM - MAX_RANDOM_MAGNITUDE_LENGTH_STRING) //private const float TOP_LIMIT_LENGTH_PENDULUM
        {
            randomSign = -1;
        }
        if (lengthStringPendulumBefore < BOTTOM_LIMIT_LENGTH_PENDULUM + MAX_RANDOM_MAGNITUDE_LENGTH_STRING) //private const float BOTTOM_LIMIT_LENGTH_PENDULUM
        {
            randomSign = 1;
        }
        float randomMagnitude = UnityEngine.Random.value * MAGNITUDE_RANDOM_MAGNITUDE_LENGTH_STRING + MINIMUM_RANDOM_MAGNITUDE_LENGTH_STRING; //[2...3] 
        float newLength = lengthStringPendulumBefore + randomSign * randomMagnitude; //[4...12]
        _lengthString = newLength;
        return;
    }
}
//private const float TOP_LIMIT_LENGTH_PENDULUM = 12f;
//private const float BOTTOM_LIMIT_LENGTH_PENDULUM = 4f;
//private const float MAGNITUDE_RANDOM_MAGNITUDE_LENGTH_STRING = 1f;
//private const float MINIMUM_RANDOM_MAGNITUDE_LENGTH_STRING = 2f;
//private const float MAX_RANDOM_MAGNITUDE_LENGTH_STRING = MAGNITUDE_RANDOM_MAGNITUDE_LENGTH_STRING + MINIMUM_RANDOM_MAGNITUDE_LENGTH_STRING;
