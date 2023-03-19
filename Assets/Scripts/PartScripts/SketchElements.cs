using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public abstract class SketchElement
{
    [JsonProperty("Id")]
    public uint ID { protected set; get; }

    public SketchElement(uint id)
    {
        ID = id;
    }
}

public class SketchPoint : SketchElement
{
    [JsonProperty("Position")]
    public Vector2 Position { get; private set; }

    public SketchPoint(Vector2 position, uint id) : base(id)
    {
        Position = position;
    }
}

public class SketchLine : SketchElement
{
    [JsonProperty("PointIDs")]
    public SketchPoint[] Points { get; private set; }
    bool Construction = false;

    public SketchLine(SketchPoint first, SketchPoint second, uint id, bool constuction = false) : base(id)
    {
        Points = new SketchPoint[2] { first, second };
        Construction = constuction;
        ID = id;
    }
}

public class SketchElementReference
{
    public string Type { get; private set; }
    public Sketch Sketch { get; private set; }
    public SketchElement Reference { get; private set; }

    public SketchElementReference(string type, Sketch sketch, SketchElement reference)
    {
        Type = type;
        Sketch = sketch;
        Reference = reference;
    }

    public JsonSketchElementReference ToJsonRef() 
    {
        return new JsonSketchElementReference(Type, Sketch.SketchID, Reference.ID);
    }
}

public class JsonSketchElementReference
{
    [JsonProperty("Type")]
    public string Type { get; private set; }

    [JsonProperty("SketchId")]
    public uint SketchID { get; private set; }

    [JsonProperty("ElementId")]
    public uint ElementID { get; private set; }

    public JsonSketchElementReference(string type, uint sketchID, uint elementID)
    {
        Type = type;
        SketchID = sketchID;
        ElementID = elementID;
    }

    public SketchElementReference ToSketchRef(List<Sketch> sketches)
    {
        Sketch sketch = sketches.Find(s => s.SketchID == SketchID);

        SketchElement element = null;
        switch (Type)
        {
            case "Line" : element = sketch.GetLine(ElementID); break;
            case "Point": element = sketch.GetPoint(ElementID); break;
        }

        return new SketchElementReference(Type, sketch, element);
    }

    public static JsonSketchElementReference Deserialize(JObject jsonObject)
    {
        string type = jsonObject["Type"].Value<string>();
        uint sketchId = jsonObject["SketchId"].Value<uint>();
        uint elementID = jsonObject["ElementId"].Value<uint>();

        return new JsonSketchElementReference(type, sketchId, elementID);
    }
}

