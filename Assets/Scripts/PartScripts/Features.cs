using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonSubTypes;

public abstract class Feature
{
    // Properties
    public uint FeatureID { protected set; get; }

    // Base Constructor
    public Feature(uint id)
    {
        FeatureID = id;
    }

    // Auxilary
    public abstract JsonFeature ToJsonFeature();
}

[JsonConverter(typeof(JsonFeature2JsonConverter))]
public abstract class JsonFeature
{
    // Properties
    [JsonProperty("id", Order = -1)]
    public uint FeatureID { protected set; get; }

    [JsonProperty("type", Order = -2)]
    public readonly string Type;

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
    public Extrude(Sketch sketch, float height, uint id) : base(id) {
        
        BaseSketch = sketch;
        ExtrusionHeight = height;
    }

    // Auxilary
    public override JsonFeature ToJsonFeature()
    {
        Extrude extrude = this;
        JsonExtrude jsonFeature = new JsonExtrude(extrude.ExtrusionHeight, extrude.BaseSketch.SketchID, extrude.FeatureID);

        return jsonFeature;
    }
}
public class JsonExtrude : JsonFeature
{
    // Properties
    [JsonProperty("baseSketch", Order = 2)]
    uint BaseSketchID;

    [JsonProperty("extrusionHeight", Order = 3)]
    float ExtrusionHeight;

    // Constructor
    public JsonExtrude(float extrusionHeight, uint basesketchID, uint featureID) : base("extrude", featureID)
    {
        BaseSketchID = basesketchID;
        ExtrusionHeight = extrusionHeight;
    }

    // Auxilary
    public override Feature ToFeature(List<Sketch> sketches)
    {
        Extrude extrude = new Extrude(sketches.Find(s => s.SketchID == BaseSketchID), ExtrusionHeight, FeatureID);
        return extrude;
    }
}
