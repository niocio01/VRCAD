using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingPlane : MonoBehaviour
{
    [SerializeField] Transform PlaneTransform;
    public Plane Plane
    {
        private set; get;
    }

    private void Start()
    {
        Plane = new Plane(PlaneTransform.up, PlaneTransform.position);
        transform.position = PlaneTransform.position;
        transform.up = PlaneTransform.up;
    }
}
