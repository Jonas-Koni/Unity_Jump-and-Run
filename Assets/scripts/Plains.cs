using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Plains : Level
{
    public static int _platfromSize;
    public Vector3[] platforms { get; set; }
    private GameObject _levelGenerator;
    private Character _character;
    private LevelGenerator _levelGeneratorScript;
    public List<GameObject> _blocks;
    
    private Vector2 _blockSize;
    private const float GRASS_OFFSET_Y = -4.2f;
    private float scaleBlock;

    private void Start()
    {
        _platfromSize = 4; //does not work

        //_seed = _levelGeneratorScript._seed;

    }
    private void Awake()
    {
        _levelGenerator = GameObject.Find("LevelGenerator");
        _levelGeneratorScript = _levelGenerator.GetComponent<LevelGenerator>();

        _character = GameObject.Find("Character").GetComponent<Character>();
    }

    public override void DestroyContent()
    {
        for (int i = 0; i < _blocks.Count; i++)
        {
            Destroy(_blocks[i]);
        }
        Destroy(this);
        Destroy(gameObject);
    }

    public override void GenerateSection()
    {
        Vector2 Range = new Vector2(0.5f, 2.0f);
        platforms = new Vector3[4];
        platforms[0] = new Vector3(PosStart.x, PosStart.y, 10);

        for (int i = 1; i < platforms.Length - 1; i++)
        {
            bool perfectPosition = false;

            float amplitudeNoise = 5.0f;
            float marginRight = 4.0f;

            int platformLength;

            float jumpX;
            float jumpY;

            float newPosX;
            float newPosY;

            do
            {
                jumpX = Mathf.PerlinNoise((LevelGenerator.Seed + platforms[i - 1].x) + 0.24643f, 1) * amplitudeNoise + marginRight;
                jumpY = -LevelGenerator.gravityScale * Mathf.Pow(jumpX, 2) * 0.5f * Mathf.Pow(1 / SpeedCharacter, 2) + _character.JumpForce * jumpX / SpeedCharacter - 0.6f;

                newPosX = platforms[i - 1].x + platforms[i - 1].z * 1.7f + jumpX;// + PlatformLength * 1.7f; //variabler Wert, sp�ter bitte konstant!
                newPosY = platforms[i - 1].y + jumpY;

                bool heightToLow = newPosY < Range.x;
                bool heightToHigh = newPosY > Range.y;

                if (heightToLow)
                {
                    amplitudeNoise *= 0.85f;
                    marginRight *= 0.85f;
                }
                if (heightToHigh)
                {
                    amplitudeNoise *= 1.15f;
                    marginRight *= 1.15f;
                }
                if ((!heightToLow && !heightToHigh))
                {
                    perfectPosition = true;
                }

            } while (!perfectPosition);

            platformLength = (int)(Mathf.PerlinNoise(LevelGenerator.Seed + i + 0.017434f, LevelGenerator.Seed + i + 0.137434f) * 10 + 1);
            platforms[i] = new Vector3(newPosX, newPosY, platformLength);
        }
        float lastPosX = platforms[platforms.Length - 2].x + platforms[platforms.Length - 2].z * 1.7f + 3f;
        float lastPosY = 0f;
        platforms[platforms.Length - 1] = new Vector3(lastPosX, lastPosY, 3);

        PosEnd = new Vector2(lastPosX + platforms[platforms.Length - 1].z * 1.7f, lastPosY);

        Vector3[] PositionList = platforms;
        _blocks = new List<GameObject>();

        GameObject GrassGameObject = _levelGeneratorScript.GrassGameObject;
        GameObject DirtGameObject = _levelGeneratorScript.DirtGameObject;

        Vector3 BlockSize = GrassGameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        scaleBlock = GrassGameObject.transform.localScale.x;
        _blockSize = new Vector2(BlockSize.x, BlockSize.y);

        for (int i = 0; i < PositionList.Length; i++)
        {
            float marginY = GRASS_OFFSET_Y + PositionList[i].y;
            for (int l = 0; l < PositionList[i].z; l++)
            {
                float positionX = PositionList[i].x + _blockSize.x * scaleBlock * l;
                Vector2 positionBlock = new(positionX, marginY);
                AddBlock(GrassGameObject, positionBlock);

                int indexDirtBlock = 1;
                while(IsBlockAboveWater(marginY, indexDirtBlock))
                {
                    float positionY = marginY - indexDirtBlock * _blockSize.y * scaleBlock;
                    Vector2 positionDirtBlock = new(positionX, positionY);
                    AddBlock(DirtGameObject, positionDirtBlock);
                    indexDirtBlock++;
                }
            }
        }
    }

    private bool IsBlockAboveWater(float marginY, int indexDirtBlock)
    {
        float heightWater = GameObject.Find("Water").GetComponent<BoxCollider2D>().bounds.max.y;
        float offSetGrassBlock = 0.5f * _blockSize.y * scaleBlock;
        float offSetDirtBlocksAbove = -indexDirtBlock * _blockSize.y * scaleBlock;
        return marginY + offSetGrassBlock + offSetDirtBlocksAbove > heightWater;
    }

    private void AddBlock(GameObject gameObject, Vector2 position)
    {
        UnityEngine.Object instantiatedObject = Instantiate(gameObject, position, Quaternion.identity);
        GameObject instantiatedGameObject = instantiatedObject as GameObject;
        BoxCollider2D boxColliderDirt = instantiatedGameObject.GetComponent<BoxCollider2D>();
        boxColliderDirt.size = new Vector2(_blockSize.x, _blockSize.y);
        instantiatedGameObject.transform.parent = this.transform;
        _blocks.Add(instantiatedGameObject);
    }
}
