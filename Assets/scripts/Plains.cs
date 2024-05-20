using UnityEngine;

public class Plains : Level
{
    public static int _platfromSize;
    public Vector3[] platforms { get; set; }
    private GameObject _levelGenerator;
    private LevelGenerator _levelGeneratorScript;
    private Character _character;
    private Transform _grass;
    private GameObject[] _levels;

    private void Start()
    {
        _platfromSize = 4; //does not work
        _levelGenerator = GameObject.Find("LevelGenerator");
        _levelGeneratorScript = _levelGenerator.GetComponent<LevelGenerator>();
        //_seed = _levelGeneratorScript._seed;
        _grass = _levelGeneratorScript.Grass;
        _levels = LevelGenerator.Levels;
    }
    private void Awake()
    {
        _character = GameObject.Find("Character").GetComponent<Character>();

    }

    public override void DisplayLevel(int positionInLevels)
    {
        Vector3[] PositionList = ((Plains)(LevelGenerator.Levels[positionInLevels]).GetComponent<Level>()).platforms;

        for (int i = 0; i < PositionList.Length; i++)
        {
            for (int l = 0; l < PositionList[i].z; l++)
            {
                //UnityEngine.Debug.Log(PositionList[i].y);
                Instantiate(
                    LevelGenerator._grassStatic, 
                    new Vector2(
                        PositionList[i].x + 1.7f * l, 
                        -4.2f + PositionList[i].y
                        ), 
                    Quaternion.identity);
            }
        }
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
                jumpY = -Character.gravityScale * 9.81f * Mathf.Pow(jumpX, 2) * 0.5f * Mathf.Pow(1 / SpeedCharacter, 2) + _character.JumpForce * jumpX / SpeedCharacter - 0.6f;


                newPosX = platforms[i - 1].x + platforms[i - 1].z * 1.7f + jumpX;// + PlatformLength * 1.7f; //variabler Wert, später bitte konstant!
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
    }
}
