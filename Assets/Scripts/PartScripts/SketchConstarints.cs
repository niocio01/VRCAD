using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;

public class SketchConstraint
{
    public string Name { get; private set; }
    public uint ConstraintID { get; private set; }

    public SketchElement Parent { get; private set; }
    public SketchElement Child { get; private set; }

    private readonly Type ChildType;
    private readonly Type ParentType;
    public SketchConstraint(string name, Type parentType, SketchElement parent, Type childType, SketchElement child, uint id)
    {

        if (Equals(parent, parentType))
            return;

        if (Equals(child, childType))
            return;

        Name = name;
        ParentType = parentType;
        Parent = parent;
        ChildType = childType;
        Child = child;
        ConstraintID = id;
    }

    public JsonConstraint ToJsonConstraint()
    {
        JsonConstraint constraint = new JsonConstraint();
        constraint.Name = Name;
        constraint.ConstraintID = ConstraintID;
        constraint.Parent = Parent.ID;
        constraint.Child = Child.ID;

        return constraint;
    }
}

public class JsonConstraint
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("id")]
    public uint ConstraintID { get; set; }

    [JsonProperty("parent")]
    public uint Parent { get; set; }

    [JsonProperty("child")]
    public uint Child { get; set; }

    public SketchConstraint ToSketchConstraint(List<SketchPoint> points, List<SketchLine> lines)
    {
        SketchConstraint constraint = null;
        switch (Name)
        {
            // Line to Line
            case "Rectangular": constraint = new Rectangular(lines.Find(l => l.ID == Parent), lines.Find(l =>l.ID == Child), ConstraintID); break;
            
            // Point to Point
            case "Horizontal": constraint = new Horizontal(points.Find(l => l.ID == Parent), points.Find(l => l.ID == Child), ConstraintID); break;
            case "Vertical": constraint = new Vertical(points.Find(l => l.ID == Parent), points.Find(l => l.ID == Child), ConstraintID); break;
        }
        return constraint;
    }
}

#region LineToLine
public class Rectangular : SketchConstraint
{
    public Rectangular(SketchElement parent, SketchElement child, uint id) : base("Rectangular", typeof(SketchLine), parent, typeof(SketchLine), child, id) { }
}

#endregion

#region PointToPoint
public class Horizontal : SketchConstraint
{
    public Horizontal(SketchElement parent, SketchElement child, uint id) : base("Horizontal", typeof(SketchPoint), parent, typeof(SketchPoint), child, id) { }
}

public class Vertical : SketchConstraint
{
    public Vertical(SketchElement parent, SketchElement child, uint id) : base("Vertical", typeof(SketchPoint), parent, typeof(SketchPoint), child, id) { }
}
#endregion
