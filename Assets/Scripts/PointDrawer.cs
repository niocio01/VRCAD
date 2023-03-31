using System.Collections.Generic;
using UnityEngine;

public class PointDrawer : MonoBehaviour
{
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private GameObject pointParent;
    [SerializeField] private GameObject sketchPlane;
    public List<GameObject> pointsObjets;

    private Sketch _sketch;
    private List<uint> _currentPointIds;
    
    private void Awake()
    {
        Sketch.OnPointAdded += Editor_OnPointAdded;
        _currentPointIds = new List<uint>();
    }
    public void SetSketch(Sketch sketch)
    {
        _sketch = sketch;
        DestroyAll();
        DrawPoints();
    }
    private void DestroyAll()
    {
        foreach (GameObject point in pointsObjets)
        {
            Destroy(point);
        }
        pointsObjets.Clear();
        _currentPointIds.Clear();
    }
    private void Editor_OnPointAdded()
    {
        DrawPoints();
    }
    private void DrawPoints()
    {
        foreach (SketchPoint point in _sketch.Points)
        {
            if (!_currentPointIds.Contains(point.ID))
            {
                Vector3 pointPosition = sketchPlane.transform.TransformPoint(point.Position.x, point.Position.y, 0);
                GameObject createdPoint = Instantiate(pointPrefab, pointPosition, Quaternion.identity, pointParent.transform);
                _currentPointIds.Add(point.ID);

                pointsObjets.Add(createdPoint);
            }
        }
    }
}
