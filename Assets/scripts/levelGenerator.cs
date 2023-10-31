using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform grass;

    enum levelState { plains, maths, german, physics, biology, english, history }

    private int _seed;
    private GameObject _characterFigure;
    private Character _characterScript;
    private Rigidbody2D _rigidbody;
    private GameObject[] _levels; 

    private void Start()
    {
        _levels = new GameObject[5];
        _seed = 2;

        GenerateFirstLevel(); //level1
        GenerateStartLevels(); //level 2 to lentgh -1
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
        firstLevelScript.GenerateSection(_seed);
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
        newLevelScript.GenerateSection(_seed);

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
        bool tooFast = newSpeed >= 20f;
        if (tooFast)
        {
            return 20;
        }
        
        return newSpeed;
    }

    public void CheckLevel(float posX)
    {
        bool playerLeftToLevelEnd = posX < _levels[_levels.Length - 2].GetComponent<Level>().PosEnd.x;
        _characterScript.MoveSpeed = _levels[_levels.Length - 2].GetComponent<Level>().SpeedCharacter;

        if (playerLeftToLevelEnd)
        {
            return;
        }

        DestroyLevels();
        MoveLevels();
        DisplayPlatform();
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
        GenerateLevel(_levels.Length - 1, _levels[_levels.Length - 2].GetComponent<Level>().LevelId + 1);
    }

    private levelState GetLevelState(int level)
    {
        if (level == 0)
        {
            return levelState.plains;
        }
        //return levelState.plains;
        System.Random random = new System.Random(_seed * level);
        levelState randomNmb = (levelState)(random.Next(0, 2));
        return randomNmb;
    }

    public void DisplayPlatform()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i].GetComponent<Level>().DisplayLevel(_levels, i, grass);
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
        GenerateStartLevels();
        GameObject[] GoGrass = GameObject.FindGameObjectsWithTag("grass");
        for (int g = 0; g < GoGrass.Length; g++)
        {
            Destroy(GoGrass[g]);
        }
        DisplayPlatform();
    }
}
