using Newtonsoft.Json;
using System;
using System.Drawing;

    public abstract class SketchConstraint
    {
        public string Name { get; private set; }
        public uint ConstraintID { get; private set; }

        public SketchElement Parent { get; private set; }
        public SketchElement Child { get; private set; }

        private readonly Type ChildType;
        private readonly Type ParentType;
        public SketchConstraint(string name, Type parentType, SketchElement parent, Type childType, SketchElement child, uint id )
        {
            
            if (Equals(parent, parentType))
                return;

            if(Equals(child, childType))
                return;

            Name = name;
            ParentType = parentType;
            Parent = parent;
            ChildType = childType;
            Child = child;
            ConstraintID = id;
        }
    }

    // Line to Line
    public class Rectangular : SketchConstraint
{
        public Rectangular(SketchElement parent, SketchElement child, uint id) : base("Rectangular", typeof(SketchLine), parent, typeof(SketchLine), child, id) { }
    }

    // Point to Point
    public abstract class Horizontal : SketchConstraint
{
        public Horizontal(SketchElement parent, SketchElement child, uint id) : base("Horizontal", typeof(SketchPoint), parent, typeof(SketchPoint), child, id) { }
    }

    public abstract class Vertical : SketchConstraint
{
        public Vertical(SketchElement parent, SketchElement child, uint id) : base("Vertical", typeof(SketchPoint), parent, typeof(SketchPoint), child, id) { }
    }

