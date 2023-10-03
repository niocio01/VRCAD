using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace Geometry
{
    /// A geometrical collection of connected faces (flat or not) 
    public abstract class Surface
    {
        public List<MeshVertex> BoundaryVertices { get; private protected set; }
        public List<Face> Faces { get; private protected set; }

        public int VertexIndexStart { get; set; }
        public int VertexIndexEnd => VertexIndexStart + BoundaryVertices.Count - 1;

        public Pose Pose { get; protected set; }
        public Vector3 Origin => Pose.position;

        public Vector3 FaceNormal => Pose.forward;
        public List<Vector3> Normals
        {
            get
            {
                List<Vector3> normals = new List<Vector3>();
                for (int i = 0; i < BoundaryVertices.Count; i++)
                {
                    normals.Add(FaceNormal);
                }

                return normals;
            }
        }

        public bool TriangulationValid { get; private protected set; }
        public int[] FaceVertexTriangleIndices {
            get
            {
                int[] indices = new int[Faces.Count * 3] ;
                for (int i = 0; i < indices.Length; i += 3)
                {
                    indices[i] = BoundaryVertices.IndexOf(Faces[i / 3].FaceVertices[0]);
                    indices[i + 1] = BoundaryVertices.IndexOf(Faces[i / 3].FaceVertices[1]);
                    indices[i + 2] = BoundaryVertices.IndexOf(Faces[i / 3].FaceVertices[2]);
                }

                return indices;
            }}

        protected Surface(MyMesh mesh, Pose pose)
        {
            Pose = pose;
            Faces = new List<Face>();
        }

        private protected void AddFaces(int[] triangles)
        {
            List<Face> faces = new List<Face>();
            for (int i = 0; i < triangles.Length; i += 3)
            {
                faces.Add(new Face(
                    BoundaryVertices[triangles[i]],
                    BoundaryVertices[triangles[i + 1]],
                    BoundaryVertices[triangles[i + 2]],
                    this));
            }
            Faces = faces;
        }

        public void AddFacesToMesh(MyMesh mesh)
        {
            mesh.AddFaces(Faces);
        }
        public void UpdateEdgesOnMesh(MyMesh mesh)
        {
            // create missing edges, add them to a list and add a reference to them in the vertices
            // modify already existing edges.
            
            foreach (Face face in Faces)
            {
                for (int i = 0; i < face.FaceVertices.Length; i++)
                {
                    Edge edge = mesh.GetEdge(face.FaceVertices[i], face.FaceVertices[(i + 1) % 3]);
                    if (edge == null)
                    {
                        Edge newEdge = new Edge(face);
                        face.FaceVertices[i].AddEdge(newEdge);
                        face.FaceVertices[(i + 1) % 3].AddEdge(newEdge);
                        mesh.AddEdge(newEdge);
                    }
                    else
                    {
                        if (edge.Faces[0] == null)
                        {
                            Debug.LogError("Found an corresponding edge, with empty face reference");
                            return;
                        }
                        if (edge.Faces[1] != null)
                        {
                            Debug.LogError("Found an corresponding edge, with already filled second face reference");
                            return;
                        }

                        edge.Faces[1] = face;
                    }
                }
            }
            
            // Mark the outside edges as such.
            for (int i = 0; i < BoundaryVertices.Count; i++)
            {
                Edge edge = mesh.GetEdge(BoundaryVertices[i], BoundaryVertices[(i + 1) % BoundaryVertices.Count]);
                if (edge == null)
                {
                    Debug.LogError("couldn't find an edge, that should exist while marking outside edges.");
                    return;
                }
                edge.OutsideEdge = true;
            }
        }

        public void FlipVectorNormals()
        {
            Pose newPose = new Pose(Pose.position, Quaternion.LookRotation(Pose.forward.Reverse(), Pose.up.Reverse()));
            Pose = newPose;
        }
        
        public void FlipFaceNormals()
        {
            // flip triangle windings
            foreach (Face face in Faces)
            {
                (face.FaceVertices[1], face.FaceVertices[2]) = (face.FaceVertices[2], face.FaceVertices[1]);
            }
        }

        public void Mirror()
        {
            FlipVectorNormals();
            FlipFaceNormals();
        }
    }

    /// A collection of faces, all on a flat Plane
    public class FlatSurface : Surface
    {
        public FlatSurface(MyMesh mesh, Pose pose, List<Vector2> boundaryVertices)
            : base(mesh, pose)
        {
            BoundaryVertices = mesh.AddVertices(boundaryVertices, pose);
            Triangulate();
            AddFacesToMesh(mesh);
            UpdateEdgesOnMesh(mesh);
        }
        public FlatSurface(MyMesh mesh, Pose pose, List<Vector3> boundaryVertices) 
            : base(mesh, pose)
        {
            BoundaryVertices = mesh.AddVertices(boundaryVertices);
            Triangulate();
            AddFacesToMesh(mesh);
            UpdateEdgesOnMesh(mesh);
        }
        public FlatSurface(MyMesh mesh, Pose pose, List<Vector2> boundaryVertices, int[] faceVertexTriangleIndices)
            : base(mesh, pose)
        {
            BoundaryVertices = mesh.AddVertices(boundaryVertices, pose);
            TriangulationValid = true;
            AddFaces(faceVertexTriangleIndices);
            AddFacesToMesh(mesh);
            UpdateEdgesOnMesh(mesh);
        }
        public FlatSurface(MyMesh mesh, Pose pose, List<Vector3> boundaryVertices, int[] faceVertexTriangleIndices)
            : base(mesh, pose)
        {
            BoundaryVertices = mesh.AddVertices(boundaryVertices);
            TriangulationValid = true;
            AddFaces(faceVertexTriangleIndices);
            AddFacesToMesh(mesh);
            UpdateEdgesOnMesh(mesh);
        }

        public bool Triangulate()
        {
            List<Vector2> vector2S = BoundaryVertices.ConvertAll(v => PolyUtils.ProjectToPlane(v.Position, Pose));
            if (PolyUtils.Triangulate(vector2S, out List<int> triangles))
            {
                TriangulationValid = true;
                AddFaces(triangles.ToArray());
                return true;
            }
            return false;
        }
    }
}