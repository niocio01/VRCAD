using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editors.FeatureEdit;
using Editors.SketchEdit;
using Newtonsoft.Json;

namespace Editors.PartEdit
{
    public class Part
    {
        // Properties
        public PartInfo PartInfo { get; private set; }
        public List<Sketch> Sketches { get; private set; }
        public List<Feature> Features {get; private set;}

        // Counters
        public uint SketchIdCounter { get; private set; }
        public uint FeatureIdCounter { get; private set; }

        // Constructors
        public Part() 
        {
            SketchIdCounter = 0;
            FeatureIdCounter = 0;

            Features = new List<Feature>();
            Sketches = new List<Sketch>();        
        }
        public Part(string title, string description, string author) : this()
        {
            PartInfo = new PartInfo(title, description, author);
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

        public void UpdateLastChanged()
        {
            PartInfo.LastEdit = DateTime.Now;
        }
        public void Save(String path)
        {
            JsonHandler.JsonSave(this, path);
        }

        // Auxiliary
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
        [JsonProperty("Meta")] public PartInfo PartInfo { get; set; }
        [JsonProperty("Data")] public JsonData Data { get; set; } = new JsonData();

        // Auxiliary
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
        [JsonProperty("Sketches")] public List<JsonSketch> Sketches { get; set; } = new List<JsonSketch>();
        [JsonProperty("Features")] public List<JsonFeature> Features { get; set; } = new List<JsonFeature>();
    }
    public class PartInfo
    {
        [JsonProperty("Title")] public string Title { get; private set; }
        [JsonProperty("Description")] public string Description { get; private set; }
        [JsonProperty("Author")] public string Author { get; private set; }
        [JsonProperty("Creation")] public DateTime Creation { get; private set; }
        [JsonProperty("LastEdit")] public DateTime LastEdit { get; set; }

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
}