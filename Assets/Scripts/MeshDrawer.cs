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

        Vector3 polygonNormal = new Vector3(0, 0, 1);
        Vector3[] normals = new Vector3[temp.vertices.Length];
        for (int i = 0; i < temp.vertices.Length; i++)
        { normals[i] = polygonNormal; }

        Mesh.vertices = temp.vertices;
        Mesh.triangles = temp.triangles;

        Color[] colors = Enumerable.Range(0, Mesh.vertices.Length)
            .Select(i => Random.ColorHSV())
            .ToArray();
        Mesh.colors = colors;
        Mesh.normals = normals;

        // Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
    }
}
