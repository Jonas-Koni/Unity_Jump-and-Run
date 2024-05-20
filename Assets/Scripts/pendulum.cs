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
    public int PendulumId;
    public Physic Physic;

    private GameObject levelGenerator;
    private LevelGenerator levelGeneratorScript;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private Character _character;

    private Vector2 _pendulumStart;
    private Vector2 _pendulumEnd;
    private Vector2 _pendulumPosCenter;
    private Vector2 _pendulumPosCenterBall;

    private GameObject _lineToOriginGameObject;
    private LineRenderer _lineToOriginLineRenderer;

    private float _angleMax;
    private float _radiusBall;
    private float _lengthString; //from Origin to OriginBall

    private float _velocityWeightPendulum;
    private float _angleWeightPendulum;

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
    private const float MASS_WEIGHT_PENDULUM = 10;

    private void Start()
    {
        levelGenerator = GameObject.Find("LevelGenerator");
        levelGeneratorScript = levelGenerator.GetComponent<LevelGenerator>();
        _character = GameObject.Find("Character").GetComponent<Character>();
        //Debug.Log(_character);
    }

    private void Awake()
    {
        levelGenerator = GameObject.Find("LevelGenerator");
        levelGeneratorScript = levelGenerator.GetComponent<LevelGenerator>();
        _character = GameObject.Find("Character").GetComponent<Character>();
    }

    public void GenerateSectionPendulum()
    {
        SetupPendulumComponents();
        SetLengthString();
        SetRadiusWeight();
        SetPendulumCoordinates();
        SetupPendulumLine();
        CalcPendulumConstants();
    }

    public void SetupPendulumComponents()
    {
        levelGenerator = GameObject.Find("LevelGenerator");
        levelGeneratorScript = levelGenerator.GetComponent<LevelGenerator>();

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = 2;

        spriteRenderer.sprite = levelGeneratorScript.SpritePendulum;
        _radiusBall = levelGeneratorScript.SpritePendulum.texture.width * 0.01f / 2;

        if(PendulumId == 0)
        {
            _pendulumStart = Physic.PosStart;
        } else 
        {
            _pendulumStart = Physic.PendulumList[PendulumId - 1].GetComponent<Pendulum>()._pendulumEnd;
        }
    }

    private void SetLengthString()
    {
        if (PendulumId == 0)
        {
            _lengthString = RandomConstantSpreadNumber.GetRandomNumber(3, 5);
            return;
        }

        float lengthStringPendulumBefore = (Physic.PendulumList[PendulumId - 1].GetComponent<Pendulum>())._lengthString;
        int randomSign = (int) RandomPolynomialSpreadNumber.GetRandomNumber(1,0,2) - 1;

        if (lengthStringPendulumBefore > TOP_LIMIT_LENGTH_PENDULUM - MAX_RANDOM_MAGNITUDE_LENGTH_STRING)
        {
            randomSign = -1;
        }
        if (lengthStringPendulumBefore < BOTTOM_LIMIT_LENGTH_PENDULUM + MAX_RANDOM_MAGNITUDE_LENGTH_STRING)
        {
            randomSign = 1;
        }

        float randomMagnitude = RandomConstantSpreadNumber.GetRandomNumber(
            MINIMUM_RANDOM_MAGNITUDE_LENGTH_STRING, 
            MINIMUM_RANDOM_MAGNITUDE_LENGTH_STRING + MAGNITUDE_RANDOM_MAGNITUDE_LENGTH_STRING);
        _lengthString = lengthStringPendulumBefore + randomSign * randomMagnitude; 
        return;
    }

    private void SetRadiusWeight()
    {
        float slopeFunctionAdjustRadius =
            (MAX_FACTOR_RADIUS_BALL - MIN_FACTOR_RADIUS_BALL) /
            (TOP_LIMIT_LENGTH_PENDULUM - BOTTOM_LIMIT_LENGTH_PENDULUM);
        float interceptFunctionAdjustRadius = MAX_FACTOR_RADIUS_BALL - TOP_LIMIT_LENGTH_PENDULUM * slopeFunctionAdjustRadius;
        float factorAdjustRadius = slopeFunctionAdjustRadius * _lengthString + interceptFunctionAdjustRadius;
        _radiusBall *= factorAdjustRadius;
        spriteRenderer.transform.localScale = new Vector3(factorAdjustRadius, factorAdjustRadius, 1f);
    }

    public void SetPendulumCoordinates()
    {
        //UnityEngine.Debug.Log(levelGeneratorScript);
        float jumpX = RandomPolynomialSpreadNumber.GetRandomNumber(2, 1, 3);
        float jumpY = 
            -Character.gravityScale * Mathf.Pow(jumpX, 2) * 0.5f * Mathf.Pow(1 / _character.MoveSpeed, 2)
            + _character.JumpForce * jumpX / _character.MoveSpeed
            - 0.9f - 2 * _radiusBall;

        _pendulumPosCenterBall = new Vector2(
            _pendulumStart.x + jumpX, 
            _pendulumStart.y + jumpY - _radiusBall);

        float startAngle = 
            UnityEngine.Random.value * 0.5f * (Mathf.PI / 2 * (STARTANGLE_PROPORTION_QUARTER_CIRCLE))
            + (Mathf.PI / 2 * STARTANGLE_PROPORTION_QUARTER_CIRCLE);

        _pendulumPosCenter = new Vector2(
            _pendulumPosCenterBall.x + _lengthString * Mathf.Cos(startAngle),
            _pendulumPosCenterBall.y + _lengthString * Mathf.Sin(startAngle));
    }

    public void SetupPendulumLine()
    {
        _lineToOriginGameObject = new GameObject("OriginLine")
        {
            tag = "lineToOrigin"
        };
        _lineToOriginGameObject.transform.parent = this.transform;
        _lineToOriginLineRenderer = _lineToOriginGameObject.AddComponent<LineRenderer>();
        _lineToOriginLineRenderer.SetPosition(0, new Vector3(_pendulumPosCenterBall.x, _pendulumPosCenterBall.y, 0));
        _lineToOriginLineRenderer.SetPosition(1, new Vector3(_pendulumPosCenter.x, _pendulumPosCenter.y, 0));
        _lineToOriginLineRenderer.widthMultiplier = 0.2f;
    }

    public void CalcPendulumConstants()
    {
        _angleMax = Mathf.Asin((_pendulumPosCenter.x - _pendulumPosCenterBall.x) / (_lengthString));
        _angleWeightPendulum = _angleMax;

        float lengthLevel = 2f * _lengthString * (float)Math.Sin(_angleMax) + _radiusBall;
        _pendulumEnd = new Vector2(_pendulumStart.x + lengthLevel + 5f, _pendulumStart.y);
        if (PendulumId == Physic.PendulumList.Length - 1)
        {
            Physic.PosEnd = _pendulumEnd;
        }

    }

    public void UpdatePendulum()
    {
        gameObject.transform.position = new Vector3(
            _pendulumPosCenterBall.x, 
            _pendulumPosCenterBall.y,
            0);

        _angleWeightPendulum = GetCurrentAngle();

        spriteRenderer.transform.position += GetMovementPendulum();
        _angleWeightPendulum = GetCurrentAngle();

        SetNewPositionLineRenderer();

        _pendulumPosCenterBall += GetVectorError();

    }

    private Vector3 GetMovementPendulum()
    {
        float forceWind = 2f * WIND_FORCE_AMPLITUDE
    * (Mathf.PerlinNoise(
        SCALE_NOISE_POSX * _pendulumPosCenterBall.x,
        SCALE_NOISE_TIME * LevelGenerator.Time)
    - 0.5f);

        float alpha = -_angleWeightPendulum;

        float beta = Mathf.PI / 2 - alpha;
        float forceGrav = Character.gravityScale * MASS_WEIGHT_PENDULUM;
        float tick = 0.01f;

        float forceNormal =
            (-Mathf.Tan(alpha) * forceWind + forceGrav) / (Mathf.Sin(beta)
            + Mathf.Tan(alpha) * Mathf.Cos(beta));

        float forceX = forceWind + forceNormal * Mathf.Cos(beta);
        float forceY = -forceGrav + forceNormal * Mathf.Sin(beta);

        float force = (float)Math.Sqrt(forceX * forceX + forceY * forceY);
        if (_angleWeightPendulum <= -Math.PI / 2)
        {
            force *= -1;
        }
        if (forceX < 0)
        {
            force *= -1;
        }

        float acceleration = force / MASS_WEIGHT_PENDULUM;
        _velocityWeightPendulum += acceleration * tick;

        float deltaLength = _velocityWeightPendulum * tick;

        float deltaX = deltaLength * (float)Math.Cos(_angleWeightPendulum);
        float deltaY = deltaLength * (float)Math.Sin(_angleWeightPendulum);

        Vector3 movePendulum = new Vector3(deltaX, deltaY, 0);
        _pendulumPosCenterBall += new Vector2(movePendulum.x, movePendulum.y);
        return movePendulum;
    }

    private void SetNewPositionLineRenderer()
    {
        _lineToOriginLineRenderer.SetPosition(0, new Vector3(_pendulumPosCenterBall.x, _pendulumPosCenterBall.y, 0));
        _lineToOriginLineRenderer.SetPosition(1, new Vector3(_pendulumPosCenter.x, _pendulumPosCenter.y, 0));
    }

    private Vector2 GetVectorError()
    {
        Vector2 distanceOrigin = _pendulumPosCenter - _pendulumPosCenterBall;
        Vector2 PosCenterOnCircle = new Vector2(_lengthString * Mathf.Cos(_angleWeightPendulum + 1.5f * (float)Math.PI), _lengthString * Mathf.Sin(_angleWeightPendulum + 1.5f * (float)Math.PI));
        return distanceOrigin + PosCenterOnCircle;
    }

    private float GetCurrentAngle()
    {
        float _currentAngle = -(float)Math.Atan((_pendulumPosCenter.x - _pendulumPosCenterBall.x) / (_pendulumPosCenter.y - _pendulumPosCenterBall.y));
        if (_pendulumPosCenterBall.y > _pendulumPosCenter.y)
        {
            return _currentAngle - (float)Math.PI;
        }
        return _currentAngle;
    }

    public void DestroyPendulum()
    {
        Destroy(this.gameObject);
    }

}
