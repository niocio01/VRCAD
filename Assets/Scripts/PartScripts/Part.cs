using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part
{
    // Properties
    public PartInfo PartInfo { get; private set; }
    public List<Sketch> Sketches { get; private set; }
    public List<Feature> Features {get; private set;}

    // Counters
    public uint SketchIdCounter { get; private set; } = 0;
    public uint FeatureIdCounter { get; private set; } = 0;

    // Constructors
    public Part() 
    {
        Features = new List<Feature>();
        Sketches = new List<Sketch>();
    }
    public Part(string title, string description, string author)
    {
        PartInfo = new PartInfo(title, description, author);
        Features = new List<Feature>();
        Sketches = new List<Sketch>();
    }    
    public Part(PartInfo partInfo)
    {
        PartInfo = partInfo;
        Features = new List<Feature>();
    }

    // Add Elements
    public void AddPartInfo(PartInfo partInfo)
    { 
        PartInfo = partInfo;
    }
    public void AddFeature(Feature feature)
    {
        Features.Add(feature);
        FeatureIdCounter++;
    }
    public void AddSketch(Sketch sketch)
    {
        Sketches.Add(sketch);
        SketchIdCounter++;
    }

    // Auxilary
    public JsonPart ToJsonPart()
    {
        JsonPart jsonPart = new JsonPart();
        jsonPart.PartInfo = this.PartInfo;

        List<JsonSketch> jsonSketches = new List<JsonSketch>();
        foreach (Sketch sketch in Sketches)
        {
            jsonSketches.Add(sketch.ToJsonSketch());
        }
        jsonPart.Data.Sketches = jsonSketches;

        List<JsonFeature> jsonFeatures = new List<JsonFeature>();
        foreach (Feature feature in Features)
        {
            jsonFeatures.Add(feature.ToJsonFeature());
        }
        jsonPart.Data.Features = jsonFeatures;

        return jsonPart;
    }
}
public class JsonPart
{
    [JsonProperty("Meta")]
    public PartInfo PartInfo { get; set; }

    [JsonProperty("Data")]
    public JsonData Data { get; set; } = new JsonData();

    // Auxilary
    public Part ToPart()
    {
        Part part = new Part();
        part.AddPartInfo(PartInfo);

        foreach(JsonSketch jsonSketch in Data.Sketches)
        {
            part.AddSketch(jsonSketch.ToSketch());
        }

        foreach (JsonFeature jsonFeature in Data.Features)
        {
            part.AddFeature(jsonFeature.ToFeature(part.Sketches));
        }

        return part;
    }

}
public class JsonData
{
    [JsonProperty("Sketches")]
    public List<JsonSketch> Sketches { get; set; } = new List<JsonSketch>();

    [JsonProperty("Features")]
    public List<JsonFeature> Features { get; set; } = new List<JsonFeature>();
}
public class PartInfo
{
    [JsonProperty("Title")]
    public string Title { get; private set; }

    [JsonProperty("Description")]
    public string Description { get; private set; }

    [JsonProperty("Author")]
    public string Author { get; private set; }

    [JsonProperty("Creation")]
    public DateTime Creation { get; private set; }

    [JsonProperty("LastEdit")]
    public DateTime LastEdit { get; private set; }

    // Constructor
    public PartInfo(string title, string description, string author)
    {
        Title = title;
        Description = description;
        Author = author;
        Creation = DateTime.Now;
        LastEdit = DateTime.Now;
    }
}

