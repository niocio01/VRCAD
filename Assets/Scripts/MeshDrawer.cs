using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MeshDrawer : MonoBehaviour
{
    
    [SerializeField] private PartEditor PartEditor;
    [SerializeField] private MeshRenderer MeshRenderer;
    [SerializeField] private GameObject SketchPlane;

    private Mesh Mesh;
    private void Awake()
    {
        var filter = MeshRenderer.AddComponent<MeshFilter>();
        Mesh = filter.mesh;
    }

    public void UpdateMesh()
    {
        Sketch sketch = PartEditor.Part.Sketches.FirstOrDefault();
        Vector2[] vertices2D = sketch.EdgeVertices;
        Transform PlaneTransform = SketchPlane.transform;
        Vector3[] vertices3D = System.Array.ConvertAll<Vector2, Vector3>(vertices2D, v => v);
        vertices3D = System.Array.ConvertAll<Vector2, Vector3>(vertices2D, v => PlaneTransform.TransformPoint(v));

        Color[] colors = Enumerable.Range(0, vertices3D.Length)
            .Select(i => Random.ColorHSV())
            .ToArray();


        Mesh.vertices = vertices3D;
        Mesh.triangles = sketch.Triangles;
        Mesh.colors = colors;

        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
    }
}
