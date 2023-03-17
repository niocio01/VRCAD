using System;
using System.Drawing;

public abstract class ConstraintBase
{
    private readonly Type ChildT;
    private readonly Type ParentT;
    private readonly int ID;
    private readonly string Name;

    public ConstraintBase(Type child, Type parent, int id, string name)
    {
        ParentT = child;
        ParentT = parent;
        ID = id;
        Name = name;
    }
}

public class Line2Line_C : ConstraintBase
{
    public Line2Line_C(int id, string name) : base(typeof(SketchLine), typeof(SketchLine), id, name) {}
}
public class Point2Point_C : ConstraintBase
{
    public Point2Point_C(int id, string name) : base(typeof(SketchPoint), typeof(SketchPoint), id, name) { }
}
public class Point2Line_C : ConstraintBase
{
    public Point2Line_C(int id, string name) : base(typeof(SketchPoint), typeof(SketchLine), id, name) { }
}
public class Line2Point_C : ConstraintBase
{
    public Line2Point_C(int id, string name) : base(typeof(SketchLine), typeof(SketchPoint), id, name) { }
}

public class Constraint
{
    // Line to Line
    public readonly Line2Line_C Rectangular_C = new Line2Line_C(0, "Rectangular");
    public readonly Line2Line_C Paralell_C = new Line2Line_C(1, "Paralell");
    public readonly Line2Line_C Equal = new Line2Line_C(2, "Equal");

    // Point to Line
    public readonly Point2Line_C Midpoint = new Point2Line_C(3, "Midpoint");
    public readonly Point2Line_C OnLine = new Point2Line_C(4, "OnLine");

    // Point to Point
    public readonly Point2Point_C Congruent = new Point2Point_C(5, "Congruent");
    public readonly Point2Point_C Horizontal = new Point2Point_C(6, "Horizontal");
    public readonly Point2Point_C Vertical = new Point2Point_C(7, "Vertical");

}

public class SketchConstraint
{
    public ConstraintBase ConstraintType;
    public SketchElement Parent;
    public SketchElement Child;

    public SketchConstraint(ConstraintBase constraintType, SketchElement parent, SketchElement child)
    {
        ConstraintType = constraintType;
        Parent = parent;
        Child = child;
    }
}