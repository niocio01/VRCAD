using UnityEngine;

[RequireComponent(typeof(SketchEditor))]
public class LineTool : MonoBehaviour
{
    [SerializeField] private GameObject previewLinePrefab;
    [SerializeField] private GameObject previewLineParent;
    
    private SketchEditor _sketchEditor;
    private SketchPoint _startPoint;
    private GameObject _previewLine;
    private LineController _lineController;

    private void Awake()
    {
        _sketchEditor = GetComponent<SketchEditor>();
        _previewLine = Instantiate(previewLinePrefab, Vector3.zero, Quaternion.identity, previewLineParent.transform);
        _lineController = _previewLine.GetComponent<LineController>();
    }
    private void Update()
    {
        if (_sketchEditor.CurrentTool != SketchTools.Line)
            return;

        if (_startPoint == null)
            return;

        if (!_sketchEditor.GetPointerPosition(out _, out Vector3 relPos))
            return;

        Vector3 lineStart = _sketchEditor.sketchPlane.transform.TransformPoint(_startPoint.Position.x, _startPoint.Position.y, 0);
        Vector3 lineEnd = _sketchEditor.sketchPlane.transform.TransformPoint(relPos.x, relPos.y, 0);

        _previewLine.GetComponent<Renderer>().enabled = true;

        _lineController.SetPoints(lineStart, lineEnd);
    }
    public void AddPoint()
    {
        if (!_sketchEditor.GetPointerPosition(out _, out Vector3 relPos))
            return;

        if (_startPoint == null)
        {
            _startPoint = _sketchEditor.Sketch.AddPoint(relPos.x, relPos.y);
        }
        else
        {
            SketchPoint endPoint = _sketchEditor.Sketch.AddPoint(relPos.x, relPos.y);
            SketchPoint startPoint = _startPoint;

            _startPoint = endPoint;

            _sketchEditor.Sketch.AddLine(startPoint, endPoint);

        }
    }
    public void EndLine()
    {
        _startPoint = null;
        _previewLine.GetComponent<Renderer>().enabled = false;
    }
}
