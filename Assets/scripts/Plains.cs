using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plains : Level
{
    public Vector3[] platforms { get; set; }
    public Plains(int levelId)
    {
        this.levelId = levelId;
    }

    public override void displayLevel(GameObject[] levels, int positionInLevels, Transform grass)
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

    public override void generateSection(int seed)
    {
        Vector2 Range = new Vector2(0.5f, 2.0f);
        platforms = new Vector3[2];
        platforms[0] = new Vector3(PosStart.x, PosStart.y, 10);

        for (int i = 1; i < platforms.Length - 1; i++)
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
                jumpY = -rb.gravityScale * 9.81f * Mathf.Pow(jumpX, 2) * 0.5f * Mathf.Pow(1 / speedCharacter, 2) + cs.jumpForce * jumpX / speedCharacter - 0.6f;

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

            newPosX--;

            PlatformLength = (int)(Mathf.PerlinNoise(seed * i * 0.017434f, seed * i * 0.137434f) * 10 + 1);
            platforms[i] = new Vector3(newPosX, newPosY, PlatformLength);
        }
        float lastPosX = platforms[platforms.Length - 2].x + platforms[platforms.Length - 2].z * 1.7f + 3f;
        float lastPosY = 0f;
        platforms[platforms.Length - 1] = new Vector3(lastPosX, lastPosY, 3);

        PosEnd = new Vector2(lastPosX + platforms[platforms.Length - 1].z * 1.7f, lastPosY);
    }
}
