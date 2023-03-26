using Habrador_Computational_Geometry;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MeshDrawer : MonoBehaviour
{
    
    [SerializeField] private PartEditor PartEditor;
    [SerializeField] private MeshRenderer MeshRenderer;
    [SerializeField] private GameObject MeshParent;

    private Mesh Mesh;
    private void Awake()
    {
        var filter = MeshRenderer.AddComponent<MeshFilter>();
        Mesh = filter.mesh;
    }

    public void UpdateMesh()
    {
        Sketch sketch = PartEditor.Part.Sketches.FirstOrDefault();

        Mesh temp = _TransformBetweenDataStructures.Triangles2ToMesh(sketch.Triangulation, true);
        // TestAlgorithmsHelpMethods.DisplayMeshWithRandomColors(Mesh, 0);
        

        Mesh.vertices = temp.vertices;
        Mesh.triangles = temp.triangles;

        Color[] colors = Enumerable.Range(0, Mesh.vertices.Length)
            .Select(i => Random.ColorHSV())
            .ToArray();
        Mesh.colors = colors;

        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
    }
}
