using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Habrador_Computational_Geometry;

public class Sketch
{
    public static event Action OnPointAdded;
    public static event Action OnLineAdded;

    // Properties
    public string Name { get; private set; }
    public uint SketchID { get; private set; }
    public List<SketchPoint> Points { get; private set; }
    public List<SketchLine> Lines { get; private set; }
    public List<SketchLine> ConstructionLines { get; private set; }
    public List<SketchConstraint> Constraints { get; private set; }
    public Transform Transform { get; private set; }

    // Counters
    public uint PointIdCounter { get; private set; } = 0;
    public uint LineIdCounter { get; private set; } = 0;
    public uint ConstraintIdCounter { get; private set; } = 0;

    // Geometry
    public List<MyVector2> EdgeVertices { get; private set; }

    public HashSet<Triangle2> Triangulation { get; private set; }

    // Constructor
    public Sketch(uint id, string name = "")
    {
        if (name == "") Name = "Sketch_" + id.ToString();
        else Name = name;

        SketchID = id;
        Points = new List<SketchPoint>();
        Lines = new List<SketchLine>();
        ConstructionLines = new List<SketchLine>();
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
        // check if point on this position allready exists,
        // if so, return that point insted of creationg a new one
        foreach (SketchPoint point in Points)
        {
            if (point.Position.x == x && point.Position.y == y)
            {
                return point;
            }
        }

        SketchPoint newPoint = new SketchPoint(new Vector2(x, y), PointIdCounter);

        Points.Add(newPoint);
        PointIdCounter++;

        OnPointAdded?.Invoke();
        return newPoint;
    }
    public SketchLine AddLine(uint firstID, uint secondID, bool construction = false) 
    {
        if (firstID == secondID) return null;

        SketchPoint first = Points.Find(x => x.ID == firstID);
        if (first == null) return null;

        SketchPoint second = Points.Find(x => x.ID == secondID);
        if (first == null) return null;

        SketchLine line = new SketchLine(first, second, LineIdCounter);
        if(construction)
        {
            ConstructionLines.Add(line);
        }
        else
        {
            Lines.Add(line);
        }        
        LineIdCounter++;

        OnLineAdded?.Invoke();

        return line;
    }
    public SketchLine AddLine(SketchPoint first, SketchPoint second, bool construction = false)
    {
        if (first == second) return null;
        if (first == null) return null;
        if (first == null) return null;

        SketchLine line = new SketchLine(first, second, LineIdCounter);
        if (construction)
        {
            ConstructionLines.Add(line);
        }
        else
        {
            Lines.Add(line);
        }
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
    public void SetLines(List<SketchLine> lines, bool construction = false)
    {
        if (construction)
        {
            ConstructionLines = lines;
        }
        else
        {
            Lines = lines;
        }

        if (Lines.Count > 0)
        {
            LineIdCounter = Math.Max( Lines.Max(l => l.ID) , LineIdCounter) + 1;
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

    /// <summary>
    /// checks if the SketchLines form a closed loop.
    /// Also reorders the list and flips reversed lines
    /// </summary>
    /// <returns>true, if closed</returns>
    public bool HullIsClosed()
    {

        // create new list and add Elements in the order of the path to speed up processing
        // when accessing function the next time
        List<SketchLine> newList = new List<SketchLine>();

        // make copy of current list, and only override it if all went smoothly...
        List<SketchLine> oldList = new List<SketchLine>(Lines);

        SketchLine first = oldList.First();

        // remove first, since its not a valid choice
        oldList.Remove(first);

        SketchLine current = first;
        newList.Add(current);

        SketchLine next = null;

        int length = oldList.Count;

        for (int i = 0; i < length; i++)
        {
            next = oldList.Find(line => line.Points[0] == current.Points[1]);
            if (next != null)
            {
                // remove from list to speed up searching...
                oldList.Remove(next);
                newList.Add(next);
                current = next; 
                continue;
            }

            next = oldList.Find(l => l.Points[1] == current.Points[1]);
            if (next != null)
            {
                // remove from list to speed up searching...
                oldList.Remove(next);
                // reverse the line
                newList.Add(new SketchLine(next.Points[1], next.Points[0], next.ID));
                current = next;
                continue;
            }

            // no matching line found
            return false;
        }

        if (newList.Last().Points[1] == first.Points[0])
        {
            Lines = newList;
            return true;
        }  
        return false;
    }
    public bool UpdateVertices()
    {
        EdgeVertices = new List<MyVector2>();

        foreach (SketchLine line in Lines)
        {
            EdgeVertices.Add(new MyVector2(line.Points[0].Position));
        }

        WindingDir order = PolyUtils.FindWindingDir(EdgeVertices);

        if (order == WindingDir.None)
        {
            Debug.Log($"Winding Order could not be determied.");
            return false;
        }

        if (order == WindingDir.Clockwise)
        {
            EdgeVertices.Reverse();
        }

        return true;
    }
    public bool UpdateTriangles()
    {
        //Triangulate
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        timer.Start();


        Triangulation = _EarClipping.Triangulate(EdgeVertices, null, optimizeTriangles: false);

        timer.Stop();
        Debug.Log($"Number of triangles from ear clipping: {Triangulation.Count}");
        Debug.Log($"Generated an Ear Clipping triangulation in {timer.ElapsedMilliseconds / 1000f} seconds");

        return true;
    }
    public JsonSketch ToJsonSketch()
    {
        JsonSketch jsonSketch = new JsonSketch();
        jsonSketch.Name = Name;
        jsonSketch.SketchID = SketchID;
        jsonSketch.Points = Points;

        List<JsonSketchLine> jsonSketchLines = new List<JsonSketchLine>();
        foreach (SketchLine line in Lines)
        {
            jsonSketchLines.Add(line.ToJsonLine(false));
        }
       
        foreach (SketchLine Cline in ConstructionLines)
        {
            jsonSketchLines.Add(Cline.ToJsonLine(true));
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
        List<SketchLine> C_sketchLines = new List<SketchLine>();
        foreach (JsonSketchLine jsonLine in Lines)
        {
            SketchLine line = jsonLine.ToSketchLine(Points, out bool construction);

            if (construction)
            {
                C_sketchLines.Add(line);
            }
            else
            {                
                sketchLines.Add(line);
            }

        }

        sketch.SetLines(sketchLines);
        sketch.SetLines(C_sketchLines, true);

        List<SketchConstraint> sketchConstraints = new List<SketchConstraint>();
        foreach(JsonConstraint jsonConstraint in Constraints)
        {
            sketchConstraints.Add(jsonConstraint.ToSketchConstraint(sketch.Points, sketch.Lines));
        }
        sketch.SetConstraints(sketchConstraints);

        return sketch;
    }
}