using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointDrawer : MonoBehaviour
{
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private GameObject pointParent;
    [SerializeField] private GameObject sketchPlane;
    [SerializeField] private SketchEditor SketchEditor;
    

    public List<GameObject> PointsObjets;
    private Sketch Sketch;
    private List<uint> CurrentPointIds;



    // Start is called before the first frame update
    private void Awake()
    {
        Sketch.OnPointAdded += Editor_OnPointAdded;
        CurrentPointIds = new List<uint>();
    }

    public void SetSketch(Sketch sketch)
    {
        Sketch = sketch;
        DestroyAll();
        DrawPoints();
    }

    private void DestroyAll()
    {
        foreach (GameObject point in PointsObjets)
        {
            Destroy(point);
        }
        PointsObjets.Clear();
        CurrentPointIds.Clear();
    }

    private void Editor_OnPointAdded()
    {
        DrawPoints();
    }

    void DrawPoints()
    {
        foreach (SketchPoint point in Sketch.Points)
        {
            if (!CurrentPointIds.Contains(point.ID))
            {
                Vector3 pointPosition = sketchPlane.transform.TransformPoint(point.Position.x, point.Position.y, 0);
                GameObject createdPoint = Instantiate(pointPrefab, pointPosition, Quaternion.identity, pointParent.transform);
                CurrentPointIds.Add(point.ID);

                PointsObjets.Add(createdPoint);
            }
        }
    }
}
