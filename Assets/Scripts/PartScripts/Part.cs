using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part
{
    public PartInfo PartInfo { get; private set; }

    public List<Feature> Features {get; private set;}

    // Constructors
    public Part(string title, string description, string author)
    {
        PartInfo = new PartInfo(title, description, author);
        Features = new List<Feature>();
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

    public static JsonPart Part2JsonPart (Part part)
    {
        List<Sketch> allSketches = new List<Sketch>();
        JsonPart jsonPart = new JsonPart();


        foreach (Feature feature in part.Features)
        {
            // convert normal feature to Json equivalent and add it to the list of features
            jsonPart.JsonFeatures.Add(feature.feature2Jsonfeature(feature));

            // go thru the feature and and add all the references sketch id's
            List<Sketch> sketches = feature.GetAllRefSketches();
            foreach (Sketch sketch in sketches)
            {
                if (!allSketches.Contains(sketch))
                {
                    allSketches.Add(sketch);
                }
            }
        }

        jsonPart.PartInfo = part.PartInfo;
        jsonPart.Sketches = allSketches;

        return jsonPart;
    }
}

public class JsonPart
{
    public PartInfo PartInfo;

    public List<Sketch> Sketches;

    public List<JsonFeature> JsonFeatures = new List<JsonFeature>();
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
