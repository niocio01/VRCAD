using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    private Part testPart;
    void InitTestSketchObject ()
    {
        testPart = new Part("TestPart", "ForDoingJsonTests", "Nico Zuber");

        Sketch sketch = new Sketch(testPart.SketchIdCounter);
        testPart.AddSketch(sketch);
        sketch.AddPoint(0, 0);
        sketch.AddPoint(1, 0);
        sketch.AddPoint(1, 1);
        sketch.AddPoint(0, 1);

        sketch.AddLine(0, 1);
        sketch.AddLine(1, 2);
        sketch.AddLine(2, 3);
        sketch.AddLine(3, 0);

        sketch.AddConstraint(new Rectangular(sketch.GetLine(0), sketch.GetLine(1), sketch.ConstraintIdCounter));
        sketch.AddConstraint(new Horizontal(sketch.GetPoint(0), sketch.GetPoint(1), sketch.ConstraintIdCounter));
        sketch.AddConstraint(new Vertical(sketch.GetPoint(0), sketch.GetPoint(3), sketch.ConstraintIdCounter));

        testPart.AddFeature(new Extrude(sketch, 0.55f, testPart.FeatureIdCounter));
        testPart.AddFeature(new Revolve(sketch, sketch.GetLine(2), testPart.FeatureIdCounter));

        Sketch sketch2 = new Sketch(testPart.SketchIdCounter);
        testPart.AddSketch(sketch2);
        sketch2.AddPoint(-1, -1);
        sketch2.AddPoint(-1, -2);
        sketch2.AddLine(0, 1, true);
    }
    public void PrintTestJson()
    {
        InitTestSketchObject();

        JsonHandler.JsonSave(testPart);
    }

    [SerializeField] TextAsset jsonFile;
    public void ReadJson()
    {
        JsonHandler.JsonLoad(jsonFile);
    }
}
