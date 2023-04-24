using System;
using System.Collections.Generic;
using System.Linq;
using Editors.FeatureEdit;
using Editors.PartEdit;
using Editors.SketchEdit;
using Geometry;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
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
        private Face _highlightFace;
        
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
            _mesh.normals = _myMesh.Normals;
            _mesh.subMeshCount = 2;
            SetFullMeshTris();

            meshCollider.sharedMesh = _mesh;
        }

        private void SetFullMeshTris()
        {
            _mesh.SetIndices(_myMesh.Triangles, MeshTopology.Triangles, 0, calculateBounds: false);
            _mesh.SetIndices(new int[]{}, MeshTopology.Triangles, 1, calculateBounds: false);
            
            _mesh.RecalculateBounds();
        }

        private void SetMeshTrisWithHighlight(Face highlightFace)
        {
            if (highlightFace == null)
                return;
            
            List<int> mainTris = new List<int>();
            List<int> highlightTris = new List<int>();

            foreach (Face face in _myMesh.Faces)
            {
                List<int> temp = face.TriangleIndices.ToList().ConvertAll(i => i + face.VertexIndexStart);
                
                if (face == highlightFace)
                {
                    highlightTris.AddRange(temp);
                }
                else
                {
                    mainTris.AddRange(temp);
                }
            }
            
            _mesh.SetIndices(mainTris, MeshTopology.Triangles, 0, calculateBounds:false);
            _mesh.SetIndices(highlightTris, MeshTopology.Triangles, 1, calculateBounds: false);

            _mesh.RecalculateBounds();
        }

        private Face GetHitFace()
        {
            if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
                return null;

            MeshCollider hitMeshCollider = hit.collider as MeshCollider;
            if (hitMeshCollider != null && hitMeshCollider.sharedMesh != _mesh)
                return null;

            Face hitFace = _myMesh.GetFaceByMeshTriangle(hit.triangleIndex);

            return hitFace;
        }

        public void OnRayHit()
        {
            _hit = true;
            SetMeshTrisWithHighlight(_highlightFace);
        }

        private void Update()
        {
            if (!_hit)
                return;

            Face hitFace = GetHitFace();
            
            if (hitFace == null)
            {
                _hit = false;
                SetFullMeshTris();
                return;
            }
            if (hitFace == _highlightFace)
                return;

            _highlightFace = hitFace;
            SetMeshTrisWithHighlight(hitFace);
        }
    }
}
