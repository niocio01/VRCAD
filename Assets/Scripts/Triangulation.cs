using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Habrador_Computational_Geometry;


//public static class Triangulation
//{
//    public static bool Triangulate(Vector2[] vertices, out int[] triangles, out string errorMessage)
//    {
//        triangles = null;
//        errorMessage = string.Empty;

//        if (vertices is null)
//        {
//            errorMessage = "The vertex list is null.";
//            return false;
//        }

//        if (vertices.Length < 3)
//        {
//            errorMessage = "The vertex list must have at least 3 vertices.";
//            return false;
//        }

//        if (vertices.Length > 1024)
//        {
//            errorMessage = "The max vertex list length is 1024";
//            return false;
//        }

//        /*
//        if (!PolygonHelper.IsSimplePolygon(vertices))
//        {
//            errorMessage = "The vertex list does not define a simple polygon.";
//            return false;
//        }

//        if (PolygonHelper.ContainsColinearEdges(vertices))
//        {
//            errorMessage = "The vertex list contains colinear edges.";
//            return false;
//        }

//        */

//        WindingDir order = PolyUtils.FindWindingDir(vertices);

//        if (order == WindingDir.None)
//        {
//            errorMessage = "The vertices list does not contain a valid polygon.";
//            return false;
//        }

//        if (order == WindingDir.CounterClockwise)
//        {
//            Array.Reverse(vertices);
//        }

//        List<int> indexList = new List<int>();
//        for (int i = 0; i < vertices.Length; i++)
//        {
//            indexList.Add(i);
//        }

//        int totalTriangleCount = vertices.Length - 2;
//        int totalTriangleIndexCount = totalTriangleCount * 3;

//        triangles = new int[totalTriangleIndexCount];
//        int triangleIndexCount = 0;

//        while (indexList.Count > 3)
//        {
//            for (int i = 0; i < indexList.Count; i++)
//            {
//                int a = indexList[i];
//                int b = PolyUtils.GetItem(indexList, i - 1);
//                int c = PolyUtils.GetItem(indexList, i + 1);

//                Vector2 va = vertices[a];
//                Vector2 vb = vertices[b];
//                Vector2 vc = vertices[c];

//                Vector2 va_to_vb = vb - va;
//                Vector2 va_to_vc = vc - va;

//                // Is ear test vertex convex?
//                if (PolyUtils.Cross(va_to_vb, va_to_vc) < 0f)
//                {
//                    continue;
//                }

//                bool isEar = true;

//                // Does test ear contain any polygon vertices?
//                for (int j = 0; j < vertices.Length; j++)
//                {
//                    if (j == a || j == b || j == c)
//                    {
//                        continue;
//                    }

//                    Vector2 p = vertices[j];

//                    if (PolyUtils.IsPointInTriangle(p, vb, va, vc))
//                    {
//                        isEar = false;
//                        break;
//                    }
//                }

//                if (isEar)
//                {
//                    triangles[triangleIndexCount++] = b;
//                    triangles[triangleIndexCount++] = a;
//                    triangles[triangleIndexCount++] = c;

//                    indexList.RemoveAt(i);
//                    break;
//                }
//            }
//        }

//        triangles[triangleIndexCount++] = indexList[0];
//        triangles[triangleIndexCount++] = indexList[1];
//        triangles[triangleIndexCount++] = indexList[2];

//        return true;
//    }
//}

public enum WindingDir
{
    Clockwise,
    CounterClockwise,
    None
}

public static class PolyUtils
{
    public static float Cross(Vector2 a, Vector2 b)
    {
        // a · b = ax × bx + ay × by
        return a.x * b.x + a.y * b.y;
    }

    public static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        // TODO: Inline each of these lines for performance?
        Vector2 ab = b - a;
        Vector2 bc = c - b;
        Vector2 ca = a - c;

        Vector2 ap = p - a;
        Vector2 bp = p - b;
        Vector2 cp = p - c;

        float c1 = Cross(ab, ap);
        float c2 = Cross(bc, bp);
        float c3 = Cross(ca, cp);

        if (c1 < 0f && c2 < 0f && c3 < 0f)
        {
            return true;
        }

        return false;
    }

    public static WindingDir FindWindingDir(List<MyVector2> vertices)
    {
        float totalArea = 0f;

        for (int i = 0; i < vertices.Count; i++)

        {
            MyVector2 a = vertices[i];
            MyVector2 b = vertices[(i + 1) % vertices.Count];

            float dy = (a.x + b.y) / 2f;
            float dx = b.x - a.x;

            float area = dy * dx;
            totalArea += area;
        }

        if(totalArea > 0) return WindingDir.CounterClockwise;
        if (totalArea < 0) return WindingDir.Clockwise;
        return WindingDir.None;
    }

    public static T GetItem<T>(T[] array, int index)
    {
        if (index >= array.Length)
        {
            return array[index % array.Length];
        }
        else if (index < 0)
        {
            return array[index % array.Length + array.Length];
        }
        else
        {
            return array[index];
        }
    }

    public static T GetItem<T>(List<T> list, int index)
    {
        if (index >= list.Count)
        {
            return list[index % list.Count];
        }
        if (index < 0)
        {
            return list[index % list.Count + list.Count];
        }
        else
        {
            return list[index];
        }
    }
}
