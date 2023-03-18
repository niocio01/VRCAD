using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sketch
{
    // Properties
    public string Name { get; private set; }
    public uint SketchID { get; private set; }
    public List<SketchPoint> Points { get; private set; }
    public List<SketchLine> Lines { get; private set; }
    public List<SketchConstraint> Constraints { get; private set; }

    // Counters
    public uint PointIdCounter { get; private set; } = 0;
    public uint LineIdCounter { get; private set; } = 0;
    public uint ConstraintIdCounter { get; private set; } = 0;

    // Constructor
    public Sketch(uint id)
    {
        Name = "Sketch_" + id.ToString();
        SketchID = id;
        Points = new List<SketchPoint>();
        Lines = new List<SketchLine>();
        Constraints = new List<SketchConstraint>();
    }

    // Get Elements
    public SketchPoint GetPoint(uint id)
    {
        return Points.Find(p => p.ID == id);
    }
    public SketchLine GetLine(uint id)
    {
        return Lines.Find(l => l.ID == id);
    }
    public SketchConstraint GetConstraint(uint id)
    {
        return Constraints.Find(c => c.ConstraintID == id);
    }

    // Add Elements
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
    public void AddConstraint(SketchConstraint constraint)
    {
        if (constraint == null) return;
        ConstraintIdCounter++;
        Constraints.Add(constraint);
        return;
    }

    // Set Lists
    public void SetPoints(List<SketchPoint> points)
    {
        Points = points;
    }
    public void SetLines(List<SketchLine> lines)
    {
        Lines = lines; 
    }
    public void SetConstraints(List<SketchConstraint> constraints)
    {
        Constraints = constraints;
    }

    // Auxilary
    public JsonSketch ToJsonSketch()
    {
        JsonSketch jsonSketch = new JsonSketch();
        jsonSketch.Name = Name;
        jsonSketch.SketchID = SketchID;
        jsonSketch.Points = Points;
        jsonSketch.Lines = Lines;

        List<JsonConstraint> jsonConstraints = new List<JsonConstraint>();
        foreach (SketchConstraint sketchConstraint in Constraints)
        {
            jsonConstraints.Add(sketchConstraint.ToJsonConstraint());
        }
        jsonSketch.Constraints = jsonConstraints;

        return jsonSketch;
    }
}

public class JsonSketch
{
    [JsonProperty("name")]
    public string Name;

    [JsonProperty("id")]
    public uint SketchID;

    [JsonProperty("points")]
    public List<SketchPoint> Points;

    [JsonProperty("lines")]
    public List<SketchLine> Lines;

    [JsonProperty("constraints")]
    public List<JsonConstraint> Constraints;

    // Auxilary
    public Sketch ToSketch()
    {
        Sketch sketch = new Sketch(SketchID);
        sketch.SetPoints(Points);
        sketch.SetLines(Lines);

        List<SketchConstraint> sketchConstraints = new List<SketchConstraint>();
        foreach(JsonConstraint jsonConstraint in Constraints)
        {
            sketchConstraints.Add(jsonConstraint.ToSketchConstraint(sketch.Points, sketch.Lines));
        }

        return sketch;
    }
}