using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawPointOnSketch : MonoBehaviour
{

    [SerializeField] GameObject Reticle;
    [SerializeField] XRRayInteractor m_RayInteractor;
    [SerializeField] GameObject Plane;
    public XRRayInteractor rayInteractor => m_RayInteractor;

    [SerializeField] bool SnapToGrid = true;
    [SerializeField] float GridSize = 25;


    private void Update()
    {
        if (GetPointerPosition(out Vector3 absPos, out Vector3 relPos))
        {
            Reticle.GetComponent<Renderer>().enabled = true;
            Reticle.transform.position = absPos;
        }
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

        Vector3 PlaneRelative = Plane.transform.InverseTransformPoint(reticlePosition);

        if (!SnapToGrid)
        {
            absPos = reticlePosition;
            relPos = PlaneRelative;
            return true;
        }        

        float SnappedRelativeX = (int)Math.Round((PlaneRelative.x / GridSize)) * GridSize;
        float SnappedRelativeY = (int)Math.Round((PlaneRelative.y / GridSize)) * GridSize;

        Vector3 SnappedReticlePos = Plane.transform.TransformPoint(SnappedRelativeX, SnappedRelativeY, 0);

        absPos = SnappedReticlePos;
        relPos = PlaneRelative;

        return true;
    }
}