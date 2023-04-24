using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Geometry
{
    public static class MeshOperations
    {
        public static bool ExtrudeFace(Face baseFace, Vector3 extrudeVector, ref MyMesh mesh, bool addBaseFace = false)
        {
            if (addBaseFace)
            {
                mesh.AddFace(baseFace);
            }
            // first add the parallel face

            Face topFace = new Face(
                new Pose(baseFace.Origin + extrudeVector, baseFace.Pose.rotation),
                baseFace.Vertices3.ConvertAll(v => v + extrudeVector),
                baseFace.TriangleIndices);

            baseFace.Mirror();

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
                Vector3 upward = (vertices[0] - vertices[2]).normalized; 
                Vector3 origin = vertices[0] + (vertices[3] - vertices[0]) / 2;

                Pose pose = new Pose(origin, Quaternion.LookRotation(normal, upward));

                Face face = new Face(pose, vertices, triIndexes);
    
                mesh.AddFace(face);
            }

            return true;
        }
    }
}