using System;
using System.Collections.Generic;
using System.Linq;
using Editors.FeatureEdit;
using Editors.PartEdit;
using Editors.SketchEdit;
using Rendering;
using UnityEngine;

namespace Geometry
{
    public class MeshManager : MonoBehaviour
    {
        [SerializeField] private PartEditor partEditor;

        public static event Action OnMeshUpdated; 
        
        public MyMesh PartMesh { get; set; }

        public void AddSketchAsSurface(Sketch sketch)
        {
            PartMesh ??= new MyMesh();
            PartMesh.AddFlatSurfaceFromSketch(sketch);
            OnMeshUpdated?.Invoke();
        }

        public void RebuildMesh()
        {
            PartMesh = new MyMesh();

            if (partEditor.Part.Features.Count < 1)
            {
                Sketch sketch = partEditor.Part.Sketches.First();
                if (sketch.FlatSurface == null)
                {
                    if (sketch.IsClosed())
                    {
                        PartMesh.AddFlatSurfaceFromSketch(sketch);
                        OnMeshUpdated?.Invoke();
                    }
                }
            }
            else
            {
                foreach (Feature feature in partEditor.Part.Features)
                {
                    MyMesh partMesh = PartMesh;
                    feature.ApplyFeature(ref partMesh);
                }
            }
            OnMeshUpdated?.Invoke();
        }
    }
}