using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Geometry
{
    public class ClosedShape
    {
        public List<LinkedVertex> Vertices { get; private set; }
        public bool IsValid
        {
            get
            {
                if (Vertices.Count < 3) return false;
                if (Vertices.FirstOrDefault() == null || Vertices.LastOrDefault() == null) return false;
                if (Vertices.First().Prev == null || Vertices.Last().Next == null) return false;
                return true;
            }
        }

        public ClosedShape(List<Vector2> vertices)
        {
            Vertices = new List<LinkedVertex>();
            foreach (Vector2 vertex in vertices)
            {
                Add(vertex);
            }

            Close();
        }

        public void Add(Vector2 vertex)
        {
            LinkedVertex linkedVertex = new LinkedVertex(vertex, Vertices.Count, Vertices.LastOrDefault());
            if (linkedVertex.Prev != null)
            {
                linkedVertex.Prev.Next = linkedVertex;
            }
            Vertices.Add(linkedVertex);
        }
        public void Remove(LinkedVertex vertex)
        {
            vertex.Prev.ResetStats();
            vertex.Next.ResetStats();

            vertex.Prev.Next = vertex.Next;
            vertex.Next.Prev = vertex.Prev;
            Vertices.Remove(vertex);
        }
        public void Close()
        {
            Vertices.LastOrDefault()!.Next = Vertices.FirstOrDefault();
            Vertices.FirstOrDefault()!.Prev = Vertices.LastOrDefault();
        }
        public void Reverse()
        {
            Vertices.Reverse();
            for (var i = 0; i < Vertices.Count; i++)
            {
                var vertex = Vertices[i];
                vertex.Reverse();
                vertex.Index = i;
            }
        }
        
        public WindingDir GetWindingDir()
        {
            List<Vector2> vertices = Vertices.ConvertAll(v => v.Pos);
            return PolyUtils.FindWindingDir(vertices);
        }
        public bool IsCornerEmpty(LinkedVertex corner)
        {
            foreach (LinkedVertex vertex in Vertices)
            {
                // Reflex vertex cannot be inside another concave ear
                if (vertex.IsReflex) continue;
                
                if (PolyUtils.IsVertexInsideCorner(vertex, corner)) return false;
            }

            return true;
        }

        public List<Vector2> GetVector2S()
        {
            return Vertices.ConvertAll(v => v.Pos);
        }
    }
    
    public class LinkedVertex
    {
        public Vector2 Pos { get; set; }
        public int Index { get; set; }

        private LinkedVertex _prev;
        public LinkedVertex Prev
        {
            get => _prev;
            set
            {
                ResetStats();
                _prev = value;
            }
        }

        private LinkedVertex _next;
        public LinkedVertex Next
        {
            get => _next;
            set
            {
                ResetStats();
                _next = value;
            }
        }

        private bool? _isReflex;
        public bool IsReflex
        {
            get
            {
                if (_isReflex != null) return (bool)_isReflex;
                float cross2 = PolyUtils.Cross2(Prev.Pos - Pos, Next.Pos - Pos);
                _isReflex = cross2 < 0f;
                return (bool)_isReflex;
            }
        }

        public LinkedVertex(Vector2 position, int index, LinkedVertex previous = null, LinkedVertex next = null)
        {
            Pos = position;
            Index = index;
            Prev = previous;
            Next = next;
        }

        public void ResetStats()
        {
            _isReflex = null;
        }
        public void Reverse()
        {
            (Prev, Next) = (Next, Prev);
        }
    }
}