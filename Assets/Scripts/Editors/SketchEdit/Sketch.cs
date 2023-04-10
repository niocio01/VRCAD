using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Habrador_Computational_Geometry;
using Newtonsoft.Json;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Editors.SketchEdit
{
    public class Sketch
    {
        // Events
        public static event Action OnPointAdded;
        public static event Action OnLineAdded;

        // general props
        public string Name { get; private set; }
        public uint SketchID { get; private set; }
        public List<SketchPoint> Points { get; private set; }
        public List<SketchLine> Lines { get; private set; }
        public List<SketchLine> ConstructionLines { get; private set; }
        public List<SketchConstraint> Constraints { get; private set; }

        // Geometry
        public Face Face { get; private set; }

        // Counters
        public uint PointIdCounter { get; private set; }
        public uint LineIdCounter { get; private set; }
        public uint ConstraintIdCounter { get; private set; }

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
            // check if point on this position already exists,
            // if so, return that point instead of creation a new one
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
            if (second == null) return null;

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
            if (second == null) return null;

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

        // Auxiliary
        public bool GenerateFace()
        {
            PolyUtils.GenerateOutline(Lines, out List<SketchLine> sortedLines, out List<Vector2> outlineVerts);
            Lines = sortedLines;
            
            // Make sure winding order is correct
            WindingDir order = PolyUtils.FindWindingDir(outlineVerts);

            if (order == WindingDir.None) return false;

            if (order == WindingDir.CounterClockwise)
            {
                Lines.Reverse();
                outlineVerts.Reverse();
            }

            //Triangulate
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            ClosedShape outline = new ClosedShape(outlineVerts);
            if (PolyUtils.Triangulate(outline, out List<int> triangles))
            {
                Face = new Face(outlineVerts, new Vector3(0, 0, 1), triangles);
            }

            timer.Stop();
            Debug.Log($"Generated an Ear Clipping triangulation in {timer.ElapsedMilliseconds / 1000f} seconds");

            return true;
        }
        public JsonSketch ToJsonSketch()
        {
            JsonSketch jsonSketch = new JsonSketch
            {
                Name = Name,
                SketchID = SketchID,
                Points = Points
            };

            List<JsonSketchLine> jsonSketchLines = new List<JsonSketchLine>();
            foreach (SketchLine line in Lines)
            {
                jsonSketchLines.Add(line.ToJsonLine(false));
            }
       
            foreach (SketchLine cline in ConstructionLines)
            {
                jsonSketchLines.Add(cline.ToJsonLine(true));
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
        [JsonProperty("Name")] public string Name;
        [JsonProperty("Id")] public uint SketchID;
        [JsonProperty("Points")] public List<SketchPoint> Points;
        [JsonProperty("Lines")] public List<JsonSketchLine> Lines;
        [JsonProperty("Constraints")] public List<JsonConstraint> Constraints;

        // Auxiliary
        public Sketch ToSketch()
        {
            Sketch sketch = new Sketch(SketchID);
            sketch.SetPoints(Points);

            List<SketchLine> sketchLines = new List<SketchLine>();
            List<SketchLine> cSketchLines = new List<SketchLine>();
            foreach (JsonSketchLine jsonLine in Lines)
            {
                SketchLine line = jsonLine.ToSketchLine(Points, out bool construction);

                if (construction)
                {
                    cSketchLines.Add(line);
                }
                else
                {                
                    sketchLines.Add(line);
                }

            }

            sketch.SetLines(sketchLines);
            sketch.SetLines(cSketchLines, true);

            List<SketchConstraint> sketchConstraints = new List<SketchConstraint>();
            foreach(JsonConstraint jsonConstraint in Constraints)
            {
                sketchConstraints.Add(jsonConstraint.ToSketchConstraint(sketch.Points, sketch.Lines));
            }
            sketch.SetConstraints(sketchConstraints);

            return sketch;
        }
    }
}