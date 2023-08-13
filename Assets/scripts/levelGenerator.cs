using System;
using System.Data;
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

        GoLevels = new GameObject[7];
        Level firstLevel;
        GameObject GoFirstLevel = new GameObject("Plains");
        firstLevel = GoFirstLevel.AddComponent<Plains>();
        firstLevel.cs = cs;
        firstLevel.rb = rb;
        firstLevel.levelId = 0;
        firstLevel.PosStart = new Vector2(-7f, 0f);
        firstLevel.generateSection(seed);
        GoLevels[0] = GoFirstLevel;

        //level 2-7
        for (int i = 1; i < GoLevels.Length; i++)
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
        //UnityEngine.Debug.Log("hi");
        checkLevel(rb.transform.position.x);

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
                newLevel.levelId = levelId;
                newLevel.PosStart = new Vector2(GoLevels[positionInLevels-1].GetComponent<Plains>().PosEnd.x + 1f, GoLevels[positionInLevels-1].GetComponent<Plains>().PosEnd.y);
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
            GenerateLevel(6, GoLevels[GoLevels.Length - 2].GetComponent<Level>().levelId + 1);


            GameObject[] GoGrass = GameObject.FindGameObjectsWithTag("grass");
            for(int g = 0; g < GoGrass.Length; g++)
            {
                Destroy(GoGrass[g]);
            }
            displayPlatform();
        }
        for(int h = 0; h < GoLevels.Length; h++)
        {
            UnityEngine.Debug.Log("h: " + h + "; X: " + GoLevels[h].GetComponent<Level>().PosStart + "; Id: " + GoLevels[h].GetComponent<Level>().levelId);
        }
        
        
    }

    public int getLevelState (int level)
    {
        if(level == 0)
        {
            return (int) levelState.plains;
        }
        return (int)levelState.plains;
        //System.Random random = new System.Random(level);
        //return (int) random.Next(0, 7);
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
}
