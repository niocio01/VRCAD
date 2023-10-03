using System;
using System.Collections.Generic;
using System.Linq;
using Editors.SketchEdit;
using MyBox;
using Unity.VisualScripting;
using UnityEngine;

namespace Geometry
{
    public class MyMesh
    {
         
        
        public List<Surface> Surfaces { get; private set; }
        public List<MeshVertex> Vertices { get; private set; }
        public List<Edge> Edges { get; private set; }
        public List<Face> Faces { get; private set; }
        
        public int NumNonSharedVertices
        {
            get
            {
                return Surfaces.Sum(surface => surface.BoundaryVertices.Count);
            }
        }
        public int NumTriangles
        {
            get
            {
                return Surfaces.Sum(surface => surface.FaceVertexTriangleIndices.Length/3);
            }
        }
        
        public Vector3[] VertexArray { get; private set; }
        public int[] TriangleArray { get; private set; }
        public Vector3[] NormalArray { get; private set; }

        public MyMesh()
        {
            Surfaces = new List<Surface>();
            Vertices = new List<MeshVertex>();
            Edges = new List<Edge>();
            Faces = new List<Face>();
        }

        public void RecalculateArrays()
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();
            List<Vector3> normals = new();
            
            foreach (Surface surface in Surfaces)
            {
                vertices.AddRange(surface.BoundaryVertices.Select(vertex => vertex.Position));
                    
                triangles.AddRange(surface.FaceVertexTriangleIndices.Select(
                    index => index + surface.VertexIndexStart));
                
                normals.AddRange(surface.Normals);
            }

            VertexArray = vertices.ToArray();
            TriangleArray = triangles.ToArray();
            NormalArray = normals.ToArray();
        }

        public FlatSurface AddFlatSurfaceFromSketch(Sketch sketch)
        {
            if (!sketch.GetOutlineVertices(out List<Vector2> vector2S))
                return null;

            FlatSurface flatSurface = new FlatSurface(this, sketch.Pose, vector2S);
            AddSurface(flatSurface);
            if (!flatSurface.TriangulationValid)
                return null;
            sketch.FlatSurface = flatSurface;

            return flatSurface;
        }

        public void AddSurface(Surface surface, bool addMeshReferences = false)
        {
            surface.VertexIndexStart = NumNonSharedVertices;
            Surfaces.Add(surface);
            if (addMeshReferences)
            {
                surface.AddFacesToMesh(this);
                surface.UpdateEdgesOnMesh(this);
            }
        }

        public void AddFaces(List<Face> faces)
        {
            Faces.AddRange(faces);
        }

        public void AddEdges(List<Edge> edges)
        {
           Edges.AddRange(edges);
        }

        public void AddEdge(Edge edge)
        {
            Edges.Add(edge);
        }

        public Edge GetEdge(MeshVertex first, MeshVertex second)
        {
            IEnumerable<Edge> intersection = first.Edges.AsQueryable().Intersect(second.Edges);

            if (intersection.Count() == 0)
                return null;

            if (intersection.Count() < 1)
            {
                Debug.LogError("Found multiple of the same Edge.");
                return null;
            }

            return intersection.First();
        }

        public List<MeshVertex> AddVertices(List<Vector2> vertices, Pose pose)
        {
            List<Vector3> vector3S = vertices.ConvertAll(v => PolyUtils.PlaneVecTo3D(v, pose));
            return AddVertices(vector3S);
        }
        public List<MeshVertex> AddVertices(List<Vector3> vertices)
        {
            List<MeshVertex> convertedMeshVertices = new List<MeshVertex>();

            foreach (Vector3 vertex in vertices)
            {
                MeshVertex match = Vertices.Find(v => v.Position == vertex);
                if (match == null)
                {
                    MeshVertex newVertex = new MeshVertex(vertex); 
                    Vertices.Add(newVertex);
                    convertedMeshVertices.Add(newVertex);
                }
                else
                {
                    convertedMeshVertices.Add(match);
                }
            }
            return convertedMeshVertices;
        }

        
        public Surface GetFaceByMeshTriangle(int triangleIndex)
        {
            int firstVertexIndex = TriangleArray[triangleIndex * 3];
            foreach (Surface face in Surfaces)
            {
                if (firstVertexIndex >= face.VertexIndexStart && firstVertexIndex <= face.VertexIndexEnd)
                {
                    return face;
                }
            }
            return null;
        }
    }
}