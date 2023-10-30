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

    float moveX;
    int numberOfPoints;
    float scale;
    float frequency;
    float amplitude;

    private Vector3[] points { get; set; }

    public Maths(int levelId)
    {
        this.levelId = levelId;
    }
    void Start()
    {
        //edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
    }

    private void Awake()
    {
        moveX = 1f;
        numberOfPoints = 40;
        scale = .5f;
        frequency = .1f;
        amplitude = 2f;

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
            float y = Mathf.Sin(x * frequency + moveX*Mathf.PerlinNoise(PosStart.x*0.3f,1f)) * amplitude-2f;

            lineRenderer.SetPosition(i, new Vector3(x + PosStart.x, y, 0f));
            points[i] = new Vector3(x, y, 0f);
        }
        lineRenderer.useWorldSpace = false;

    }

    public override void generateSection(int seed)
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();

        PosEnd = new Vector2(PosStart.x + numberOfPoints*scale+1f, 1f);

        lineRenderer.positionCount = numberOfPoints;

        points = new Vector3[numberOfPoints];
        
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
        moveX += 0.01f;
        SetEdgeCollider(lineRenderer);
        displayLevel(null, -1, null);
    }

}
