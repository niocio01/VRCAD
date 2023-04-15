using System.Collections.Generic;
using System.Linq;
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

        private Mesh _mesh;
        private void Awake()
        {
            var filter = meshRenderer.AddComponent<MeshFilter>();
            _mesh = filter.mesh;
        }
        public void UpdateMesh()
        {
            // TODO: use not just the first, but all of the sketches and features to build the mesh
            Sketch sketch = partEditor.Part.Sketches.FirstOrDefault();

            MyMesh myMesh = new MyMesh();

            if (sketch.Face != null)
            {
                myMesh.AddFace(sketch.Face);
                MeshOperations.Extrude(sketch.Face, new Vector3(0, 0, 0.2f), ref myMesh);

                _mesh.vertices = myMesh.Vertices;
                _mesh.triangles = myMesh.Triangles;
                _mesh.normals = myMesh.Normals;
                _mesh.RecalculateBounds();
            }

            else
            {
                _mesh.Clear();
            }
        }
    }
}
