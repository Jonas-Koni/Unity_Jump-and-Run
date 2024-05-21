using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Plains : Level
{
    private const int NUMBER_PLATFORMS = 8;
    private int[] _numberBlocksInPlatform;
    private Vector2[] _platforms;
    private Character _character;
    private LevelGenerator _levelGeneratorScript;
    private List<GameObject> _blocks;
    private Vector2 _blockSize;
    private float scaleBlock;
    private const float GRASS_OFFSET_Y = -4.2f;
    private const float BLOCK_POSITION_Y_MAX = 2.3f;
    private const float BLOCK_POSITION_Y_MIN = 0.2f;
    private const float MARGIN_X_LAST_BLOCK = 5f;
    private const float RANDOM_MARGIN_X_MIN_VALUE = -0.4f;
    private const float RANDOM_MARGIN_X_MAX_VALUE = -0.17f;
    private const int FIRST_PLATFORM_NUMBER_BLOCKS = 5;
    private const int PLATFORM_LENGTH_MIN_VALUE = 2;
    private const int PLATFORM_LENGTH_MAX_VALUE = 5;

    private GameObject _grassGameObject;
    private GameObject _dirtGameObject;

    private void Awake()
    {
        _blocks = new List<GameObject>();
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
        GameObject _levelGenerator = GameObject.Find("LevelGenerator");
        _levelGeneratorScript = _levelGenerator.GetComponent<LevelGenerator>();

        _character = GameObject.Find("Character").GetComponent<Character>();

        _grassGameObject = _levelGeneratorScript.GrassGameObject;
        _dirtGameObject = _levelGeneratorScript.DirtGameObject;

        //Debug.Log("3");
        Vector3 BlockSize = _grassGameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        scaleBlock = _grassGameObject.transform.localScale.x;
        _blockSize = new Vector2(BlockSize.x, BlockSize.y);

        _platforms = new Vector2[NUMBER_PLATFORMS];
        _numberBlocksInPlatform = new int[NUMBER_PLATFORMS];
        _platforms[0] = new Vector3(PosStart.x, PosStart.y, FIRST_PLATFORM_NUMBER_BLOCKS);
        _numberBlocksInPlatform[0] = FIRST_PLATFORM_NUMBER_BLOCKS;

        for (int i = 1; i < _platforms.Length - 1; i++)
        {
            float jumpPosY = RandomConstantSpreadNumber.GetRandomNumber(BLOCK_POSITION_Y_MIN, BLOCK_POSITION_Y_MAX);
            float jumpHeight = jumpPosY - _platforms[i - 1].y;

            float speedX = SpeedCharacter;
            float speedY = _character.JumpForce;

            float randomMarginX = RandomConstantSpreadNumber.GetRandomNumber(RANDOM_MARGIN_X_MIN_VALUE, RANDOM_MARGIN_X_MAX_VALUE);

            float root = Mathf.Sqrt(Mathf.Pow(speedY, 2) + 2 * LevelGenerator.gravityScale * jumpHeight);
            float jumpWidth = speedX / LevelGenerator.gravityScale * (speedY + root) + randomMarginX;

            float newPosX = _platforms[i - 1].x + _numberBlocksInPlatform[i - 1] * _blockSize.x * scaleBlock + jumpWidth;
            float newPosY = _platforms[i - 1].y + jumpHeight;

            int platformLength = (int) RandomConstantSpreadNumber.GetRandomNumber(PLATFORM_LENGTH_MIN_VALUE, PLATFORM_LENGTH_MAX_VALUE);
            _platforms[i] = new Vector3(newPosX, newPosY, platformLength);
            _numberBlocksInPlatform[i] = platformLength;
        }

        float lastPosX = _platforms[_platforms.Length - 2].x + _numberBlocksInPlatform[_platforms.Length - 2] * _blockSize.y * scaleBlock + MARGIN_X_LAST_BLOCK;
        float lastPosY = 0f;
        _platforms[_platforms.Length - 1] = new Vector3(lastPosX, lastPosY, 3);

        PosEnd = new Vector2(lastPosX + _numberBlocksInPlatform[_platforms.Length - 1] * 1.7f, lastPosY);

        for (int i = 0; i < _platforms.Length; i++)
        {
            float marginY = GRASS_OFFSET_Y + _platforms[i].y;
            for (int l = 0; l < _numberBlocksInPlatform[i]; l++)
            {
                float positionX = _platforms[i].x + _blockSize.x * scaleBlock * l;
                Vector2 positionBlock = new(positionX, marginY);
                AddBlock(_grassGameObject, positionBlock);

                int indexDirtBlock = 1;
                while(IsBlockAboveWater(marginY, indexDirtBlock))
                {
                    float positionY = marginY - indexDirtBlock * _blockSize.y * scaleBlock;
                    Vector2 positionDirtBlock = new(positionX, positionY);
                    AddBlock(_dirtGameObject, positionDirtBlock);
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
