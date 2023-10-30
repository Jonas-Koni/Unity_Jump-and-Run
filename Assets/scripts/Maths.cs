using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Maths : Level
{
    [SerializeField] private Line linePrefab;

    EdgeCollider2D edgeCollider;
    LineRenderer lineRenderer;

    float time;
    int numberOfPoints;
    float scale;
    float frequency;
    float wavelength;
    float amplitude;

    private void Awake()
    {
        edgeCollider = gameObject.GetComponent<EdgeCollider2D>();
    }

    public override void displayLevel(GameObject[] levels, int positionInLevels, Transform grass)
    {
        lineRenderer.numCornerVertices = 3;
        lineRenderer.numCapVertices = 3;
        lineRenderer.widthMultiplier = 1f;
        
        for (int i = 0; i < numberOfPoints; i++)
        {
            float x = i * scale;
            float y = Mathf.Sin(2*Mathf.PI * (time*frequency - x/wavelength)) * amplitude-2f;

            lineRenderer.SetPosition(i, new Vector3(x + PosStart.x, y, 0f));
        }
    }

    public override void generateSection(int seed)
    {
        time = 0f;
        scale = .5f;
        amplitude = 2f;

        numberOfPoints = (int)(50f * Mathf.PerlinNoise(PosStart.x * 0.67f, 1f) + 10f);
        frequency = 1.6f * Mathf.PerlinNoise(PosStart.x * 0.67f, 1f) + 0.4f;
        wavelength = 20f * Mathf.PerlinNoise(PosStart.x * 0.37f, 1f) + 10f;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();

        PosEnd = new Vector2(PosStart.x + numberOfPoints*scale + 1f + speedCharacter/4f, 1f);

        lineRenderer.positionCount = numberOfPoints;        
    }

    void SetEdgeCollider(LineRenderer lineRenderer) 
    {
        List<Vector2> edges = new List<Vector2>();

        for (int point = 0; point < lineRenderer.positionCount; point++)
        {
            Vector3 lineRendererPoint = lineRenderer.GetPosition(point);
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }
        edgeCollider.edgeRadius = 0.4f;
        edgeCollider.SetPoints(edges);
    }

    public override void updateSection()
    {
        lineRenderer = GetComponent<LineRenderer>();
        time += 0.01f;
        SetEdgeCollider(lineRenderer);
        displayLevel(null, -1, null);
    }

}
