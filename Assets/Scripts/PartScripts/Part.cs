using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part
{
    public PartInfo PartInfo { get; private set; }

    public List<Sketch> Sketches { get; private set; }

    public List<Feature> Features {get; private set;}

    public uint SketchIdCounter { get; private set; } = 0;
    public uint FeatureIdCounter { get; private set; } = 0;

    // Constructors
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

    public void AddFeature(Feature feature)
    {
        Features.Add(feature);
    }

    public Sketch AddSketch()
    {
        Sketch sketch = new Sketch(SketchIdCounter);
        SketchIdCounter++;
        Sketches.Add(sketch);
        return sketch;
    }
}

public class PartInfo
{
    [JsonProperty("title")]
    public string Title { get; private set; }

    [JsonProperty("description")]
    public string Description { get; private set; }

    [JsonProperty("author")]
    public string Author { get; private set; }

    [JsonProperty("creation")]
    public DateTime Creation { get; private set; }

    [JsonProperty("last_edit")]
    public DateTime LastEdit { get; private set; }

    public PartInfo(string title, string description, string author)
    {
        Title = title;
        Description = description;
        Author = author;
        Creation = DateTime.Now;
        LastEdit = DateTime.Now;
    }
}
