using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SketchEditor : MonoBehaviour
{
    public static event Action OnPointAdded;

    [SerializeField] private GameObject Reticle_Prefab;
    [SerializeField] private XRRayInteractor RayInteractor;
    [SerializeField] private XRSimpleInteractable Interactable;
    [SerializeField] private XRController Controller;
    [SerializeField] private GameObject Plane;
    [SerializeField] private GameObject PreviewPointParent;
    [SerializeField] private PointDrawer PointDrawer;
    [SerializeField] private LineDrawer LineDrawer;

    [SerializeField] bool SnapToGrid;
    [SerializeField] float SnapSize;

    public Sketch Sketch { get; private set; }
    private GameObject reticle;

    private void Awake()
    {
        reticle = Instantiate(Reticle_Prefab, PreviewPointParent.transform);
        reticle.GetComponent<Renderer>().enabled = false;
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
    }

    public void AddPoint()
    {
        if (GetPointerPosition(out Vector3 absPos, out Vector3 relPos))
        {
            SketchPoint point = Sketch.AddPoint(relPos.x, relPos.y);
            print(point.ID);
            print(point.Position.ToString());
            OnPointAdded?.Invoke();
        }
        else
        {
            print("failed to get point on plane position");
        }
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

        Vector3 PlaneRelative = Plane.transform.InverseTransformPoint(reticlePosition);

        if (!SnapToGrid)
        {
            absPos = reticlePosition;
            relPos = PlaneRelative;
            return true;
        }        

        Vector2 TextureScale = Plane.GetComponent<Renderer>().material.mainTextureScale;
        float ScalingFacor = Math.Min(TextureScale.x, TextureScale.y);

        float SnappedRelativeX = (int)Math.Round((PlaneRelative.x / (double)(SnapSize / ScalingFacor) ), MidpointRounding.AwayFromZero ) * (SnapSize / ScalingFacor);
        float SnappedRelativeY = (int)Math.Round((PlaneRelative.y / (double)(SnapSize / ScalingFacor) ), MidpointRounding.AwayFromZero) * (SnapSize / ScalingFacor);

        // print(SnappedRelativeX.ToString() + "   " + SnappedRelativeY.ToString());

        Vector3 SnappedAbsPos = Plane.transform.TransformPoint(SnappedRelativeX, SnappedRelativeY, 0);
        Vector3 SnappedRelPos = new Vector3(SnappedRelativeX, SnappedRelativeY, 0);

        absPos = SnappedAbsPos;
        relPos = SnappedRelPos;

        return true;
    }
}