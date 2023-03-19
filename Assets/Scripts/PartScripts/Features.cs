using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public abstract class Feature
{
    // Properties
    public virtual uint FeatureID { protected set; get; }

    // Base Constructor
    public Feature(uint id)
    {
        FeatureID = id;
    }

    // Auxilary
    public abstract JsonFeature ToJsonFeature();
}

public abstract class JsonFeature
{
    // Properties
    [JsonProperty("Id", Order = -1)]
    public virtual uint FeatureID { protected set; get; }

    [JsonProperty("Type", Order = -2)]
    public virtual string Type { protected set; get; }

    // Base Constructor
    public JsonFeature(string type, uint  id)
    {
        Type = type;
        FeatureID = id;
    }
    public abstract Feature ToFeature(List<Sketch> sketches);
}

public class Extrude : Feature
{
    // Properties
    public Sketch BaseSketch { get; private set; }
    public float ExtrusionHeight { get; private set; }

    // Constructor
    public Extrude(Sketch baseSketch, float height, uint featureID) : base(featureID) {
        
        BaseSketch = baseSketch;
        ExtrusionHeight = height;
    }

    // Auxilary
    public override JsonFeature ToJsonFeature()
    {
        return new JsonExtrude(ExtrusionHeight, BaseSketch.SketchID, FeatureID); ;
    }
}
public class JsonExtrude : JsonFeature
{
    // Properties
    [JsonProperty("BaseSketch", Order = 2)]
    uint BaseSketchID;

    [JsonProperty("ExtrusionHeight", Order = 3)]
    float ExtrusionHeight;

    // Constructor
    public JsonExtrude(float extrusionHeight, uint basesketchID, uint featureID) : base("Extrude", featureID)
    {
        BaseSketchID = basesketchID;
        ExtrusionHeight = extrusionHeight;
    }

    // Auxilary
    public override Feature ToFeature(List<Sketch> sketches)
    {
        return new Extrude(sketches.Find(s => s.SketchID == BaseSketchID), ExtrusionHeight, FeatureID); ;
    }
    public static JsonExtrude Deserialize(JObject jsonObject)
    {
        uint featureId = jsonObject["Id"].Value<uint>();
        uint sketchId = jsonObject["BaseSketch"].Value<uint>();
        float height = jsonObject["ExtrusionHeight"].Value<float>();
        return new JsonExtrude(height, sketchId, featureId);
    }
}


public class Revolve : Feature
{
    // Properties
    public Sketch BaseSketch { get; private set; }
    public SketchElementReference Axis { get; private set; }

    // Constructor
    public Revolve(Sketch baseSketch, SketchLine axis, uint featureId) : base(featureId)
    {
        BaseSketch = baseSketch;
        Axis = new SketchElementReference("Line", baseSketch, axis);
    }

    // Auxilary
    public override JsonFeature ToJsonFeature()
    {
        return new JsonRevolve(BaseSketch.SketchID, Axis.ToJsonRef(), FeatureID);
    }
}
public class JsonRevolve : JsonFeature
{
    // Properties
    [JsonProperty("BaseSketch", Order = 2)]
    uint BaseSketchID;

    [JsonProperty("Axis", Order = 3)]
    JsonSketchElementReference Reference;

    // Constructor
    public JsonRevolve(uint basesketchID, JsonSketchElementReference reference, uint featureID) : base("Revolve", featureID)
    {
        BaseSketchID = basesketchID;
        Reference = reference;
    }

    // Auxilary
    public override Feature ToFeature(List<Sketch> sketches)
    {
        SketchElementReference reference = Reference.ToSketchRef(sketches);        
        return new Revolve(reference.Sketch, (SketchLine)reference.Reference, FeatureID);
    }

    public static JsonRevolve Deserialize(JObject jsonObject)
    {
        uint featureId = jsonObject["Id"].Value<uint>();
        uint sketchId = jsonObject["BaseSketch"].Value<uint>();
        JsonSketchElementReference reference = JsonSketchElementReference.Deserialize((JObject)jsonObject["Axis"]);
        return new JsonRevolve(sketchId, reference, featureId);
    }
}
