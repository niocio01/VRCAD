using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3[] points = new Vector3[2];

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }
    public void SetPoints(Vector3 first, Vector3 second)
    {
        points[0] = first;
        points[1] = second;
        lineRenderer.positionCount = points.Length;
    }
    private void LateUpdate()
    {
        lineRenderer.SetPositions(points);
    }
}
