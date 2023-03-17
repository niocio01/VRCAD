using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public abstract class SketchElement
{
    [JsonProperty("id")]
    public uint ID { protected set; get; }

    public SketchElement(uint id)
    {
        ID = id;
    }
}

public class SketchPoint : SketchElement
{
    [JsonProperty("position")]
    public Vector2 Position { get; private set; }

    public SketchPoint(Vector2 position, uint id) : base(id)
    {
        Position = position;
    }
}

public class SketchLine : SketchElement
{
    [JsonProperty("pointIDs")]
    public SketchPoint[] Points { get; private set; }
    bool Construction = false;

    public SketchLine(SketchPoint first, SketchPoint second, uint id, bool constuction = false) : base(id)
    {
        Points = new SketchPoint[2] { first, second };
        Construction = constuction;
        ID = id;
    }
}

