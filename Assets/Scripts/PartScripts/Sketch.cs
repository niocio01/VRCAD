using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Sketch
{
    public static event Action OnPointAdded;
    public static event Action OnLineAdded;

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
    public Sketch(uint id, string name = "")
    {
        if (name == "") Name = "Sketch_" + id.ToString();
        else Name = name;

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

        OnPointAdded?.Invoke();
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

        OnLineAdded?.Invoke();

        return line;
    }

    public SketchLine AddLine(SketchPoint first, SketchPoint second, bool construction = false)
    {
        if (first == second) return null;
        if (first == null) return null;
        if (first == null) return null;

        SketchLine line = new SketchLine(first, second, LineIdCounter, construction);
        Lines.Add(line);
        LineIdCounter++;
        OnLineAdded?.Invoke();

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
        if (Points.Count > 0)
        {
            PointIdCounter = Points.Max(p => p.ID) + 1;
        }
    }
    public void SetLines(List<SketchLine> lines)
    {
        Lines = lines;
        if (Lines.Count > 0)
        {
            LineIdCounter = Lines.Max(l => l.ID) + 1;
        }
    }
    public void SetConstraints(List<SketchConstraint> constraints)
    {
        Constraints = constraints;
        if (Constraints.Count > 0)
        {
            ConstraintIdCounter = Constraints.Max(c => c.ConstraintID) + 1;
        }
    }

    // Auxilary
    public JsonSketch ToJsonSketch()
    {
        JsonSketch jsonSketch = new JsonSketch();
        jsonSketch.Name = Name;
        jsonSketch.SketchID = SketchID;
        jsonSketch.Points = Points;

        List<JsonSketchLine> jsonSketchLines = new List<JsonSketchLine>();
        foreach (SketchLine line in Lines)
        {
            jsonSketchLines.Add(line.ToJsonLine());
        }
        jsonSketch.Lines = jsonSketchLines;

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
    [JsonProperty("Name")]
    public string Name;

    [JsonProperty("Id")]
    public uint SketchID;

    [JsonProperty("Points")]
    public List<SketchPoint> Points;

    [JsonProperty("Lines")]
    public List<JsonSketchLine> Lines;

    [JsonProperty("Constraints")]
    public List<JsonConstraint> Constraints;

    // Auxilary
    public Sketch ToSketch()
    {
        Sketch sketch = new Sketch(SketchID);
        sketch.SetPoints(Points);

        List<SketchLine> sketchLines = new List<SketchLine>();
        foreach(JsonSketchLine line in Lines)
        {
            sketchLines.Add(line.ToSketchLine(Points));
        }
        sketch.SetLines(sketchLines);

        List<SketchConstraint> sketchConstraints = new List<SketchConstraint>();
        foreach(JsonConstraint jsonConstraint in Constraints)
        {
            sketchConstraints.Add(jsonConstraint.ToSketchConstraint(sketch.Points, sketch.Lines));
        }
        sketch.SetConstraints(sketchConstraints);

        return sketch;
    }
}