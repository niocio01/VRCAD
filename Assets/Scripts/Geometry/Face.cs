using UnityEngine;
using UnityEngine.UIElements;

namespace Geometry
{
    /// Single face (Triangle) of myMesh 
    public class Face
    {
        public MeshVertex[] FaceVertices { get; private set; } = new MeshVertex[3];
        public Surface ParentSurface { get; private set; }

        public Face(MeshVertex[] faceVertices, Surface parentSurface)
        {
            FaceVertices = faceVertices;
            ParentSurface = parentSurface;
        }

        public Face(MeshVertex first, MeshVertex second, MeshVertex third, Surface parentSurface)
        {
            FaceVertices[0] = first;
            FaceVertices[1] = second;
            FaceVertices[2] = third;
            ParentSurface = parentSurface;
        }

        public void FlipNormal()
            {
                (FaceVertices[0], FaceVertices[2]) = (FaceVertices[2], FaceVertices[0]);
            }
    }
}