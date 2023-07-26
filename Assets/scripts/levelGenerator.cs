using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

public class levelGenerator : MonoBehaviour
{
    [SerializeField] private Transform grass;

    enum levelState {plains, maths, german, physics, biology, english, history }
    int currentLevel = 0;

    private int seed = 1;
    private GameObject character;
    private Rigidbody2D rb;
    private charakter cs;

    private Vector3[] PositionList;
    private Level[] levels;

    private void Start()
    {
        character = GameObject.Find("Character");
        rb = character.GetComponent<Rigidbody2D>();
        cs = character.GetComponent<charakter>();

        levels = new Level[7];

        //first Level
        /*PositionList = new Vector3[16];
        PositionList[0] = new Vector3(-10f, 0f, 10f);
        PositionList = calcPositionPlatform(PositionList);
        Level firstLevel = new Plains(0, PositionList, true);
        levels[0] = firstLevel;*/

        //first level
        Level firstLevel = new Plains(0);
        firstLevel.cs = cs;
        firstLevel.rb = rb;
        firstLevel.PosStart = new Vector2(-7f, 0f);
        firstLevel.generateSection(seed);
        levels[0] = firstLevel;

        //level 2-7
        for(int i = 1; i < levels.Length; i++)
        {
            GenerateLevel(i,i);
        }
    }

    private void Awake()
    {
    }

    private void Update()
    {
        displayPlatform();
    }


    class Level
    {
        public int levelId {get; set;}
        public Boolean isGenerated { get; set; }
        public Rigidbody2D rb { get; set;}
        public charakter cs { get; set;}
        public Vector2 PosStart { get; set;}
        public Vector2 PosEnd { get; set;}
        //public levelState state { get; set; }


        public virtual void displayLevel(Level[] levels, int level,Transform grass) {}

        public virtual void generateSection(int seed)
        {

        }
    }

    class Plains : Level
    {
        public Vector3[] platforms { get; set; }
        public Plains(int levelId)
        {
            this.levelId = levelId;
        }

        public override void displayLevel(Level[] levels, int positionInLevels, Transform grass)
        {
            Vector3[] PositionList = ((Plains)levels[positionInLevels]).platforms;
            for (int i = 0; i < PositionList.Length; i++)
            {
                for (int l = 0; l < PositionList[i].z; l++)
                {
                    Instantiate(grass, new Vector2(PositionList[i].x + 1.7f * l, -4.2f + PositionList[i].y), Quaternion.identity);
                }
            }
        }

        public override void generateSection(int seed)
        {
            Vector2 Range = new Vector2(0.5f, 2.0f);
            platforms = new Vector3[4];
            platforms[0] = new Vector3(PosStart.x, PosStart.y, 10);
            
            for (int i = 1; i < platforms.Length-1; i++)
            {
                Boolean PerfectPosition = false;

                float AmplitudeNoise = 5.0f;
                float marginRight = 4.0f;

                int PlatformLength = -1;

                float jumpX = -1f;
                float jumpY = -1f;

                float newPosX;
                float newPosY;

                do
                {
                    jumpX = Mathf.PerlinNoise((seed * platforms[i - 1].x) * 0.24643f, 1) * AmplitudeNoise + marginRight;
                    jumpY = -rb.gravityScale * 9.81f * Mathf.Pow(jumpX, 2) * 0.5f * Mathf.Pow(1 / cs.moveSpeed, 2) + cs.jumpForce * jumpX / cs.moveSpeed - 0.6f;

                    newPosX = platforms[i - 1].x + platforms[i - 1].z * 1.7f + jumpX;// + PlatformLength * 1.7f; //variabler Wert, später bitte konstant!
                    newPosY = platforms[i - 1].y + jumpY;

                    Boolean heightToLow = newPosY < Range.x;
                    Boolean heightToHigh = newPosY > Range.y;

                    if (heightToLow)
                    {
                        AmplitudeNoise *= 0.85f;
                        marginRight *= 0.85f;
                    }
                    if (heightToHigh)
                    {
                        AmplitudeNoise *= 1.15f;
                        marginRight *= 1.15f;
                    }
                    if ((!heightToLow && !heightToHigh))
                    {
                        PerfectPosition = true;
                    }

                } while (!PerfectPosition);

                PlatformLength = (int)(Mathf.PerlinNoise(seed * i * 0.017434f, seed * i * 0.137434f) * 10 + 1);
                platforms[i] = new Vector3(newPosX, newPosY, PlatformLength);
            }
            float lastPosX = platforms[platforms.Length-2].x + platforms[platforms.Length - 2].z*1.7f + 3f;
            float lastPosY = 0f;
            platforms[platforms.Length - 1] = new Vector3(lastPosX, lastPosY, 3);

            PosEnd = new Vector2(lastPosX + platforms[platforms.Length - 1].z * 1.7f, lastPosY);
            //return platforms;
        }
    }

    public void GenerateLevel(int positionInLevels, int levelId)
    {
        levelState currentLevelState = (levelState) getLevelState(levelId);
        Level newLevel;
        switch (currentLevelState)
        {
            case levelState.plains:
                newLevel = new Plains(levelId);
                newLevel.rb = rb;
                newLevel.cs = cs;

                newLevel.PosStart = new Vector2(levels[levelId - 1].PosEnd.x + 1f, levels[levelId - 1].PosEnd.y);

                newLevel.generateSection(seed);
                break;
            default:
                return;
                //case levelState.physics:
                //newLevel = new Physics(levelId, PositionList, false);

        }
        levels[positionInLevels] = newLevel;
    }


    public int getLevel(float posX)
    {
        int levelId = levels[2].levelId;
        Boolean PlayerRightLevel = posX > levels[2].PosEnd.x;
        if(PlayerRightLevel)
        {
            if (levelId > 5)
            {
                for(int i = 1; i < levels.Length-1; i++)
                {
                    levels[i] = levels[i + 1];
                }

            }
        }
        return -1;
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
        for (int i = 0; i < 4; i ++) // !ArrayIndexOutOfBoundary
        {
            if(i < 0)
            {
                continue;
            }
            /*if (!levels[i].isGenerated)
            {
                GenerateLevel(i);
            }*/
            levels[i].displayLevel(levels, i, grass);
            
            
        }
        Destroy(this);

    }


}
