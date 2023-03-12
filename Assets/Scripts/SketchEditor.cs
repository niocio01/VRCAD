using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SketchEditor : MonoBehaviour
{
    public static event Action OnPointAdded;

    [SerializeField] private GameObject Reticle;
    [SerializeField] private XRRayInteractor RayInteractor;
    [SerializeField] private XRSimpleInteractable Interactable;
    [SerializeField] private XRController Controller;
    [SerializeField] private GameObject Plane;

    [SerializeField] bool SnapToGrid;
    [SerializeField] float SnapSize;

    public Sketch sketch;

    private void Awake()
    {
        sketch = new Sketch();
    }

    private void Update()
    {
        if (GetPointerPosition(out Vector3 absPos, out Vector3 relPos))
        {
            Reticle.GetComponent<Renderer>().enabled = true;
            Reticle.transform.position = absPos;
        }
    }

    public void AddPoint()
    {
        if (GetPointerPosition(out Vector3 absPos, out Vector3 relPos))
        {
            int id = sketch.AddPoint(relPos.x, relPos.z);
            print(id);
            print(sketch.Points[id].Position.ToString());
            OnPointAdded?.Invoke();
        }
        else
        {
            print("failed");
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

        float SnappedRelativeX = (int)Math.Round((PlaneRelative.x / (SnapSize * ScalingFacor)) * (SnapSize * ScalingFacor) ) ;
        float SnappedRelativeZ = (int)Math.Round((PlaneRelative.z / (SnapSize * ScalingFacor)) * (SnapSize * ScalingFacor) );

        Vector3 SnappedAbsPos = Plane.transform.TransformPoint(SnappedRelativeX, 0, SnappedRelativeZ);
        Vector3 SnappedRelPos = new Vector3(SnappedRelativeX, 0, SnappedRelativeZ);

        absPos = SnappedAbsPos;
        relPos = SnappedRelPos;

        return true;
    }
}