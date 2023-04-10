using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    public class Face
    {
        private List<Vector2> _vertices2;

        /// Ordered List of vertex positions
        public List<Vector2> Vertices2
        {
            private set
            {
                _vertices2 = value;
                Vertices3 = _vertices2.ConvertAll(v => new Vector3(v.x, v.y, 0));
            }
            get => _vertices2;
        }
        public List<Vector3> Vertices3 { private set; get; }

        /// Used to generate the vertex normals
        public Vector3 FaceNormal { private set; get; }

        /// Just a list filled with FaceNormal
        public List<Vector3> Normals
        {
            get
            {
                List<Vector3> normals = new List<Vector3>();
                for (int i = 0; i < _vertices2.Count; i++)
                {
                    normals.Add(FaceNormal);
                }

                return normals;
            }
        }

        private List<FaceTriangle> _triangles;
        public List<FaceTriangle> Triangles
        {
            private set
            {
                if (value.Count != Vertices2.Count - 1)
                {
                    Debug.LogError("Number of Triangles does not match with number of Vertices.");
                    return;
                }

                _triangles = value;
            }
            get => _triangles;
        }

        public List<int> TriangleIndices
        {
            set
            {
               if (value.Count % 3 != 0)
               {
                   Debug.LogError("indices must come in pairs of 3");
                   return;
               }

               if (value.Count == 3 && Vertices2.Count != 3)
               {
                   Debug.LogError("indices count does not match with vertex count.");
                   return;
               }
               
               if (Vertices2.Count - 2 != value.Count/3 )
               {
                   Debug.LogError("indices count does not match with vertex count.");
                   return;
               }

               List<FaceTriangle> tris = new List<FaceTriangle>();
               for (int i = 0; i < value.Count; i += 3)
               {
                   tris.Add(new FaceTriangle(value[i], value[i+1], value[i+2]));
               }

               _triangles = tris;
            }
            get
            {
                List<int> indices = new List<int>();
                foreach (FaceTriangle triangle in _triangles)
                {
                    int[] triVerts = triangle.VertexIndices;
                    indices.Add(triVerts[0]);
                    indices.Add(triVerts[1]);
                    indices.Add(triVerts[2]);
                }
                return indices;
            }
        }

        public Face(List<Vector2> vertices2, Vector3 faceNormal, List<int> triangles)
        {
            Vertices2 = vertices2;
            FaceNormal = faceNormal;
            TriangleIndices = triangles;
        }

        public Face(List<Vector3> vertices3, Vector3 faceNormal, List<int> triangles)
        {
            Vertices3 = vertices3;
            FaceNormal = faceNormal;
            TriangleIndices = triangles;
        }
    }

    public class FaceTriangle
    {
        public int[] VertexIndices { get; } = new int[3];

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
    }
}