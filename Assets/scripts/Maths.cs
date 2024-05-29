using UnityEngine;

public class Maths : Level
{
    protected const float SCALE_X = 0.5f;
    private const int NUMBER_WAVES = 5;
    private GameObject[] _wavesObject;

    public override void GenerateSection()
    {
        _wavesObject = new GameObject[NUMBER_WAVES];

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
