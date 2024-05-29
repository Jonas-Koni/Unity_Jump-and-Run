using UnityEngine;

public class Maths : Level
{
    //public static float Time;
    public static float Scale;
    public static float Amplitude;

    private int _numberWaves;
    private GameObject[] _wavesObject;

    private void Awake()
    {
        Scale = .5f;
        Amplitude = 2f;

        _numberWaves = 5;
    }

    public override void GenerateSection()
    {
        _wavesObject = new GameObject[_numberWaves];

        for (int id = 0; id < _wavesObject.Length; id++)
        { 
            GameObject newWaveObject;
            newWaveObject = new GameObject("Wave")
            {
                layer = LayerMask.NameToLayer("ground")
            };

            Wave newWaveScript;
            newWaveScript = newWaveObject.AddComponent<Wave>();
            newWaveScript.WaveId = id;
            if(id == 0)
            {
                newWaveScript.WaveStart = PosStart;
            } else
            {
                newWaveScript.WaveStart = new Vector2(_wavesObject[id - 1].GetComponent<Wave>().WaveEnd.x + 1f, 1f);
            }
            newWaveScript.GenerateSectionWave();

            _wavesObject[id] = newWaveObject;
        }
        PosEnd = _wavesObject[^1].GetComponent<Wave>().WaveEnd;
    }


    public override void UpdateSection()
    {
        for (int id = 0; id < _wavesObject.Length; id++)
        {
            _wavesObject[id].GetComponent<Wave>().UpdateSection();
        }
    }

    public override void DestroyContent()
    {
        for (int id = 0; id < _wavesObject.Length; id++)
        {
            Destroy(_wavesObject[id]);
        }

    }
}
