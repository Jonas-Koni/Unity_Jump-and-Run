using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Plains : Level
{
    private const int NUMBER_PLATFORMS = 8;
    private Vector2[] _positionPlatforms;

    private List<GameObject> _blocks;
    private int[] _numberBlocksInPlatform;
    private const int FIRST_PLATFORM_NUMBER_BLOCKS = 5;
    private const int NUMBER_BLOCKS_IN_PLATFORM_MIN_VALUE = 2;
    private const int NUMBER_BLOCKS_IN_PLATFORM_MAX_VALUE = 5;

    private Vector2 _blockSize;
    private float _scaleBlock;

    private const float GRASS_OFFSET_Y = -4.2f;
    private const float BLOCK_POSITION_Y_MIN = 0.2f;
    private const float BLOCK_POSITION_Y_MAX = 2.3f;

    private const float RANDOM_MARGIN_X_MIN_VALUE = -0.4f;
    private const float RANDOM_MARGIN_X_MAX_VALUE = -0.17f;
    private const float MARGIN_X_LAST_BLOCK = 5f;

    private GameObject _grassGameObject;
    private GameObject _dirtGameObject;

    private Character _characterScript;
    private LevelGenerator _levelGeneratorScript;

    private void Awake()
    {
        _blocks = new List<GameObject>();
        GameObject _levelGenerator = GameObject.Find("LevelGenerator");
        _levelGeneratorScript = _levelGenerator.GetComponent<LevelGenerator>();

        _characterScript = GameObject.Find("Character").GetComponent<Character>();
        _grassGameObject = _levelGeneratorScript.GrassGameObject;
        _dirtGameObject = _levelGeneratorScript.DirtGameObject;

        Vector3 blockBoundSize = _grassGameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;

        _scaleBlock = _grassGameObject.transform.localScale.x;
        _blockSize = new Vector2(blockBoundSize.x, blockBoundSize.y);
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
        _positionPlatforms = new Vector2[NUMBER_PLATFORMS];
        _numberBlocksInPlatform = new int[NUMBER_PLATFORMS];
        _positionPlatforms[0] = new Vector2(PosStart.x, PosStart.y);
        _numberBlocksInPlatform[0] = FIRST_PLATFORM_NUMBER_BLOCKS;

        for (int i = 1; i < _positionPlatforms.Length - 1; i++)
        {
            float jumpPosY = RandomConstantSpreadNumber.GetRandomNumber(BLOCK_POSITION_Y_MIN, BLOCK_POSITION_Y_MAX);
            float jumpHeight = jumpPosY - _positionPlatforms[i - 1].y;

            float speedX = SpeedCharacter;
            float speedY = _characterScript.JumpForce;

            float randomMarginX = RandomConstantSpreadNumber.GetRandomNumber(RANDOM_MARGIN_X_MIN_VALUE, RANDOM_MARGIN_X_MAX_VALUE);

            float root = Mathf.Sqrt(Mathf.Pow(speedY, 2) + 2 * LevelGenerator.gravityScale * jumpHeight);
            float jumpWidth = speedX / LevelGenerator.gravityScale * (speedY + root) + randomMarginX;

            float newPosX = _positionPlatforms[i - 1].x + _numberBlocksInPlatform[i - 1] * _blockSize.x * _scaleBlock + jumpWidth;
            float newPosY = _positionPlatforms[i - 1].y + jumpHeight;

            int platformLength = (int) RandomConstantSpreadNumber.GetRandomNumber(
                NUMBER_BLOCKS_IN_PLATFORM_MIN_VALUE,
                NUMBER_BLOCKS_IN_PLATFORM_MAX_VALUE);

            _positionPlatforms[i] = new Vector2(newPosX, newPosY);
            _numberBlocksInPlatform[i] = platformLength;
        }

        float lastPosX = _positionPlatforms[^2].x + _numberBlocksInPlatform[^2] * _blockSize.y * _scaleBlock + MARGIN_X_LAST_BLOCK;
        float lastPosY = 0f;
        _positionPlatforms[^1] = new Vector2(lastPosX, lastPosY);
        _numberBlocksInPlatform[^1] = 3;

        PosEnd = new Vector2(lastPosX + _numberBlocksInPlatform[^1] * _blockSize.y * _scaleBlock, lastPosY);

        for (int i = 0; i < _positionPlatforms.Length; i++)
        {
            float marginY = GRASS_OFFSET_Y + _positionPlatforms[i].y;
            for (int l = 0; l < _numberBlocksInPlatform[i]; l++)
            {
                float positionX = _positionPlatforms[i].x + _blockSize.x * _scaleBlock * l;
                Vector2 positionGrassBlock = new(positionX, marginY);
                AddBlock(_grassGameObject, positionGrassBlock);

                int indexDirtBlock = 0;
                while(IsBlockAboveWater(marginY, indexDirtBlock))
                {
                    float positionY = marginY - (indexDirtBlock + 1) * _blockSize.y * _scaleBlock;
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
        float offSetGrassBlock = -0.5f * _blockSize.y * _scaleBlock;
        float offSetDirtBlocksAbove = -indexDirtBlock * _blockSize.y * _scaleBlock;
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
