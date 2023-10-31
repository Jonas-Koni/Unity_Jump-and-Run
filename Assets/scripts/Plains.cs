using UnityEngine;

public class Plains : Level
{
    public Vector3[] platforms { get; set; }
    
    public override void DisplayLevel(GameObject[] levels, int positionInLevels, Transform grass)
    {        
        Vector3[] PositionList = ((Plains)(levels[positionInLevels]).GetComponent<Level>()).platforms;

        for (int i = 0; i < PositionList.Length; i++)
        {
            for (int l = 0; l < PositionList[i].z; l++)
            {
                Instantiate(grass.gameObject, new Vector2(PositionList[i].x + 1.7f * l, -4.2f + PositionList[i].y), Quaternion.identity);
            } 
        }
    }


    public override void GenerateSection(int seed)
    {
        Vector2 Range = new Vector2(0.5f, 2.0f);
        platforms = new Vector3[2];
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
                jumpX = Mathf.PerlinNoise((seed * platforms[i - 1].x) * 0.24643f, 1) * amplitudeNoise + marginRight;
                jumpY = -Rigidbody.gravityScale * 9.81f * Mathf.Pow(jumpX, 2) * 0.5f * Mathf.Pow(1 / SpeedCharacter, 2) + CharacterScript.JumpForce * jumpX / SpeedCharacter - 0.6f;

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

            newPosX--;

            platformLength = (int)(Mathf.PerlinNoise(seed * i * 0.017434f, seed * i * 0.137434f) * 10 + 1);
            platforms[i] = new Vector3(newPosX, newPosY, platformLength);
        }
        float lastPosX = platforms[platforms.Length - 2].x + platforms[platforms.Length - 2].z * 1.7f + 3f;
        float lastPosY = 0f;
        platforms[platforms.Length - 1] = new Vector3(lastPosX, lastPosY, 3);

        PosEnd = new Vector2(lastPosX + platforms[platforms.Length - 1].z * 1.7f, lastPosY);
    }
}
