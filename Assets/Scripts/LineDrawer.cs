using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] private GameObject LinePrefab;
    [SerializeField] private GameObject ConstructionLinePrefab;
    [SerializeField] private GameObject LineParent;
    [SerializeField] private GameObject SketchPlane;
    [SerializeField] private SketchEditor SketchEditor;


    public List<GameObject> LineObjects;
    private Sketch Sketch = null;
    private List<uint> CurrentLineIds;

    // Start is called before the first frame update
    private void Awake()
    {
        LineObjects = new List<GameObject>();
        CurrentLineIds = new List<uint>();
    }

    public void SetSketch(Sketch sketch)
    {
        Sketch = sketch;
        Sketch.OnLineAdded += Editor_OnLineAdded;
        DestroyAll();
        DrawAll();
    }

    private void DestroyAll()
    {
        foreach (GameObject point in LineObjects)
        {
            Destroy(point);
        }
        LineObjects.Clear();
        CurrentLineIds.Clear();
    }

    private void Editor_OnLineAdded()
    {
        DrawAll();
    }

    private void DrawAll()
    {
        DrawLines(Sketch.Lines, LinePrefab);
        DrawLines(Sketch.ConstructionLines, ConstructionLinePrefab);
    }

    // Update is called once per frame
    void DrawLines(List<SketchLine> lines, GameObject prefab)
    {
        foreach (SketchLine line in lines)
        {
            if (!CurrentLineIds.Contains(line.ID))
            {
                Vector3 lineStart = SketchPlane.transform.TransformPoint(line.Points[0].Position.x, line.Points[0].Position.y, 0);
                Vector3 lineEnd = SketchPlane.transform.TransformPoint(line.Points[1].Position.x, line.Points[1].Position.y, 0);

                GameObject createdLineObject = Instantiate(prefab, Vector3.zero, Quaternion.identity, LineParent.transform);
                LineController createdLine = createdLineObject.GetComponent<LineController>();
                createdLine.SetPoints(lineStart, lineEnd);

                CurrentLineIds.Add(line.ID);

                LineObjects.Add(createdLineObject);
            }
        }
    }
}
