using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;

public class SketchPoints
{
    public List<Vector2> Points { private set; get; }

    public SketchPoints()
    {
        Points = new List<Vector2>();
    }

    private void AddPoint(Vector2 point)
    {
        Points.Add(point);
    }
}
