using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject lineParent;
    [SerializeField] private GameObject sketchPlane;
    [SerializeField] private SketchEditor sketchEditor;
    

    public List<GameObject> LineObjects;
    private Sketch Sketch;
    private List<uint> CurrentLineIds;

    // Start is called before the first frame update
    private void Start()
    {
        SketchEditor.OnPointAdded += Editor_OnPointAdded;
        CurrentLineIds = new List<uint>();
    }

    public void SetSketch(Sketch sketch)
    {
        Sketch = sketch;
        DestroyAll();
        DrawLines();
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

    private void Editor_OnPointAdded()
    {
        DrawLines();
    }

    // Update is called once per frame
    void DrawLines()
    {
        foreach (SketchLine line in Sketch.Lines)
        {
            if (!CurrentLineIds.Contains(line.ID))
            {
                Vector3 lineStart = sketchPlane.transform.TransformPoint(line.Points[0].Position.x, line.Points[0].Position.y, 0);
                Vector3 lineEnd = sketchPlane.transform.TransformPoint(line.Points[1].Position.x, line.Points[1].Position.y, 0);

                GameObject createdLineObject = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, lineParent.transform);
                LineController createdLine = createdLineObject.GetComponent<LineController>();
                createdLine.SetPoints(lineStart, lineEnd);

                CurrentLineIds.Add(line.ID);

                LineObjects.Add(createdLineObject);
            }
        }
    }
}
