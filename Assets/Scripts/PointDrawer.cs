using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointDrawer : MonoBehaviour
{
    [SerializeField] GameObject pointObject;
    [SerializeField] GameObject DrawingPlane;
    [SerializeField] private SketchEditor editor;

    public List<GameObject> PointsObjets; // used for inspector access
    private Sketch sketch;
    private List<int> currentPointIds;


    // Start is called before the first frame update
    private void Start()
    {
        sketch = editor.sketch;
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
                Vector3 pointPosition = DrawingPlane.transform.TransformPoint(point.Position.x, 0, point.Position.y);
                GameObject createdPoint = Instantiate(pointObject, pointPosition, Quaternion.identity);
                currentPointIds.Add(point.ID);

                PointsObjets.Add(createdPoint);
            }
        }
    }
}
