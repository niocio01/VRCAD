using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
                    foreach (Vector2 vertex in face.Vertices2)
                    {
                        verts[i] = PolyUtils.PlaneVecTo3D(vertex, face.Pose);
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

            Face topFace = new Face(
                new Pose(baseFace.Origin + extrudeVector, baseFace.Pose.rotation),
                baseFace.Vertices3.ConvertAll(v => v + extrudeVector),
                baseFace.TriangleIndices);

            topFace.Mirror();

            mesh.AddFace(topFace);

            for (var i = 0; i < baseFace.Vertices3.Count; i++)
            {
                int noOfVerts = baseFace.Vertices3.Count();
                
                List<Vector3> vertices = new List<Vector3>();
                vertices.Add(baseFace.Vertices3[i]);
                vertices.Add(baseFace.Vertices3[(i + 1) % noOfVerts]);
                vertices.Add(baseFace.Vertices3[i] + extrudeVector);
                vertices.Add(baseFace.Vertices3[(i + 1) % noOfVerts] + extrudeVector);

                int[] triIndexes = new int[6];
                
                // first Triangle
                triIndexes[0] = 1;
                triIndexes[1] = 0;
                triIndexes[2] = 2;
                
                // second Triangle
                triIndexes[3] = 1;
                triIndexes[4] = 2;
                triIndexes[5] = 3;

                Vector3 normal = Vector3.Cross(vertices[2] - vertices[0], vertices[1] - vertices[0]).normalized;
                Vector3 upward = (vertices[2] - vertices[0]).normalized; 
                Vector3 origin = vertices[0] + (vertices[3] - vertices[0]) / 2;

                Pose pose = new Pose(origin, Quaternion.LookRotation(normal, upward  ));

                Face face = new Face(pose, vertices, triIndexes);
    
                mesh.AddFace(face);
            }

            return true;
        }
    }
}