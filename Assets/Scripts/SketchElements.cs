using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SketchElement
{
    public int ID { protected set; get; }

    public SketchElement(int id)
    {
        ID = id;
    }
}

public class SketchPoint : SketchElement
{
    public Vector2 Position { get; private set; }

    public SketchPoint(Vector2 position, int id) : base(id)
    {
        Position = position;
    }
}

public class SketchLine : SketchElement
{
    public SketchPoint[] Points { get; private set; }
    bool Construction = false;

    public SketchLine(SketchPoint first, SketchPoint second, int id) : base(id)
    {
        Points = new SketchPoint[2] { first, second };
        Construction = false;
        ID = id;
    }

    public SketchLine(SketchPoint first, SketchPoint second, bool construction, int id) : this(first, second, id)
    {
        Construction = construction;
    }
}

