using System.Collections.Generic;
using Editors.SketchEdit;
using Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Editors.FeatureEdit
{
    public abstract class Feature
    {
        protected virtual uint FeatureID { set; get; }
        protected Feature(uint id)
        {
            FeatureID = id;
        }
        
        public abstract bool ApplyFeature(ref MyMesh mesh);
        
        public abstract JsonFeature ToJsonFeature();
    }
    
    public abstract class JsonFeature
    {
        [JsonProperty("Id", Order = -1)] public virtual uint FeatureID { protected set; get; }
        [JsonProperty("Type", Order = -2)] public virtual string Type { protected set; get; }
    
        protected JsonFeature(string type, uint  id)
        {
            Type = type;
            FeatureID = id;
        }
        public abstract Feature ToFeature(List<Sketch> sketches);
    }
}