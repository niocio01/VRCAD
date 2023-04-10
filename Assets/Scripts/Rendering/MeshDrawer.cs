using System.Linq;
using Editors.PartEdit;
using Editors.SketchEdit;
using Habrador_Computational_Geometry;
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


            _mesh.vertices = sketch.Face.Vertices3.ToArray();
            _mesh.triangles = sketch.Face.TriangleIndices.ToArray();
            _mesh.normals = sketch.Face.Normals.ToArray();

            // Mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }
    }
}
