using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


public class Sketch
{
    [JsonProperty("name")]
    public string Name { get; private set; }

    [JsonProperty("id")]
    public int SketchID { get; private set; }

    [JsonProperty("points")]
    public List<SketchPoint> Points { get; private set; }

    [JsonProperty("lines")]
    public List<SketchLine> Lines { get; private set; }

    [JsonProperty("constraints")]
    public List<SketchConstraint> Constraints { get; private set; }

    private uint PointIdCounter = 0;
    private uint LineIdCounter = 0;
    private uint ConstraintIdCounter = 0;

    public Sketch(int id)
    {
        Name = "Sketch_" + id.ToString();
        SketchID = id;
        Points = new List<SketchPoint>();
        Lines = new List<SketchLine>();
        Constraints = new List<SketchConstraint>();
    }

    public void AddPoint(Vector2 position)
    {
        Points.Add(new SketchPoint(position, PointIdCounter));
        PointIdCounter++;
    }
    public SketchPoint AddPoint(float x, float y)
    {
        SketchPoint point = new SketchPoint(new Vector2(x, y), PointIdCounter);
        Points.Add(point);
        PointIdCounter++;
        return point;
    }

    public SketchLine AddLine(uint firstID, uint secondID, bool construction = false) 
    {
        if (firstID == secondID) return null;

        SketchPoint first = Points.Find(x => x.ID == firstID);
        if (first == null) return null;

        SketchPoint second = Points.Find(x => x.ID == secondID);
        if (first == null) return null;

        SketchLine line = new SketchLine(first, second, LineIdCounter, construction);
        Lines.Add(line);
        LineIdCounter++;

        return line;
    }
}