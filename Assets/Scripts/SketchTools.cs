using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SketchTools_t
{
    Point,
    Line,
    Circle,
};

public class SketchTools : MonoBehaviour
{
    [SerializeField] Button PointTool;
    [SerializeField] Button LineTool;
    [SerializeField] Button CircleTool;

    private void Start()
    {
        SetSketchTool("Point");
    }

    public SketchTools_t CurrentTool { get; private set; } = SketchTools_t.Point;

    public void SetSketchTool(string tool)
    {

        PointTool.enabled = false;
        LineTool.enabled = false;
        CircleTool.enabled = false;

        CurrentTool = SketchTools_t.Point;

        switch(tool)
        {
            case "Point": PointTool.enabled = true; break;
            case "Line" : LineTool.enabled = true; break;
            case "Circle" : CircleTool.enabled = true; break;
        }
    }
}
