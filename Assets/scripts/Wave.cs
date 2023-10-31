using System.Collections.Generic;
using UnityEngine;

public class Wave : Maths
{
    private EdgeCollider2D _edgeCollider;
    private LineRenderer _lineRenderer;

    public int WaveId;

    public Vector2 WaveStart;
    public Vector2 WaveEnd;

    private int _numberOfPoints;
    private float _frequency;
    private float _wavelength;

    private void Awake()
    {
        _edgeCollider = gameObject.GetComponent<EdgeCollider2D>();
    }

    public void DisplayWave()
    {
        for (int pointIndex = 0; pointIndex < _numberOfPoints; pointIndex++)
        {
            float x = pointIndex * Scale;
            float y = Mathf.Sin(2*Mathf.PI * (Time*_frequency - x/_wavelength)) * Amplitude-2f;

            _lineRenderer.SetPosition(pointIndex, new Vector3(x + WaveStart.x, y, 0f));
        }
    }

    public void GenerateSectionWave(int seed)
    {
        _numberOfPoints = (int)(20f * Mathf.PerlinNoise(WaveStart.x * seed * 0.67f, 1f) + 10f);
        _frequency = 0.6f * Mathf.PerlinNoise(WaveStart.x * seed * 0.57f, 1f) + 0.2f;
        _wavelength = 20f * Mathf.PerlinNoise(WaveStart.x * seed * 0.35f, 1f) + 10f;

        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _edgeCollider = gameObject.AddComponent<EdgeCollider2D>();

        WaveEnd = new Vector2(WaveStart.x + _numberOfPoints*Scale + 1f + SpeedCharacter/4f, 1f);

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
    }

    public void updateSectionWave()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        SetEdgeCollider(_lineRenderer);
        DisplayWave();
    }
}
