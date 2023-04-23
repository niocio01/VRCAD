using System;
using System.Collections.Generic;
using System.Linq;
using Editors.FeatureEdit;
using Editors.PartEdit;
using Editors.SketchEdit;
using Geometry;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Rendering
{
    public class MeshDrawer : MonoBehaviour
    {
        [SerializeField] private PartEditor partEditor;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private XRRayInteractor rayInteractor;
        [SerializeField] private MeshCollider meshCollider;

        private MyMesh _myMesh;
        private Mesh _mesh;
        private bool _hit = false;
        
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

            meshCollider.sharedMesh = _mesh;
        }

        private bool GetHitTriangle()
        {
            if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
                return false;

            MeshCollider hitMeshCollider = hit.collider as MeshCollider;
            if (hitMeshCollider != null && hitMeshCollider.sharedMesh != _mesh)
                return false;

            Vector3[] vertices = _mesh.vertices;
            int[] triangles = _mesh.triangles;
            Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
            Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
            Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
            Transform hitTransform = hit.collider.transform;
            p0 = hitTransform.TransformPoint(p0);
            p1 = hitTransform.TransformPoint(p1);
            p2 = hitTransform.TransformPoint(p2);
            Debug.DrawLine(p0, p1);
            Debug.DrawLine(p1, p2);
            Debug.DrawLine(p2, p0);
            return true;
        }

        public void OnRayHit()
        {
            _hit = true;
        }

        private void Update()
        {
            if (!_hit)
                return;
            
            if (!GetHitTriangle())
            {
                _hit = false;
                return;
            }
        }
    }
}
