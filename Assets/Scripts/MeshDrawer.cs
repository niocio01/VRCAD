using Habrador_Computational_Geometry;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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

        Mesh temp = _TransformBetweenDataStructures.Triangles2ToMesh(sketch.Triangulation, true);
        // TestAlgorithmsHelpMethods.DisplayMeshWithRandomColors(Mesh, 0);

        Vector3 polygonNormal = new Vector3(0, 0, 1);
        Vector3[] normals = new Vector3[temp.vertices.Length];
        for (int i = 0; i < temp.vertices.Length; i++)
        { normals[i] = polygonNormal; }

        _mesh.vertices = temp.vertices;
        _mesh.triangles = temp.triangles;

        Color[] colors = Enumerable.Range(0, _mesh.vertices.Length)
            .Select(i => Random.ColorHSV())
            .ToArray();
        _mesh.colors = colors;
        _mesh.normals = normals;

        // Mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }
}
