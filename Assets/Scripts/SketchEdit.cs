using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SketchEdit : MonoBehaviour
{
    public static event Action<Vector2> OnPointAdded;

    [SerializeField] private GameObject Reticle;
    [SerializeField] private XRRayInteractor RayInteractor;
    [SerializeField] private XRSimpleInteractable Interactable;
    [SerializeField] private XRController Controller;
    [SerializeField] private GameObject Plane;

    [SerializeField] bool SnapToGrid;
    [SerializeField] float SnapSize;

    SketchPoints sketchPoints = new SketchPoints();

    private void Awake()
    {
        // Controller.GetComponent<XRBaseControllerInteractor>().allowHoveredActivate = true;
    }

    public void SelectEntered()
    {
        print("Select Called");
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
        print("Adding Point");
        if (GetPointerPosition(out _, out Vector3 relPos))
        {
            SketchEdit.OnPointAdded?.Invoke(relPos);
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

        Vector3 SnappedReticlePos = Plane.transform.TransformPoint(SnappedRelativeX, 0, SnappedRelativeZ);

        absPos = SnappedReticlePos;
        relPos = PlaneRelative;

        return true;
    }
}