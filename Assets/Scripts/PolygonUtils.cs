using System.Collections.Generic;
using Habrador_Computational_Geometry;

public enum WindingDir
{
    Clockwise,
    CounterClockwise,
    None
}

public static class PolyUtils
{
    public static WindingDir FindWindingDir(List<MyVector2> vertices)
    {
        float totalArea = 0f;

        for (int i = 0; i < vertices.Count; i++)
        {
            MyVector2 a = vertices[i];
            MyVector2 b = vertices[(i + 1) % vertices.Count];

            float dy = (b.y + a.y) / 2f;
            float dx = b.x - a.x;

            float area = dy * dx;
            totalArea += area;
        }

        if (totalArea < 0) return WindingDir.CounterClockwise;
        if (totalArea > 0) return WindingDir.Clockwise;
        return WindingDir.None;
    }
}
