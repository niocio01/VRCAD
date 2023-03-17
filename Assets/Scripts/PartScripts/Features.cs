using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Feature
{
    public int FeatureID { protected set; get; }

    // Base Constructor
    public Feature(int id)
    {
        FeatureID = id;
    }
    
    public abstract List<Sketch> GetAllRefSketches();

    public abstract JsonFeature feature2Jsonfeature(Feature feature);
}

public abstract class JsonFeature
{
    [JsonProperty("id", Order = -1)]
    public int FeatureID { protected set; get; }

    [JsonProperty("type", Order = -2)]
    public readonly string Name;

    // Base Constructor
    public JsonFeature(string name, int id)
    {
        Name = name;
        FeatureID = id;
    }

    public abstract Feature JsonFeature2Feature(JsonFeature jsonFeature);
}

public class Extrude : Feature
{
    public Sketch BaseSketch { get; private set; }
    public float ExtrusionHeight { get; private set; }

    public Extrude(Sketch sketch, int height, int id) : base(id) {
        
        BaseSketch = sketch;
        ExtrusionHeight = height;
    }

    public override List<Sketch> GetAllRefSketches()
    {
        return new List<Sketch> { BaseSketch };
    }

    public override JsonFeature feature2Jsonfeature(Feature feature)
    {
        Extrude extrude = (Extrude)feature;
        Extrude_Json jsonFeature = new Extrude_Json(extrude.ExtrusionHeight, extrude.BaseSketch.SketchID, extrude.FeatureID);

        return jsonFeature;
    }
}

public class Extrude_Json : JsonFeature
{
    [JsonProperty("baseSketch", Order = 2)]
    int BaseSketchID;

    [JsonProperty("extrusionHeight", Order = 3)]
    float ExtrusionHeight;

    public Extrude_Json(float extrusionHeight, int basesketchID, int featureID) : base("extrude", featureID)
    {
        BaseSketchID = basesketchID;
        ExtrusionHeight = extrusionHeight;
    }

    public override Feature JsonFeature2Feature(JsonFeature jsonFeature)
    {
        throw new System.NotImplementedException();
    }
}
