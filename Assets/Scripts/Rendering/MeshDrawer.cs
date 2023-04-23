using System.Collections.Generic;
using System.Linq;
using Editors.FeatureEdit;
using Editors.PartEdit;
using Editors.SketchEdit;
using Geometry;
using Unity.VisualScripting;
using UnityEngine;

namespace Rendering
{
    public class MeshDrawer : MonoBehaviour
    {
        [SerializeField] private PartEditor partEditor;
        [SerializeField] private MeshRenderer meshRenderer;
        
        private MyMesh _myMesh;
        private Mesh _mesh;
        
        private void Awake()
        {
            var filter = meshRenderer.AddComponent<MeshFilter>();
            _mesh = filter.mesh;
        }
        
        public void RebuildMesh()
        {
            _myMesh = new MyMesh();
            _mesh.Clear();
            
            if (partEditor.Part.Features.Count < 1)
            {
                Sketch sketch = partEditor.Part.Sketches.First();
                if (sketch.Face == null) return;
                
                _myMesh.AddFace(sketch.Face);
            }

            else
            {
                foreach (Feature feature in partEditor.Part.Features)
                {
                    feature.ApplyFeature(ref _myMesh);
                }
            }
            
            _mesh.vertices = _myMesh.Vertices;
            _mesh.triangles = _myMesh.Triangles;
            _mesh.normals = _myMesh.Normals;
            _mesh.RecalculateBounds();
        }
    }
}
