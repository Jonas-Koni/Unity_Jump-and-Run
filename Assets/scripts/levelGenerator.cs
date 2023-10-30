using System;
using System.Data;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class levelGenerator : MonoBehaviour
{
    [SerializeField] private Transform grass;

    enum levelState {plains, maths, german, physics, biology, english, history }

    private int seed = 1;
    private GameObject character;
    private Rigidbody2D rb;
    private charakter cs;
    private GameObject[] GoLevels;

    private void Start()
    {
        character = GameObject.Find("Character");
        rb = character.GetComponent<Rigidbody2D>();
        cs = character.GetComponent<charakter>();

        GoLevels = new GameObject[6];

        GenerateFirstLevel(); //level1

        for (int i = 1; i < GoLevels.Length; i++) //level 2-7
        {
            GenerateLevel(i, i);
        }
        displayPlatform();

    }

    private void Awake()
    {
        
    }

    private void Update()
    {
        checkLevel(rb.transform.position.x);
        updatePlatform();


    }

    public void GenerateFirstLevel()
    {
        Level firstLevel;
        GameObject GoFirstLevel = new GameObject("Plains");
        firstLevel = GoFirstLevel.AddComponent<Plains>();
        firstLevel.cs = cs;
        firstLevel.rb = rb;
        firstLevel.levelId = 0;
        firstLevel.speedCharacter = cs.moveSpeed;
        firstLevel.PosStart = new Vector2(-7f, 0f);
        firstLevel.generateSection(seed);
        GoLevels[0] = GoFirstLevel;
    }

    public void GenerateLevel(int positionInLevels, int levelId)
    {
        levelState currentLevelState = (levelState) getLevelState(levelId);
        Level newLevel;
        GameObject GoNewLevel;
        float newSpeed = new float();
        switch (currentLevelState)
        {
            case levelState.plains:
                GoNewLevel = new GameObject("Plains");
                newLevel = GoNewLevel.AddComponent<Plains>();
                newLevel.rb = rb;
                newLevel.cs = cs;
                newLevel.levelId = levelId;

                newLevel.speedCharacter = cs.moveSpeed;
                newSpeed = GoLevels[positionInLevels - 1].GetComponent<Level>().speedCharacter * 1.05f;
                if(newSpeed >= 40)
                {
                    newSpeed = 40;
                }
                if(levelId < 6)
                {
                    newSpeed = cs.moveSpeed;
                }
                newLevel.speedCharacter = newSpeed;

                newLevel.PosStart = new Vector2(GoLevels[positionInLevels-1].GetComponent<Level>().PosEnd.x + 1f, GoLevels[positionInLevels-1].GetComponent<Level>().PosEnd.y);
                newLevel.generateSection(seed);
                break;

            case levelState.maths:
                GoNewLevel = new GameObject("Maths");
                GoNewLevel.layer = LayerMask.NameToLayer("ground");
                newLevel = GoNewLevel.AddComponent<Maths>();
                newLevel.rb = rb;
                newLevel.cs = cs;
                newLevel.levelId = levelId;

                newLevel.speedCharacter = cs.moveSpeed;
                newSpeed = GoLevels[positionInLevels - 1].GetComponent<Level>().speedCharacter * 1.05f;
                if (newSpeed >= 40)
                {
                    newSpeed = 40;
                }
                if (levelId < 6)
                {
                    newSpeed = cs.moveSpeed;
                }
                newLevel.speedCharacter = newSpeed;

                newLevel.PosStart = new Vector2(GoLevels[positionInLevels - 1].GetComponent<Level>().PosEnd.x + 1f, 1f);
                newLevel.generateSection(seed);
                break;
            default:
                return;
                //case levelState.physics:
                //newLevel = new Physics(levelId, PositionList, false);

        }
        GoLevels[positionInLevels] = GoNewLevel;
    }


    public void checkLevel(float posX)
    {
        
        int levelId = GoLevels[2].GetComponent<Level>().levelId;
        Boolean PlayerRightLevel = posX > GoLevels[GoLevels.Length-2].GetComponent<Level>().PosEnd.x;

        if (PlayerRightLevel)
        {
            Destroy(GoLevels[1].GetComponent<Level>().gameObject);
            for (int i = 1; i < GoLevels.Length - 1; i++)
            {
                GoLevels[i] = GoLevels[i + 1];
            }
            GenerateLevel(GoLevels.Length-1, GoLevels[GoLevels.Length - 2].GetComponent<Level>().levelId + 1);

            GameObject[] GoGrass = GameObject.FindGameObjectsWithTag("grass");
            for(int g = 0; g < GoGrass.Length; g++)
            {
                Destroy(GoGrass[g]);
            }
            displayPlatform();
        }
        cs.moveSpeed = GoLevels[GoLevels.Length - 2].GetComponent<Level>().speedCharacter;



    }

    public int getLevelState (int level)
    {
        if(level == 0)
        {
            return (int) levelState.plains;
        }
        //return (int)levelState.maths;
        //                jumpX = Mathf.PerlinNoise((seed * platforms[i - 1].x) * 0.24643f, 1) * AmplitudeNoise + marginRight;
        //return 0;
        //int randomNmb = (int)(2f * Mathf.PerlinNoise((seed * level * 1.23f), 1)); //random?
        //UnityEngine.Debug.Log(randomNmb);
        //return randomNmb;

        //return (int) (2f*Mathf.PerlinNoise((seed * level * 0.23f), 1));
        //UnityEngine.Debug.Log((int)(2f * Mathf.PerlinNoise((seed * level * 0.23f), 1)));
        System.Random random = new System.Random(seed*level);
        int randomNmb = (int)random.Next(0, 2);
        UnityEngine.Debug.Log(randomNmb);
        return randomNmb;
    }

    public void displayPlatform()
    {

        float posX = rb.transform.position.x;
        for (int i = 0; i < GoLevels.Length; i ++) // !ArrayIndexOutOfBoundary
        {
            if(i < 0)
            {
                continue;
            }
            GoLevels[i].GetComponent<Level>().displayLevel(GoLevels, i, grass);
        }
    }
    public void updatePlatform ()
    {
        float posX = rb.transform.position.x;
        for (int i = 0; i < GoLevels.Length; i++) // !ArrayIndexOutOfBoundary
        {
            if (i < 0)
            {
                continue;
            }
            GoLevels[i].GetComponent<Level>().updateSection();
        }
    }
}
