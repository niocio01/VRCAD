using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTool : MonoBehaviour
{
    [SerializeField] GameObject PreviewLinePrefab;
    [SerializeField] GameObject PreviewLineParent;
    [SerializeField] SketchEditor SketchEditor;

    SketchPoint StartPoint;
    GameObject PreviewLine;
    LineController LineController;

    private void Awake()
    {
        PreviewLine = Instantiate(PreviewLinePrefab, Vector3.zero, Quaternion.identity, PreviewLineParent.transform);
        LineController = PreviewLine.GetComponent<LineController>();
    }


    // Update is called once per frame
    void Update()
    {
        if (SketchEditor.CurrentTool != SketchTools.Line)
            return;

        if (StartPoint == null)
            return;

        if (!SketchEditor.GetPointerPosition(out Vector3 absPos, out Vector3 relPos))
            return;

        Vector3 lineStart = SketchEditor.SketchPlane.transform.TransformPoint(StartPoint.Position.x, StartPoint.Position.y, 0);
        Vector3 lineEnd = SketchEditor.SketchPlane.transform.TransformPoint(relPos.x, relPos.y, 0);

        PreviewLine.GetComponent<Renderer>().enabled = true;

        LineController.SetPoints(lineStart, lineEnd);
    }

    public void AddPoint()
    {
        if (!SketchEditor.GetPointerPosition(out Vector3 absPos, out Vector3 relPos))
            return;

        if (StartPoint == null)
        {
            StartPoint = SketchEditor.Sketch.AddPoint(relPos.x, relPos.y);
        }
        else
        {
            SketchPoint endPoint = SketchEditor.Sketch.AddPoint(relPos.x, relPos.y);
            SketchPoint startPoint = StartPoint;

            StartPoint = endPoint;

            SketchEditor.Sketch.AddLine(startPoint, endPoint);

        }
    }
    public void EndLine()
    {
        StartPoint = null;
        PreviewLine.GetComponent<Renderer>().enabled = false;
    }
}
