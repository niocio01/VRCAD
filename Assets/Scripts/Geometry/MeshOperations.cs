using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Geometry
{
    public static class MeshOperations
    {
        public static bool ExtrudeFace(FlatSurface bottomSurface, Vector3 extrudeVector, ref MyMesh mesh, bool addBaseFace = false)
        {
            if (addBaseFace)
            {
                mesh.AddSurface(bottomSurface, true);
            }
            bottomSurface.Mirror();

            // first add the parallel face
            List<Vector3> topSurfaceVertexPositions = bottomSurface.BoundaryVertices.ConvertAll(v => v.Position + extrudeVector);

            FlatSurface topSurface = new(
                mesh,
                new Pose(bottomSurface.Origin + extrudeVector, bottomSurface.Pose.rotation),
                topSurfaceVertexPositions,
                bottomSurface.FaceVertexTriangleIndices);
            
            topSurface.Mirror();
            mesh.AddSurface(topSurface);

            int noOfVerts = bottomSurface.BoundaryVertices.Count;
            for (var i = 0; i < noOfVerts; i++)
            {
                List<Vector3> bottomVertices = bottomSurface.BoundaryVertices.ConvertAll(v => v.Position);
                List<Vector3> vertices = new()
                {
                    bottomVertices[i],
                    bottomVertices[(i + 1) % noOfVerts],
                    bottomVertices[(i + 1) % noOfVerts] + extrudeVector,
                    bottomVertices[i] + extrudeVector,
                };

                int[] triIndices = new int[6];
                
                // first Triangle
                triIndices[0] = 1;
                triIndices[1] = 0;
                triIndices[2] = 3;
                
                // second Triangle
                triIndices[3] = 1;
                triIndices[4] = 3;
                triIndices[5] = 2;

                Vector3 normal = (Vector3.Cross(vertices[3] - vertices[0], vertices[1] - vertices[0]).normalized);
                Vector3 upward = (vertices[0] - vertices[3]).normalized; 
                Vector3 origin = vertices[0] + (vertices[2] - vertices[0]) / 2;
                
                Pose pose = new Pose(origin, Quaternion.LookRotation(normal, upward));

                
                FlatSurface flatSurface = new(mesh, pose, vertices, triIndices);
                mesh.AddSurface(flatSurface);
            }
            

            return true;
        }
    }
}