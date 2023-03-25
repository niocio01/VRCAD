using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
public enum SketchTools
{
    Point,
    Line,
    Circle,
};

public class SketchEditor : MonoBehaviour
{
    [SerializeField] public GameObject SketchPlane;

    [SerializeField] private GameObject Reticle_Prefab;
    [SerializeField] private XRRayInteractor RayInteractor;
    [SerializeField] private XRSimpleInteractable Interactable;
    [SerializeField] private XRController Controller;    
    [SerializeField] private GameObject PreviewPointParent;
    [SerializeField] private PointDrawer PointDrawer;
    [SerializeField] private LineDrawer LineDrawer;
    [SerializeField] private LineTool LineTool;
    [SerializeField] private RadioButtonController ToolsGroup;

    [SerializeField] bool SnapToGrid;
    [SerializeField] float SnapSize;

    public Sketch Sketch { get; private set; }
    public SketchTools CurrentTool { get; private set; }
    private GameObject reticle;

    private void Start()
    {
        reticle = Instantiate(Reticle_Prefab, PreviewPointParent.transform);
        reticle.GetComponent<Renderer>().enabled = false;
        CurrentTool = SketchTools.Point;
        ToolsGroup.onSelectionChanged.AddListener(SetTool);
    }

    private void Update()
    {
        if (GetPointerPosition(out Vector3 absPos, out Vector3 relPos))
        {
            reticle.GetComponent<Renderer>().enabled = true;
            reticle.transform.position = absPos;
        }
        else
        {
            reticle.GetComponent<Renderer>().enabled = false;
        }
    }

    public void SetSketch(Sketch sketch)
    {
        Sketch = sketch;
        PointDrawer.SetSketch(sketch);
        LineDrawer.SetSketch(sketch);
    }

    public bool StartEditSketch(Sketch sketch)
    {
        if (sketch == null) return false;

        SetSketch(sketch);

        return true;
    }

    public void SelectEntered()
    {
        switch (CurrentTool) 
        {
            case SketchTools.Point: AddPoint(); break;
            case SketchTools.Line: LineTool.AddPoint(); break;
        }
    }

    public void AddPoint()
    {
        if (GetPointerPosition(out Vector3 absPos, out Vector3 relPos))
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

    public void SetTool(string toolName)
    {
        // Deselect old Tool
        switch (CurrentTool)
        {
            case SketchTools.Point: break;
            case SketchTools.Line: LineTool.EndLine(); break;
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
        RayInteractor.TryGetHitInfo(out Vector3 reticlePosition, out _, out _, out bool isValid);

        if (!isValid)
        {
            absPos = Vector3.zero;
            relPos = Vector3.zero;
            return false;
        }

        Vector3 PlaneRelative = SketchPlane.transform.InverseTransformPoint(reticlePosition);

        if (!SnapToGrid)
        {
            absPos = reticlePosition;
            relPos = PlaneRelative;
            return true;
        }        

        Vector2 TextureScale = SketchPlane.GetComponent<Renderer>().material.mainTextureScale;
        float ScalingFacor = Math.Min(TextureScale.x, TextureScale.y);

        float SnappedRelativeX = (int)Math.Round((PlaneRelative.x / (double)(SnapSize / ScalingFacor) ), MidpointRounding.AwayFromZero ) * (SnapSize / ScalingFacor);
        float SnappedRelativeY = (int)Math.Round((PlaneRelative.y / (double)(SnapSize / ScalingFacor) ), MidpointRounding.AwayFromZero) * (SnapSize / ScalingFacor);

        Vector3 SnappedAbsPos = SketchPlane.transform.TransformPoint(SnappedRelativeX, SnappedRelativeY, 0);
        Vector3 SnappedRelPos = new Vector3(SnappedRelativeX, SnappedRelativeY, 0);

        absPos = SnappedAbsPos;
        relPos = SnappedRelPos;

        return true;
    }
}