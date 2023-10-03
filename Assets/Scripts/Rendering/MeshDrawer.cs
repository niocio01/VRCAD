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
       
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private XRRayInteractor rayInteractor;
        [SerializeField] private MeshCollider meshCollider;
        [SerializeField] private MeshManager meshManager;

        private MyMesh _myMesh;
        private Mesh _mesh;
        private bool _hit = false;
        private FlatSurface _highlightFlatSurface;
        private bool _meshIsOld;

        private void Awake()
        {
            var filter = meshRenderer.AddComponent<MeshFilter>();
            _mesh = filter.mesh;
            MeshManager.OnMeshUpdated += MarkMeshAsOld;
        }

        private void Start()
        {
            _myMesh = meshManager.PartMesh;
            meshCollider.sharedMesh = _mesh;
        }

        private void MarkMeshAsOld()
        {
            _meshIsOld = true;
            if (meshManager.PartMesh != null)
            {
                _myMesh = meshManager.PartMesh;
            }
        }

        private void UpdateMesh()
        {
            _myMesh.RecalculateArrays();
            
            _mesh.vertices = _myMesh.VertexArray;
            _mesh.normals = _myMesh.NormalArray;
            // _mesh.RecalculateBounds();
            // _mesh.RecalculateNormals();
            _mesh.subMeshCount = 2;
            SetFullMeshTris();

            _meshIsOld = false;
        }

        private void SetFullMeshTris()
        {
            _mesh.SetIndices(_myMesh.TriangleArray, MeshTopology.Triangles, 0, calculateBounds: false);
            _mesh.SetIndices(new int[]{}, MeshTopology.Triangles, 1, calculateBounds: false);
            
            _mesh.RecalculateBounds();
        }

        private void SetMeshTrisWithHighlight(FlatSurface highlightFlatSurface)
        {
            if (highlightFlatSurface == null)
                return;
            
            List<int> mainTris = new List<int>();
            List<int> highlightTris = new List<int>();

            foreach (Surface surface in meshManager.PartMesh.Surfaces)
            {
                List<int> temp = surface.FaceVertexTriangleIndices.ToList().ConvertAll(i => i + surface.VertexIndexStart);
                
                if (surface == highlightFlatSurface)
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

        private FlatSurface GetHitFace()
        {
            if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
                return null;

            MeshCollider hitMeshCollider = hit.collider as MeshCollider;
            if (hitMeshCollider != null && hitMeshCollider.sharedMesh != _mesh)
                return null;

            Surface hitSurface = meshManager.PartMesh.GetFaceByMeshTriangle(hit.triangleIndex);
            if (hitSurface is FlatSurface surface)
            {
                return surface;
            }

            return null;
        }

        public void OnRayHit()
        {
            _hit = true;
            SetMeshTrisWithHighlight(_highlightFlatSurface);
        }

        private void Update()
        {
            if (_meshIsOld)
            {
                UpdateMesh();
            }
            
            if (!_hit)
                return;
            
            FlatSurface hitFlatSurface = GetHitFace();
            
            if (hitFlatSurface == null)
            {
                _hit = false;
                SetFullMeshTris();
                return;
            }
            if (hitFlatSurface == _highlightFlatSurface)
                return;
            
            _highlightFlatSurface = hitFlatSurface;
            SetMeshTrisWithHighlight(hitFlatSurface);
        }
    }
}
