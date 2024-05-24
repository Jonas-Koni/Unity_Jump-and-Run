using System.Collections.Generic;
using Unity.Mathematics;
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

    enum LevelState 
    { 
        plains, 
        maths, 
        german, 
        physics, 
        biology, 
        english, 
        history 
    }

    public static int Seed;
    public static int Time;
    public static int CurrentLevel;
    public static Sprite[] BookSprites;
    public static GameObject[] Levels;

    private const float MAX_SPEED = 15f;
    private static GameObject _characterFigure;
    private static Character _characterScript;
    private static Rigidbody2D _rigidbody;
    private static System.Random _randomLevel;


    private void Awake()
    {
        Levels = new GameObject[4];
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
        //Debug.Log(Time);
        CheckLevel(_rigidbody.transform.position.x);
    }

    private void FixedUpdate()
    {
        UpdatePlatform();
        Time++;
    }

    public static void GenerateStartLevels() //should be moved to Start (remove function), when DeadPlayer reload scene?
    {
        for (int id = 0; id < Levels.Length; id++)
        {
            int currentLevel = id;
            GenerateLevel(currentLevel, id);
        }
        DisplayPlatform();
    }

    public static void GenerateLevel(int positionInLevels, int levelId)
    {
        LevelState currentLevelState = (LevelState)GetLevelState(levelId);
        Level newLevelScript;
        GameObject newLevelObject;
        switch (currentLevelState)
        {
            case LevelState.plains:
                newLevelObject = new GameObject("Plains");
                newLevelScript = newLevelObject.AddComponent<Plains>();
                break;

            case LevelState.maths:
                newLevelObject = new GameObject("Maths");
                newLevelScript = newLevelObject.AddComponent<Maths>();
                break;
            case LevelState.german:
                newLevelObject = new GameObject("German");
                newLevelScript = newLevelObject.AddComponent<German>();
                break;
            case LevelState.physics:
                newLevelObject = new GameObject("Physic");
                newLevelScript = newLevelObject.AddComponent<Physic>();
                break;
            default:
                return;
                //case levelState.physics:
                //newLevel = new Physics(levelId, PositionList, false);

        }
        newLevelScript.Rigidbody = _rigidbody;
        newLevelScript.CharacterScript = _characterScript;
        newLevelScript.LevelId = levelId;
        newLevelScript.SpeedCharacter = CalcSpeedCharacter(positionInLevels, levelId);

        Vector2 posStart;
        if(levelId == 0)
        {
            posStart = new Vector2(-7f, 0f);
        } else
        {
            Level previousLevel = Levels[positionInLevels - 1].GetComponent<Level>();
            posStart = new Vector2(previousLevel.PosEnd.x + 1f, 1f);
        }
        newLevelScript.PosStart = posStart;
        newLevelScript.GenerateSection();

        Levels[positionInLevels] = newLevelObject;
    }

    public static float CalcSpeedCharacter(int positionInLevels, int levelId)
    {
        bool inStartLevels = levelId < 6;
        if (inStartLevels)
        {
            return _characterScript.MoveSpeed;
        }

        float newSpeed = Levels[positionInLevels - 1].GetComponent<Level>().SpeedCharacter * 1.05f;
        bool tooFast = newSpeed >= MAX_SPEED;
        if (tooFast)
        {
            return MAX_SPEED;
        }

        return newSpeed;
    }

    public static void CheckLevel(float posX)
    {
        bool playerLeftToLevelEnd = posX < Levels[^2].GetComponent<Level>().PosEnd.x;
        _characterScript.MoveSpeed = Levels[^2].GetComponent<Level>().SpeedCharacter;

        if (playerLeftToLevelEnd)
        {
            return;
        }
        CurrentLevel = Levels[Levels.Length - 1].GetComponent<Level>().LevelId;
        //DestroyLevels();
        MoveLevels();
        DisplayPlatform();
        RefreshData();
    }
    public static void RefreshData()
    {
        for (int levelIndex = 0; levelIndex < Levels.Length; levelIndex++)
        {
            Levels[levelIndex].GetComponent<Level>().RefreshData();
        }
    }

    public static void DestroyLevels()
    {
        Levels[1].GetComponent<Level>().DestroyContent();
        Destroy(Levels[1].GetComponent<Level>().gameObject);
    }

    public static void MoveLevels()
    {
        Levels[1].GetComponent<Level>().DestroyContent();
        for (int i = 1; i < Levels.Length - 1; i++)
        {
            Levels[i] = Levels[i + 1];
        }
        GenerateLevel(Levels.Length - 1, Levels[^2].GetComponent<Level>().LevelId + 1);
    }

    private static LevelState GetLevelState(int level)
    {
        if (level == 0)
        {
            return LevelState.plains;
        }
        //return LevelState.plains;
        //return LevelState.physics;
        LevelState randomNmb = (LevelState)(_randomLevel.Next(0, 4));
        return randomNmb;
    }

    public static void DisplayPlatform()
    {
        for (int i = 0; i < Levels.Length; i++)
        {
            Levels[i].GetComponent<Level>().DisplayLevel();
        }
    }
    public static void UpdatePlatform()
    {
        for (int i = 0; i < Levels.Length; i++)
        {
            Levels[i].GetComponent<Level>().UpdateSection();
        }
    }
    public static void DeadPlayer()
    {
        for (int i = 1; i < Levels.Length; i++)
        {
            Levels[i].GetComponent<Level>().DestroyContent();
            Destroy(Levels[i].GetComponent<Level>().gameObject);
        }
        Seed = UnityEngine.Random.Range(0, 2000);
        _randomLevel = new System.Random(Seed);
        GenerateStartLevels();
        DisplayPlatform();
        //        LevelGenerator.Seed = UnityEngine.Random.Range(0, 2000);

    }
}
