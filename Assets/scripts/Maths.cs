using UnityEngine;

public class Maths : Level
{
    public static float Time;
    public static float Scale;
    public static float Amplitude;

    private int _numberWaves;
    private GameObject[] _wavesObject;

    private void Awake()
    {
        Scale = .5f;
        Amplitude = 2f;

        _numberWaves = 2;
    }

    public override void GenerateSection(int seed)
    {
        _wavesObject = new GameObject[_numberWaves];

        Wave firstWaveScript;
        GameObject firstWaveObject = new GameObject("Wave");
        firstWaveObject.layer = LayerMask.NameToLayer("ground");

        firstWaveScript = firstWaveObject.AddComponent<Wave>();
        firstWaveScript.WaveId = 0;
        firstWaveScript.WaveStart = PosStart;
        firstWaveScript.GenerateSectionWave(seed);
        _wavesObject[0] = firstWaveObject;

        for (int id = 1; id < _wavesObject.Length; id++)
        { 
            Wave newWaveScript;
            GameObject newWaveObject;

            newWaveObject = new GameObject("Wave");
            newWaveObject.layer = LayerMask.NameToLayer("ground");
            newWaveScript = newWaveObject.AddComponent<Wave>();
            newWaveScript.WaveId = id;

            newWaveScript.WaveStart = new Vector2(_wavesObject[id - 1].GetComponent<Wave>().WaveEnd.x + 1f, 1f);
            newWaveScript.GenerateSectionWave(seed);

            _wavesObject[id] = newWaveObject;
        }
        PosEnd = _wavesObject[_wavesObject.Length - 1].GetComponent<Wave>().WaveEnd;
    }

    public override void DisplayLevel(GameObject[] levels, int level, Transform grass)
    {
        for (int id = 0; id < _wavesObject.Length; id++)
        {
            _wavesObject[id].GetComponent<Wave>().DisplayWave();
        }
    }

    public override void UpdateSection()
    {
        for (int id = 0; id < _wavesObject.Length; id++)
        {
            _wavesObject[id].GetComponent<Wave>().updateSectionWave();
        }
        Time += 0.01f;
    }

    public override void DestroyContent()
    {
        for (int id = 0; id < _wavesObject.Length; id++)
        {
            Destroy(_wavesObject[id]);
        }

    }
}
