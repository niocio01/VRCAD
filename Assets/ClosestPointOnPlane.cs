using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClosestPointOnPlane : MonoBehaviour
{
    [Header("Object references")]
    [SerializeField] DrawingPlane DrawingPlane;
    [SerializeField] Transform ControllerTransform;
    [SerializeField] GameObject DrawingSphere;

    [Header("Settings")]
    [Range(0, 1)] public float ActivationDistance;


    [SerializeField] Vector3 Controller_Offsets;


    // Update is called once per frame
    void Update()
    {
        Vector3 Projection = DrawingPlane.Plane.ClosestPointOnPlane(ControllerTransform.position);  
       

        if (Vector3.Distance(Projection, ControllerTransform.position) < ActivationDistance)
        {
            transform.position = Projection;
            transform.up = DrawingPlane.transform.up;
            transform.Translate(Controller_Offsets);

            DrawingSphere.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            DrawingSphere.GetComponent<Renderer>().enabled = false;
        }
    }
}
