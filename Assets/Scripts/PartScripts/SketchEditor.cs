using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
public enum SketchTools
{
    Point,
    Line,
    Circle,
};

[RequireComponent(typeof(PointDrawer))]
[RequireComponent(typeof(LineDrawer))]
[RequireComponent(typeof(LineTool))]
public class SketchEditor : MonoBehaviour
{
    [SerializeField] public GameObject sketchPlane;
    
    [SerializeField] private GameObject reticlePrefab;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private GameObject previewPointParent;
    [SerializeField] private RadioButtonController toolsGroup;
    
    [SerializeField] private bool snapToGrid = true;
    [SerializeField] private float snapSize = 1f;
    
    private PointDrawer _pointDrawer;
    private LineDrawer _lineDrawer;
    private LineTool _lineTool;

    public Sketch Sketch { get; private set; }
    public SketchTools CurrentTool { get; private set; } = SketchTools.Line;
    private GameObject _reticle;

    private void Awake()
    {
        _pointDrawer = GetComponent<PointDrawer>();
        _lineDrawer = GetComponent<LineDrawer>();
        _lineTool = GetComponent<LineTool>();
    }
    private void Start()
    {
        _reticle = Instantiate(reticlePrefab, previewPointParent.transform);
        _reticle.GetComponent<Renderer>().enabled = false;
        toolsGroup.onSelectionChanged.AddListener(SetTool);
    }
    private void Update()
    {
        if (GetPointerPosition(out Vector3 absPos, out _))
        {
            _reticle.GetComponent<Renderer>().enabled = true;
            _reticle.transform.position = absPos;
        }
        else
        {
            _reticle.GetComponent<Renderer>().enabled = false;
        }
    }
    private void SetSketch(Sketch sketch)
    {
        Sketch = sketch;
        _pointDrawer.SetSketch(sketch);
        _lineDrawer.SetSketch(sketch);
    }
    public bool StartEditSketch(Sketch sketch)
    {
        if (sketch == null) return false;

        SetSketch(sketch);

        return true;
    }
    public void OnInteracted()
    {
        switch (CurrentTool) 
        {
            case SketchTools.Point: AddPoint(); break;
            case SketchTools.Line: _lineTool.AddPoint(); break;
            case SketchTools.Circle: break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public void AddPoint()
    {
        if (GetPointerPosition(out _, out Vector3 relPos))
        {
            SketchPoint point = Sketch.AddPoint(relPos.x, relPos.y);
            print(point.ID);
            print(point.Position.ToString());
        }
        else
        {
            print("failed to get point on plane position");
        }
    }
    private void SetTool(string toolName)
    {
        // Deselect old Tool
        switch (CurrentTool)
        {
            case SketchTools.Point: break;
            case SketchTools.Line: _lineTool.EndLine(); break;
            case SketchTools.Circle: break;
        }

        // Set new Tool
        switch(toolName) 
        {
            case "PointTool": CurrentTool = SketchTools.Point; break;
            case "LineTool": CurrentTool = SketchTools.Line; break;
            case "CircleTool": CurrentTool= SketchTools.Circle; break;
            default: print("unknown Sketch Tool selected:" + toolName); break;
        }
    }
    public bool AcceptPressed()
    {
        if (Sketch.HullIsClosed())
        {
            print("Sketch is closed. Leaving sketch.");

            Sketch.UpdateVertices();
            Sketch.UpdateTriangles();

            return true;
        }
        print("Sketch is not closed. Cannot Accept.");
        return false;
    }
    public bool CancelPressed()
    {
        return true;
    }
    public bool GetPointerPosition(out Vector3 absPos, out Vector3 relPos)
    {
        rayInteractor.TryGetHitInfo(out Vector3 reticlePosition, out _, out _, out bool isValid);

        if (!isValid)
        {
            absPos = Vector3.zero;
            relPos = Vector3.zero;
            return false;
        }

        Vector3 planeRelative = sketchPlane.transform.InverseTransformPoint(reticlePosition);

        if (!snapToGrid)
        {
            absPos = reticlePosition;
            relPos = planeRelative;
            return true;
        }        

        Vector2 textureScale = sketchPlane.GetComponent<Renderer>().material.mainTextureScale;
        float scalingFactor = Math.Min(textureScale.x, textureScale.y);

        float snappedRelativeX = (int)Math.Round((planeRelative.x / (double)(snapSize / scalingFactor) ), MidpointRounding.AwayFromZero ) * (snapSize / scalingFactor);
        float snappedRelativeY = (int)Math.Round((planeRelative.y / (double)(snapSize / scalingFactor) ), MidpointRounding.AwayFromZero) * (snapSize / scalingFactor);

        Vector3 snappedAbsPos = sketchPlane.transform.TransformPoint(snappedRelativeX, snappedRelativeY, 0);
        Vector3 snappedRelPos = new Vector3(snappedRelativeX, snappedRelativeY, 0);

        absPos = snappedAbsPos;
        relPos = snappedRelPos;

        return true;
    }
}