using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointDrawer : MonoBehaviour
{
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private GameObject pointParent;
    [SerializeField] private GameObject sketchPlane;
    [SerializeField] private SketchEditor sketchEditor;
    

    public List<GameObject> PointsObjets; // used for inspector access
    private Sketch sketch;
    private List<int> currentPointIds;



    // Start is called before the first frame update
    private void Start()
    {
        sketch = sketchEditor.sketch;
        SketchEditor.OnPointAdded += Editor_OnPointAdded;
        currentPointIds = new List<int>();
    }

    private void Editor_OnPointAdded()
    {
        DrawPoints();
    }

    // Update is called once per frame
    void DrawPoints()
    {
        print("Points: " + sketch.Points.Count.ToString() );

        foreach (SketchPoint point in sketch.Points)
        {
            if (!currentPointIds.Contains(point.ID))
            {
                Vector3 pointPosition = sketchPlane.transform.TransformPoint(point.Position.x, 0, point.Position.y);
                GameObject createdPoint = Instantiate(pointPrefab, pointPosition, Quaternion.identity, pointParent.transform);
                currentPointIds.Add(point.ID);

                PointsObjets.Add(createdPoint);
            }
        }
    }
}
