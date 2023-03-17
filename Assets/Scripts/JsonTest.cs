using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    private Part testPart;
    void InitTestSketchObject ()
    {
        testPart = new Part("TestPart", "ForDoingJsonTests", "Nico Zuber");

        Sketch testSketch = new Sketch(0);
        testSketch.AddPoint(0, 0);
        testSketch.AddPoint(1, 0);
        testSketch.AddPoint(1, 1);
        testSketch.AddPoint(0, 1);

        testSketch.AddLine(0, 1);
        testSketch.AddLine(1, 2);

        Extrude testExtrude = new Extrude(testSketch, 1, 123);

        testPart.AddFeature(testExtrude);
    }

    public void PrintTestJson()
    {
        if (testPart == null)
        {
            InitTestSketchObject();
        }

        JsonHandler.JsonSave(testPart);
    }
}
