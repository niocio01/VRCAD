using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Geometry
{
    public class MeshVertex : IEquatable<MeshVertex>
    {
        public Vector3 Position { get; set; }
        public List<Edge> Edges { get; private set; }

        public MeshVertex(Vector3 position, List<Edge> edges)
        {
            Position = position;
            Edges = edges;
        }
        public MeshVertex(Vector3 position, Edge edge)
        {
            Position = position;
            Edges = new List<Edge> {edge};
        }
        public MeshVertex(Vector3 position)
        {
            Position = position;
            Edges = new List<Edge>();
        }

        public void AddEdge(Edge edge)
        {
            Edges.Add(edge);
        }

        public bool Equals(MeshVertex other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Position.Equals(other.Position);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MeshVertex)obj);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }
}