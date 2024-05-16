using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public enum BookType { Start, horizontal, vertical, diagonal, drop, old, End }


public class LevelGenerator : MonoBehaviour
{
    [SerializeField] public Transform Grass;
    [SerializeField] public PhysicsMaterial2D MaterialFriction;
    [SerializeField] public Material MaterialLine;
    [SerializeField] public Sprite SpritePendulum;

    public static Transform _grassStatic;

    enum levelState { plains, maths, german, physics, biology, english, history }

    public static int Seed;
    public static int Time;

    public static int CurrentLevel; //test

    public List<Sprite> BookSprites;

    private float _maxSpeed;
    private GameObject _characterFigure;
    private Character _characterScript;
    private Rigidbody2D _rigidbody;
    public static GameObject[] _levels;

    private System.Random _randomLevel;


    private void Start()
    {
        _levels = new GameObject[4];
        _maxSpeed = 15f;
        Seed = UnityEngine.Random.Range(0, 2000);
        _randomLevel = new System.Random(Seed);
        _grassStatic = Grass;
        //public enum BookType { Start, horizontal, vertical, diagonal, drop, old, End }

        GenerateFirstLevel(); //level1
        GenerateStartLevels(); //level 2 to lentgh -1
        for (int i = 0; i < _levels.Length; i++)
        {
            //Debug.Log(_levels[i].GetComponent<Level>() + " level: " + GetLevelState(_levels[i].GetComponent<Level>().LevelId));

        }

    }

    private void Awake()
    {
        _characterFigure = GameObject.Find("Character");
        _rigidbody = _characterFigure.GetComponent<Rigidbody2D>();
        _characterScript = _characterFigure.GetComponent<Character>();
    }

    private void Update()
    {
        CheckLevel(_rigidbody.transform.position.x);
    }

    private void FixedUpdate()
    {
        UpdatePlatform();
        Time++;
    }

    public void GenerateFirstLevel()
    {
        Level firstLevelScript;
        GameObject firstLevelObject = new GameObject("Plains");
        firstLevelScript = firstLevelObject.AddComponent<Plains>();

        firstLevelScript.CharacterScript = _characterScript;
        firstLevelScript.Rigidbody = _rigidbody;
        firstLevelScript.LevelId = 0;
        firstLevelScript.SpeedCharacter = _characterScript.MoveSpeed;
        firstLevelScript.PosStart = new Vector2(-7f, 0f);
        firstLevelScript.GenerateSection();
        _levels[0] = firstLevelObject;

    }

    public void GenerateStartLevels() //level 2-7
    {
        for (int id = 1; id < _levels.Length; id++) //level 2-7
        {
            int positionInLevels = id;
            GenerateLevel(positionInLevels, id);
        }
        DisplayPlatform();
    }

    public void GenerateLevel(int positionInLevels, int levelId)
    {
        levelState currentLevelState = (levelState)GetLevelState(levelId);
        Level newLevelScript;
        GameObject newLevelObject;
        switch (currentLevelState)
        {
            case levelState.plains:
                newLevelObject = new GameObject("Plains");
                newLevelScript = newLevelObject.AddComponent<Plains>();
                break;

            case levelState.maths:
                newLevelObject = new GameObject("Maths");
                newLevelScript = newLevelObject.AddComponent<Maths>();
                break;
            case levelState.german:
                newLevelObject = new GameObject("German");
                newLevelScript = newLevelObject.AddComponent<German>();
                break;
            case levelState.physics:
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
        newLevelScript.PosStart = new Vector2(_levels[positionInLevels - 1].GetComponent<Level>().PosEnd.x + 1f, 1f);
        newLevelScript.GenerateSection();

        _levels[positionInLevels] = newLevelObject;
    }

    public float CalcSpeedCharacter(int positionInLevels, int levelId)
    {
        bool inStartLevels = levelId < 6;
        if (inStartLevels)
        {
            return _characterScript.MoveSpeed;
        }

        float newSpeed = _levels[positionInLevels - 1].GetComponent<Level>().SpeedCharacter * 1.05f;
        bool tooFast = newSpeed >= _maxSpeed;
        if (tooFast)
        {
            return _maxSpeed;
        }

        return newSpeed;
    }

    public void CheckLevel(float posX)
    {
        bool playerLeftToLevelEnd = posX < _levels[^2].GetComponent<Level>().PosEnd.x;
        _characterScript.MoveSpeed = _levels[^2].GetComponent<Level>().SpeedCharacter;

        if (playerLeftToLevelEnd)
        {
            return;
        }
        _characterFigure.GetComponent<ItemCollector>().Test();
        CurrentLevel = _levels[_levels.Length - 1].GetComponent<Level>().LevelId;
        DestroyLevels();
        MoveLevels();
        DisplayPlatform();
        RefreshData();
    }
    public void RefreshData()
    {
        for (int levelIndex = 0; levelIndex < _levels.Length; levelIndex++)
        {
            _levels[levelIndex].GetComponent<Level>().RefreshData();
        }
    }

    public void DestroyLevels()
    {
        _levels[1].GetComponent<Level>().DestroyContent();
        Destroy(_levels[1].GetComponent<Level>().gameObject);
        GameObject[] GoGrass = GameObject.FindGameObjectsWithTag("grass");
        for (int g = 0; g < GoGrass.Length; g++)
        {
            Destroy(GoGrass[g]);
        }
    }

    public void MoveLevels()
    {
        for (int i = 1; i < _levels.Length - 1; i++)
        {
            _levels[i] = _levels[i + 1];
        }
        GenerateLevel(_levels.Length - 1, _levels[^2].GetComponent<Level>().LevelId + 1);
    }

    private levelState GetLevelState(int level)
    {
        if (level == 0)
        {
            return levelState.plains;
        }
        //return levelState.german;
        //System.Random random = new System.Random(Seed + level);
        return levelState.physics;
        levelState randomNmb = (levelState)(_randomLevel.Next(0, 4));
        return randomNmb;
    }

    public void DisplayPlatform()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i].GetComponent<Level>().DisplayLevel(i);
        }
    }
    public void UpdatePlatform()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i].GetComponent<Level>().UpdateSection();
        }
    }
    public void DeadPlayer()
    {
        for (int i = 1; i < _levels.Length; i++)
        {
            _levels[i].GetComponent<Level>().DestroyContent();
            Destroy(_levels[i].GetComponent<Level>().gameObject);
        }
        Seed = UnityEngine.Random.Range(0, 2000);
        _randomLevel = new System.Random(Seed);
        GenerateStartLevels();
        GameObject[] GoGrass = GameObject.FindGameObjectsWithTag("grass");
        for (int g = 0; g < GoGrass.Length; g++)
        {
            Destroy(GoGrass[g]);
        }
        DisplayPlatform();
        //        LevelGenerator.Seed = UnityEngine.Random.Range(0, 2000);

    }
}
