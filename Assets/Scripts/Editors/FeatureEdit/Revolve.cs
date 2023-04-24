using System.Collections.Generic;
using Editors.SketchEdit;
using Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Editors.FeatureEdit
{
    public class Revolve : Feature
    {
        private Sketch BaseSketch { get; set; }
        private SketchElementReference Axis { get; set; }
    
        public Revolve(Sketch baseSketch, SketchLine axis, uint featureId) : base(featureId)
        {
            BaseSketch = baseSketch;
            Axis = new SketchElementReference("Line", baseSketch, axis);
        }

        public override bool ApplyFeature(ref MyMesh mesh)
        {
            throw new System.NotImplementedException();
        }

        public override JsonFeature ToJsonFeature()
        {
            return new JsonRevolve(BaseSketch.SketchID, Axis.ToJsonRef(), FeatureID);
        }
    }
    public class JsonRevolve : JsonFeature
    {
        [JsonProperty("BaseSketch", Order = 2)] private uint _baseSketchID;
        [JsonProperty("Axis", Order = 3)] private JsonSketchElementReference _reference;
        public JsonRevolve(uint baseSketchID, JsonSketchElementReference reference, uint featureID) : base("Revolve", featureID)
        {
            _baseSketchID = baseSketchID;
            _reference = reference;
        }

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
}