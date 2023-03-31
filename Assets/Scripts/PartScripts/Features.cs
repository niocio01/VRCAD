using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public abstract class Feature
{
    // Properties
    protected virtual uint FeatureID { set; get; }

    // Base Constructor
    protected Feature(uint id)
    {
        FeatureID = id;
    }

    // Auxiliary
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
    protected JsonFeature(string type, uint  id)
    {
        Type = type;
        FeatureID = id;
    }
    public abstract Feature ToFeature(List<Sketch> sketches);
}

public class Extrude : Feature
{
    // Properties
    private Sketch BaseSketch { get; set; }
    private float ExtrusionHeight { get; set; }

    // Constructor
    public Extrude(Sketch baseSketch, float height, uint featureID) : base(featureID) {
        
        BaseSketch = baseSketch;
        ExtrusionHeight = height;
    }

    // Auxiliary
    public override JsonFeature ToJsonFeature()
    {
        return new JsonExtrude(ExtrusionHeight, BaseSketch.SketchID, FeatureID);
    }
}
public class JsonExtrude : JsonFeature
{
    // Properties
    [JsonProperty("BaseSketch", Order = 2)]
    private uint _baseSketchID;

    [JsonProperty("ExtrusionHeight", Order = 3)]
    private float _extrusionHeight;

    // Constructor
    public JsonExtrude(float extrusionHeight, uint baseSketchID, uint featureID) : base("Extrude", featureID)
    {
        _baseSketchID = baseSketchID;
        _extrusionHeight = extrusionHeight;
    }

    // Auxiliary
    public override Feature ToFeature(List<Sketch> sketches)
    {
        return new Extrude(sketches.Find(s => s.SketchID == _baseSketchID), _extrusionHeight, FeatureID);
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
    private Sketch BaseSketch { get; set; }
    private SketchElementReference Axis { get; set; }

    // Constructor
    public Revolve(Sketch baseSketch, SketchLine axis, uint featureId) : base(featureId)
    {
        BaseSketch = baseSketch;
        Axis = new SketchElementReference("Line", baseSketch, axis);
    }

    // Auxiliary
    public override JsonFeature ToJsonFeature()
    {
        return new JsonRevolve(BaseSketch.SketchID, Axis.ToJsonRef(), FeatureID);
    }
}
public class JsonRevolve : JsonFeature
{
    // Properties
    [JsonProperty("BaseSketch", Order = 2)]
    private uint _baseSketchID;

    [JsonProperty("Axis", Order = 3)]
    JsonSketchElementReference _reference;

    // Constructor
    public JsonRevolve(uint baseSketchID, JsonSketchElementReference reference, uint featureID) : base("Revolve", featureID)
    {
        _baseSketchID = baseSketchID;
        _reference = reference;
    }

    // Auxiliary
    public override Feature ToFeature(List<Sketch> sketches)
    {
        SketchElementReference reference = _reference.ToSketchRef(sketches);        
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
