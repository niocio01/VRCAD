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

    Vector3 reticlePosition;
    Vector3 reticleNormal;

   
    private void Awake()
    {
        if (m_RayInteractor == null)
        {
            Debug.LogWarning("2Cannot perform reticle position lookup without a valid Ray Interactor set.", this);
        }
    }

    private void OnEnable()
    {
        if (rayInteractor != null)
        {
            rayInteractor.hoverEntered.AddListener(UpdateReticlePosition);
            print("Interaction Enabled");
        }
    }
    void OnDisable()
    {
        if (rayInteractor != null)
        {
            rayInteractor.hoverEntered.RemoveListener(UpdateReticlePosition);
        }
    }

    private void Update()
    {
        rayInteractor.TryGetHitInfo(out reticlePosition, out reticleNormal, out _, out bool isValid);

        if (isValid)
        {
            if (SnapToGrid)
            {
                Vector3 PlaneRelative = Plane.transform.InverseTransformPoint(reticlePosition);
                float SnappedRelativeX = (int)Math.Round((PlaneRelative.x / GridSize)) * GridSize;
                float SnappedRelativeY = (int)Math.Round((PlaneRelative.y / GridSize)) * GridSize;

                Vector3 SnappedReticlePos = Plane.transform.TransformPoint(SnappedRelativeX, SnappedRelativeY, 0);

                print(SnappedReticlePos.ToString());

                Reticle.transform.position = SnappedReticlePos;
                Reticle.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                Reticle.transform.position = reticlePosition;
                Reticle.GetComponent<Renderer>().enabled = true;
            }
        }

        else
        {
            Reticle.GetComponent<Renderer>().enabled = false;
        }
    }

    public void UpdateReticlePosition(HoverEnterEventArgs args)
    {
        print("Reticle Update");


        if(rayInteractor.TryGetHitInfo(out reticlePosition, out reticleNormal, out _, out _))
        {
            Reticle.transform.position = reticlePosition;
        }       
    }
}