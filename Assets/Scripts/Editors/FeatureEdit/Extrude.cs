using System.Collections.Generic;
using Editors.SketchEdit;
using Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Editors.FeatureEdit
{
    public class Extrude : Feature
    {
        private Sketch BaseSketch { get; set; }
        private float ExtrusionHeight { get; set; }
        public Extrude(Sketch baseSketch, float height, uint featureID) : base(featureID)
        {
            BaseSketch = baseSketch;
            ExtrusionHeight = height;
        }

        public override bool ApplyFeature(ref MyMesh mesh)
        {
            return MeshOperations.Extrude(BaseSketch.Face, new Vector3(0, 0, ExtrusionHeight), ref mesh);
        }
        
        public override JsonFeature ToJsonFeature()
        {
            return new JsonExtrude(ExtrusionHeight, BaseSketch.SketchID, FeatureID);
        }
    }
    
    public class JsonExtrude : JsonFeature
    {
        [JsonProperty("BaseSketch", Order = 2)] private uint _baseSketchID;

        [JsonProperty("ExtrusionHeight", Order = 3)] private float _extrusionHeight;
        public JsonExtrude(float extrusionHeight, uint baseSketchID, uint featureID) : base("Extrude", featureID)
        {
            _baseSketchID = baseSketchID;
            _extrusionHeight = extrusionHeight;
        }
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
}