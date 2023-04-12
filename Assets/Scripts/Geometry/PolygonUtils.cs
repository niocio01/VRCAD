using System;
using System.Collections.Generic;
using System.Linq;
using Editors.SketchEdit;
using Unity.VisualScripting;
using UnityEngine;

namespace Geometry
{
    public enum WindingDir
    {
        Clockwise,
        CounterClockwise,
        None
    }
    
    public static class PolyUtils
    {
        public static WindingDir FindWindingDir(List<Vector2> vertices)
        {
            float totalArea = 0f;

            for (int i = 0; i < vertices.Count; i++)
            {
                Vector2 a = vertices[i];
                Vector2 b = vertices[(i + 1) % vertices.Count];

                float dy = (b.y + a.y) / 2f;
                float dx = b.x - a.x;

                float area = dy * dx;
                totalArea += area;
            }

            if (totalArea < 0) return WindingDir.CounterClockwise;
            if (totalArea > 0) return WindingDir.Clockwise;
            Debug.LogError("Winding Order could not be determined.");
            return WindingDir.None;
        }

        public static bool GenerateOutline(List<SketchLine> lines, out List<SketchLine> sortedLines, out List<Vector2> outlineVerts)
        {
            outlineVerts = null;
            sortedLines = null;
            if (lines.Count < 3)
            {
                Debug.LogError("At least 3 lines are required to form a loop.");
                return false;
            }

            // make copy of current list
            List<SketchLine> oldList = new List<SketchLine>(lines);

            // create new list and add Elements in the order of the path to speed up processing
            // when accessing function the next time
            List<SketchLine> newList = new List<SketchLine>();

            // hold onto the first line, since its used to check if the loop is actually closed
            SketchLine first = oldList.First();

            // remove first, since its not a valid choice
            oldList.Remove(first);

            SketchLine current = first;
            newList.Add(current);

            SketchLine next;

            int length = oldList.Count;

            for (int i = 0; i <= length; i++)
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

                // no connecting line Found
                // does the current list form a closed loop?
                if (current.Points[1] == first.Points[0])
                {
                    if (oldList.Count == 0)
                    {
                        sortedLines = newList;

                        outlineVerts = new List<Vector2>();
                        foreach (SketchLine sketchLine in lines)
                        {
                            outlineVerts.Add(sketchLine.Points[0].Position);
                        }

                        return true;
                    }

                    Debug.LogError("Found a closed loop, but there are still lines left");
                    return false;
                }

                // no matching line found
                Debug.LogError($"Was not able to find a Line that connects to {current.Points[1].Position.ToString()}");
                return false;
            }

            Debug.LogError("Lines do not form a closed loop.");
            return false;
        }

        public static bool Triangulate(ClosedShape closedShape, out List<int> triangles )
        {
            triangles = null;
            
            if (!closedShape.IsValid)
            {
                Debug.LogError("Provided Outline is invalid");
                return false;
            }
            
            /*
            if (!PolygonHelper.IsSimplePolygon(vertices))
            {
                errorMessage = "The vertex list does not define a simple polygon.";
                return false;
            }
    
            if (PolygonHelper.ContainsColinearEdges(vertices))
            {
                errorMessage = "The vertex list contains colinear edges.";
                return false;
            }    
            */

            // Make sure winding order is correct
            WindingDir order = closedShape.GetWindingDir();

            if (order == WindingDir.None) return false;

            if (order == WindingDir.CounterClockwise)
            {
                closedShape.Reverse();
            }

            triangles = new List<int>();
            int numVerts = closedShape.Vertices.Count();

            for ( int i = 0; i < numVerts - 2; i++ )
            {
                var earFound = false;
                foreach (LinkedVertex vertex in closedShape.Vertices)
                {
                    // is vertex Reflex?
                    if(vertex.IsReflex) continue;
                    
                    // are there any other vertexes inside it?
                    if (!closedShape.IsCornerEmpty(vertex)) continue;

                    earFound = true;

                    // Valid ear Found, so save the triangle and clip the ear.
                    triangles.Add(vertex.Prev.Index);
                    triangles.Add(vertex.Index);
                    triangles.Add(vertex.Next.Index);
                    
                    closedShape.Remove(vertex);
                    break;
                }

                if (earFound) continue;
                
                Debug.LogError("Unable to find Ear to clip.");
                return false;
            }

            return true;
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static float Cross2(Vector2 a, Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
        }

        public static Vector2 ProjectToPlane(Vector3 vector, Vector3 planeOrigin, Vector3 planeNormal)
        {
            // https://www.baeldung.com/cs/3d-point-2d-plane

            
            // TODO: make inline
            float a = planeNormal.x;
            float b = planeNormal.y;
            float c = planeNormal.z;

            float p = planeOrigin.x;
            float q = planeOrigin.y;
            float r = planeOrigin.z;

            float z1 = vector.x;
            float z2 = vector.y;
            float z3 = vector.z;

            // d = ap + bq + cr
            float d = a * p + b * q + c * r;

            float k = (d - a * z1 - b * z2 - c * z3) / (a * a + b * b + c * c);

            // get position of projection in original (3D) coordinate system
            float z1_ = (float)Math.Round(z1 + k * a, 4, MidpointRounding.AwayFromZero);
            float z2_ = (float)Math.Round(z2 + k * b, 4, MidpointRounding.AwayFromZero);
            // float z3_ = z2 + k * c;
            
            // assuming unit vectors of new (2D) coordinate system are the x and y of the provided plane normal.
            Vector2 projection = new Vector2(z1_ , z2_);
            return projection;

            
            // for custom unit vectors:
            // Vector3 e1 = new Vector3(1, 0, 0);
            // Vector3 e2 = new Vector3(0, 1, 0);
            //
            // Vector2 customProj =
            //     new Vector2(e1.x * z1_ + e1.y * z2_ + e1.z * z3_, e2.x * z1_ + e2.y * z2_ + e2.z * z3_);
        }

        public static Vector3 PlaneVecTo3D(Vector2 vector2, Pose pose)
        {
            Matrix4x4 mat =  Matrix4x4.TRS(pose.position, Quaternion.FromToRotation(pose.forward, new Vector3(0, 0,1)), Vector3.one);
            
            Vector3 vector = mat.MultiplyPoint3x4(new Vector3(vector2.x, vector2.y, 0));
            return vector;
        }

        public static bool IsVertexInsideCorner(LinkedVertex vertex, LinkedVertex corner)
        {
            // TODO: Inline each of these lines for performance?
            Vector2 ab = corner.Next.Pos - corner.Pos;
            Vector2 bc = corner.Prev.Pos - corner.Next.Pos;
            Vector2 ca = corner.Pos - corner.Prev.Pos;

            Vector2 ap = vertex.Pos - corner.Pos;
            Vector2 bp = vertex.Pos - corner.Next.Pos;
            Vector2 cp = vertex.Pos - corner.Prev.Pos;

            float c1 = Cross2(ab, ap);
            float c2 = Cross2(bc, bp);
            float c3 = Cross2(ca, cp);

            if (c1 < 0f && c2 < 0f && c3 < 0f)
            {
                return true;
            }

            return false;
        }
    }

    public static class VectExt
    {
        public static Vector3 Reverse(this Vector3 vector)
        {
            return new Vector3(-vector.x, -vector.y, -vector.z);
        }

        public static Vector2 Mirror(this Vector2 vector)
        {
            return new Vector2(-vector.x, -vector.y);
        }
    }
}