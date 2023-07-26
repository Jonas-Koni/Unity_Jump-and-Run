using System;
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

    private Vector3[] PositionList;
    //private Level[] levels;
    private GameObject[] GoLevels;

    private void Start()
    {
        character = GameObject.Find("Character");
        rb = character.GetComponent<Rigidbody2D>();
        cs = character.GetComponent<charakter>();

        //GameObject go = new GameObject();
        //go.AddComponent<Level>();

        //levels = new Level[7];

        //first level
        GoLevels = new GameObject[7];
        Level firstLevel;
        GameObject GoFirstLevel = new GameObject("Plains");
        firstLevel = GoFirstLevel.AddComponent<Plains>();
        firstLevel.cs = cs;
        firstLevel.rb = rb;
        firstLevel.PosStart = new Vector2(-7f, 0f);
        firstLevel.generateSection(seed);
        GoLevels[0] = GoFirstLevel;

        //level 2-7
        for(int i = 1; i < GoLevels.Length; i++)
        {
            GenerateLevel(i,i);
        }
    }

    private void Awake()
    {

    }

    private void Update()
    {
        UnityEngine.Debug.Log("hi");
        checkLevel(transform.position.x);
        displayPlatform();

    }

    public void GenerateLevel(int positionInLevels, int levelId)
    {
        levelState currentLevelState = (levelState) getLevelState(levelId);
        Level newLevel;
        GameObject GoNewLevel;
        switch (currentLevelState)
        {
            case levelState.plains:
                GoNewLevel = new GameObject("Plains");
                newLevel = GoNewLevel.AddComponent<Plains>();
                newLevel.rb = rb;
                newLevel.cs = cs;
                newLevel.PosStart = new Vector2(GoLevels[levelId - 1].GetComponent<Plains>().PosEnd.x + 1f, GoLevels[levelId - 1].GetComponent<Plains>().PosEnd.y);
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
        if(PlayerRightLevel)
        {
            //if (levelId > 5)
            {
                for(int i = 1; i < GoLevels.Length-1; i++)
                {
                    GoLevels[i] = GoLevels[i + 1];
                }
                GenerateLevel(6, GoLevels[GoLevels.Length - 2].GetComponent<Level>().levelId);
            }
        }
    }

    public int getLevelState (int level)
    {
        if(level == 0)
        {
            return (int) levelState.plains;
        }
        return (int)levelState.plains;
        System.Random random = new System.Random(level);
        return (int) random.Next(0, 7);
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
        Destroy(this);
    }
}
