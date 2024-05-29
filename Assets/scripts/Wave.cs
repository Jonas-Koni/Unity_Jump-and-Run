using System.Collections.Generic;
using UnityEngine;

public class Wave : Maths
{
    private GameObject _levelGenerator;
    private LevelGenerator _levelGeneratorScript;

    private EdgeCollider2D _edgeCollider;
    private LineRenderer _lineRenderer;

    private Material _materialLine;
    private PhysicsMaterial2D _materialFriction;

    public int WaveId;

    public Vector2 WaveStart;
    public Vector2 WaveEnd;

    private int _numberOfPoints;
    private float _frequency;
    private float _wavelength;
    private float _amplitude;
    private float _addTime;

    private void Start()
    {
        _levelGenerator = GameObject.Find("LevelGenerator");
        _levelGeneratorScript = _levelGenerator.GetComponent<LevelGenerator>();

        _materialLine = _levelGeneratorScript.MaterialLine;
        _materialFriction = _levelGeneratorScript.MaterialFriction;
    }

    public void DisplayWave()
    {
        for (int pointIndex = 0; pointIndex < _numberOfPoints; pointIndex++)
        {
            float x = pointIndex * SCALE_X;
            float y = Mathf.Sin(2 * Mathf.PI * (Time.time * _frequency - x / _wavelength) + _addTime) * _amplitude - 2f;

            _lineRenderer.SetPosition(pointIndex, new Vector3(x + WaveStart.x, y, 0f));
        }
    }

    public void GenerateSectionWave()
    {
        _numberOfPoints = (int)RandomConstantSpreadNumber.GetRandomNumber(11, 25);
        _frequency = RandomPolynomialSpreadNumber.GetRandomNumber(1, 0.4f, 0.9f);
        _wavelength = RandomPolynomialSpreadNumber.GetRandomNumber(1, 8, 22);
        _amplitude = RandomConstantSpreadNumber.GetRandomNumber(1.2f, 3.4f);
        _addTime = RandomConstantSpreadNumber.GetRandomNumber(0, 2f * Mathf.PI);

        WaveEnd = new Vector2(WaveStart.x + _numberOfPoints * SCALE_X + 6f + SpeedCharacter / 4f, 1f);

        _edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = _numberOfPoints;
        _lineRenderer.numCornerVertices = 3;
        _lineRenderer.numCapVertices = 3;
        _lineRenderer.widthMultiplier = 1f;
    }

    void SetEdgeCollider(LineRenderer lineRenderer)
    {
        List<Vector2> edges = new();
        for (int pointIndex = 0; pointIndex < lineRenderer.positionCount; pointIndex++)
        {
            Vector3 lineRendererPoint = lineRenderer.GetPosition(pointIndex);
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }
        _edgeCollider.edgeRadius = 0.4f;
        _edgeCollider.SetPoints(edges);

        _edgeCollider.sharedMaterial = _materialFriction;
        lineRenderer.sharedMaterial = _materialLine;
    }

    public override void UpdateSection()
    {
        SetEdgeCollider(_lineRenderer);
        DisplayWave();
    }
}
