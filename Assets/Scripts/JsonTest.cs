using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    private Part testPart;
    void InitTestSketchObject ()
    {
        testPart = new Part("TestPart", "ForDoingJsonTests", "Nico Zuber");

        Sketch sketch = new Sketch(0);
        testPart.AddSketch(sketch);
        sketch.AddPoint(0, 0);
        sketch.AddPoint(1, 0);
        sketch.AddPoint(1, 1);
        sketch.AddPoint(0, 1);

        sketch.AddLine(0, 1);
        sketch.AddLine(1, 2);

        sketch.AddConstraint(new Rectangular(sketch.GetLine(0), sketch.GetLine(1), sketch.ConstraintIdCounter));

        Extrude testExtrude = new Extrude(sketch, 1, 123);

        testPart.AddFeature(testExtrude);
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
