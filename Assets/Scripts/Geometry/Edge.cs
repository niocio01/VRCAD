using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    public class Edge
    {
        public Face[] Faces { get; set; } = new Face[2];

        public bool OutsideEdge { get; set; }

        public Edge(Face first, Face second)
        {
            Faces[0] = first;
            Faces[1] = second;
        }
        public Edge(Face[] faces)
        {
            if (faces.Length is < 1 or > 2)
                return;

            Faces = faces;
        }
        public Edge(Face faceGroup)
        {
            Faces[0] = faceGroup;
        }
    }
}