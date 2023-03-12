using System.Collections.Generic;
using UnityEngine;


public class Sketch
{
    private int idCounter = 0;
    public List<SketchPoint> Points { get; private set; }    
    public List<SketchLine> Lines { get; private set; }
    public List<SketchConstraint> Constraints { get; private set; }

    public Sketch()
    {
        Points = new List<SketchPoint>();
        Lines = new List<SketchLine>();
        Constraints = new List<SketchConstraint>();
    }

    public int AddPoint(Vector2 position)
    {
        Points.Add(new SketchPoint(position, idCounter));
        return idCounter++;
    }
    public int AddPoint(float x, float y)
    {
        Points.Add(new SketchPoint(new Vector2(x, y), idCounter));
        return idCounter++;
    }
}