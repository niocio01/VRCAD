using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Geometry
{
    public class MyMesh
    {
        public List<Face> Faces { get; private set; }
        
        public int NumVertices { get; private set; }
        public int NumTriangles { get; private set; }
        
        public Vector3[] Vertices
        {
            get
            {
                Vector3[] verts = new Vector3[NumVertices];
                int i = 0;
                foreach (Face face in Faces)
                {
                    foreach (Vector3 vertex in face.Vertices3)
                    {
                        verts[i] = vertex + face.Origin;
                        i++;
                    }
                }
                return verts;
            }
        }

        public int[] Triangles
        {
            get
            {
                int[] tris = new int[NumTriangles*3];
                int i = 0;
                int faceTriIndexOffset = 0;
                foreach (Face face in Faces)
                {
                    foreach (int triangleIndex in face.TriangleIndices)
                    {
                        tris[i] = triangleIndex + faceTriIndexOffset;
                        i++;
                    }

                    faceTriIndexOffset += face.Vertices3.Count;
                }

                return tris;
            }
        }

        public Vector3[] Normals
        {
            get
            {
                Vector3[] normals = new Vector3[NumVertices];
                int i = 0;
                foreach (Face face in Faces)
                {
                    foreach (Vector3 vertNormal in face.Normals)
                    {
                        normals[i] = vertNormal;
                        i++;
                    }
                }

                return normals;
            }
        }

        public MyMesh()
        {
            Faces = new List<Face>();
            NumVertices = 0;
            NumTriangles = 0;
        }

        public void AddFace(Face face)
        {
            Faces.Add(face);
            NumVertices += face.Vertices3.Count;
            NumTriangles += face.Triangles.Count;
        }
    }
    
    public static class MeshOperations
    {
        public static bool Extrude(Face baseFace, Vector3 extrudeVector, ref MyMesh mesh)
        {
            // first add the parallel face

            Face topFace = new Face(baseFace.Origin + extrudeVector,
                baseFace.FaceNormal.Reverse(),
                baseFace.Vertices3.ConvertAll(v => v + extrudeVector),
                baseFace.TriangleIndices);
            
            topFace.FlipNormals();
            
            mesh.AddFace(topFace);

            return true;
        }
    }
}