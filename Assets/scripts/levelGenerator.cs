using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public enum BookType { Start, horizontal, vertical, diagonal, drop, old, End }


public class LevelGenerator : MonoBehaviour
{
    public PhysicsMaterial2D MaterialFriction;
    public Material MaterialLine;
    public Sprite SpritePendulum;
    public GameObject GrassGameObject;
    public GameObject DirtGameObject;

    public static float gravityScale;

    public static int Seed;
    public static int Time;
    public static int CurrentLevel;
    public static Sprite[] BookSprites;
    public static List<GameObject> Levels;

    private const float START_SPEED = 7f;
    private const float MAX_SPEED = 15f;
    private const float DECREASE_SPEED = 11f; //see Documentation! (not existing right now)
    private const int NUMBER_CONSTANT_SPEED_LEVELS = 3;

    private const int NUMBER_START_LEVELS = 4;


    private static GameObject _characterFigure;
    private static Character _characterScript;
    private static Rigidbody2D _rigidbody;
    private static System.Random _randomLevel;


    private void Awake()
    {
        Levels = new List<GameObject>();
        Seed = UnityEngine.Random.Range(0, 2000);
        _randomLevel = new System.Random(Seed);

        BookSprites = Resources.LoadAll<Sprite>("books");

        gravityScale = 3;
    }

    private void Start()
    {
        _characterFigure = GameObject.Find("Character");
        _characterScript = _characterFigure.GetComponent<Character>();
        _rigidbody = _characterFigure.GetComponent<Rigidbody2D>();

        gravityScale = _rigidbody.gravityScale * 9.81f;

        GenerateStartLevels();
    }


    private void Update()
    {
        CheckLevel();
    }

    private void FixedUpdate()
    {
        UpdatePlatform();
        Time++;
    }

    public static void GenerateStartLevels() //should be moved to Start (remove function), when DeadPlayer reload scene?
    {
        for (int id = 0; id < NUMBER_START_LEVELS; id++)
        {
            GenerateLevel(id);
        }
        DisplayPlatform();
    }

    public static void GenerateLevel(int levelId)
    {
        System.Type levelType = GetLevelType(levelId);
        GameObject newLevelObject = new GameObject(levelType.ToString());
        Level newLevelScript = (Level) newLevelObject.AddComponent(levelType);

        newLevelScript.Rigidbody = _rigidbody;
        newLevelScript.CharacterScript = _characterScript;
        newLevelScript.LevelId = levelId;
        newLevelScript.SpeedCharacter = CalcSpeedCharacter(levelId);

        Vector2 posStart;
        if(levelId == 0)
        {
            posStart = new Vector2(-7f, 0f);
        } else
        {
            Level previousLevel = Levels[^1].GetComponent<Level>();
            posStart = new Vector2(previousLevel.PosEnd.x + 1f, 1f);
        }
        newLevelScript.PosStart = posStart;
        newLevelScript.GenerateSection();

        Levels.Add(newLevelObject);
    }

    public static float CalcSpeedCharacter(int levelId)
    {
        if (levelId < NUMBER_CONSTANT_SPEED_LEVELS)
        {
            return START_SPEED;
        }

        float a = MAX_SPEED - START_SPEED;
        float b = -1f / 10f * Mathf.Log((MAX_SPEED - DECREASE_SPEED) / a, 2f);
        float c = MAX_SPEED;
        float n = levelId - NUMBER_CONSTANT_SPEED_LEVELS;

        return -a * Mathf.Pow(2, -b * n) + c;
    }

    private static void CheckLevel()
    {
        bool playerLeftToLevelEnd = _rigidbody.transform.position.x < Levels[^2].GetComponent<Level>().PosEnd.x;
        _characterScript.MoveSpeed = Levels[^2].GetComponent<Level>().SpeedCharacter;

        if (playerLeftToLevelEnd)
        {
            return;
        }
        CurrentLevel = Levels[^1].GetComponent<Level>().LevelId;
        MoveLevels();
        DisplayPlatform();
    }


    public static void MoveLevels()
    {
        Levels[1].GetComponent<Level>().DestroyContent();
        Levels.RemoveAt(1);
        GenerateLevel(Levels[^1].GetComponent<Level>().LevelId + 1);
    }

    private static System.Type GetLevelType(int level)
    {
        if(level == 0)
        {
            return typeof(Plains);
        }
        int randomLevelType = _randomLevel.Next(0, 4);
        return randomLevelType switch
        {
            0 => typeof(Plains),
            1 => typeof(Maths),
            2 => typeof(German),
            3 => typeof(Physic),
            _ => throw new InvalidOperationException()
        };
    }

    public static void DisplayPlatform()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            Levels[i].GetComponent<Level>().DisplayLevel();
        }
    }
    public static void UpdatePlatform()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            Levels[i].GetComponent<Level>().UpdateSection();
        }
    }
}
