using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Feature
{
    public uint FeatureID { protected set; get; }

    // Base Constructor
    public Feature(uint id)
    {
        FeatureID = id;
    }
    
    public abstract List<Sketch> GetAllRefSketches();

    public abstract JsonFeature feature2Jsonfeature();
}

public abstract class JsonFeature
{
    [JsonProperty("id", Order = -1)]
    public uint FeatureID { protected set; get; }

    [JsonProperty("type", Order = -2)]
    public readonly string Name;

    // Base Constructor
    public JsonFeature(string name, uint  id)
    {
        Name = name;
        FeatureID = id;
    }

    public abstract Feature JsonFeature2Feature();
}

public class Extrude : Feature
{
    public Sketch BaseSketch { get; private set; }
    public float ExtrusionHeight { get; private set; }

    public Extrude(Sketch sketch, int height, uint id) : base(id) {
        
        BaseSketch = sketch;
        ExtrusionHeight = height;
    }

    public override List<Sketch> GetAllRefSketches()
    {
        return new List<Sketch> { BaseSketch };
    }

    public override JsonFeature feature2Jsonfeature()
    {
        Extrude extrude = this;
        Extrude_Json jsonFeature = new Extrude_Json(extrude.ExtrusionHeight, extrude.BaseSketch.SketchID, extrude.FeatureID);

        return jsonFeature;
    }
}

public class Extrude_Json : JsonFeature
{
    [JsonProperty("baseSketch", Order = 2)]
    uint BaseSketchID;

    [JsonProperty("extrusionHeight", Order = 3)]
    float ExtrusionHeight;

    public Extrude_Json(float extrusionHeight, uint basesketchID, uint featureID) : base("extrude", featureID)
    {
        BaseSketchID = basesketchID;
        ExtrusionHeight = extrusionHeight;
    }

    public override Feature JsonFeature2Feature()
    {
        throw new System.NotImplementedException();
    }
}
