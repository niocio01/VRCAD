using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    public class Face
    {
        /// Ordered List of vertex positions
        public List<Vector2> Vertices2 { private set; get; }

        public List<Vector3> Vertices3 {
            private set
            {
                Vertices2 = value.ConvertAll(v => PolyUtils.ProjectToPlane(v, Origin, FaceNormal));
            }
            get
            {
                return Vertices2.ConvertAll(v => new Vector3(v.x, v.y, 0));
            } 
        }
        
        public Vector3 Origin { get; private set; }

        /// Used to generate the vertex normals
        public Vector3 FaceNormal { private set; get; }

        /// Just a list filled with FaceNormal
        public List<Vector3> Normals
        {
            get
            {
                List<Vector3> normals = new List<Vector3>();
                for (int i = 0; i < Vertices2.Count; i++)
                {
                    normals.Add(FaceNormal);
                }

                return normals;
            }
        }

        private List<FaceTriangle> _triangles;
        public List<FaceTriangle> Triangles
        {
            get => _triangles;
            private set
            {
                if (value.Count != Vertices2.Count - 2)
                {
                    Debug.LogError("Number of Triangles does not match with number of Vertices.");
                    return;
                }

                _triangles = value;
            }
        }

        public int[] TriangleIndices
        {
            set
            {
               if (value.Length % 3 != 0)
               {
                   Debug.LogError("indices must come in pairs of 3");
                   return;
               }

               if (value.Length == 3 && Vertices3.Count != 3)
               {
                   Debug.LogError("indices count does not match with vertex count.");
                   return;
               }
               
               if (Vertices3.Count - 2 != value.Length/3 )
               {
                   Debug.LogError("indices count does not match with vertex count.");
                   return;
               }

               List<FaceTriangle> tris = new List<FaceTriangle>();
               for (int i = 0; i < value.Length; i += 3)
               {
                   tris.Add(new FaceTriangle(value[i], value[i+1], value[i+2]));
               }

               Triangles = tris;
            }
            get
            {
                int[] indices = new int[Triangles.Count * 3];
                int i = 0;
                foreach (FaceTriangle triangle in Triangles)
                {
                    int[] triVerts = triangle.VertexIndices;
                    indices[i++] = triVerts[0];
                    indices[i++] = triVerts[1];
                    indices[i++] = triVerts[2];
                }
                return indices;
            }
        }

        public Face(Vector3 origin, Vector3 faceNormal, List<Vector2> vertices2, int[] triangles)
        {
            Origin = origin;
            if (faceNormal.magnitude == 0f)
            {
                Debug.LogError("Face Normal cannot have length of zero.");
                return;
            }
            FaceNormal = faceNormal;
            Vertices2 = vertices2;
            TriangleIndices = triangles;
        }

        public Face(Vector3 origin, Vector3 faceNormal, List<Vector3> vertices3, int[] triangles)
        {
            Origin = origin;
            if (faceNormal.magnitude == 0f)
            {
                Debug.LogError("Face Normal cannot have length of zero.");
                return;
            }
            FaceNormal = faceNormal;
            Vertices3 = vertices3;
            TriangleIndices = triangles;
        }

        public void FlipNormals()
        {
            foreach (FaceTriangle triangle in Triangles)
            {
                triangle.FlipNormal();
            }
        }
    }

    public class FaceTriangle
    {
        public int[] VertexIndices { get; private set; } = new int[3];

        FaceTriangle(int[] vertexIndices)
        {
            if (vertexIndices.Length != 3)
            {
                Debug.LogError("FaceTriangle must have exactly 3 vertices!");
                return;
            }

            VertexIndices = vertexIndices;
        }

        public FaceTriangle(int first, int second, int third)
        {
            VertexIndices[0] = first;
            VertexIndices[1] = second;
            VertexIndices[2] = third;
        }

        public void FlipNormal()
        {
            (VertexIndices[0], VertexIndices[2]) = (VertexIndices[2], VertexIndices[0]);
        }
    }
}