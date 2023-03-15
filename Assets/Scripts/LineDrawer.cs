using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject lineParent;
    [SerializeField] private GameObject sketchPlane;
    [SerializeField] private SketchEditor sketchEditor;
    

    public List<GameObject> lineObjects; // used for inspector access
    private Sketch sketch;
    private List<int> currentLineIds;

    // Start is called before the first frame update
    private void Start()
    {
        sketch = sketchEditor.sketch;
        SketchEditor.OnPointAdded += Editor_OnPointAdded;
        currentLineIds = new List<int>();
    }

    private void Editor_OnPointAdded()
    {
        DrawPoints();
    }

    // Update is called once per frame
    void DrawPoints()
    {
        print("Points: " + sketch.Points.Count.ToString() );

        foreach (SketchLine line in sketch.Lines)
        {
            if (!currentLineIds.Contains(line.ID))
            {
                Vector3 lineStart = sketchPlane.transform.TransformPoint(line.Points[0].Position.x, 0, line.Points[0].Position.y);
                Vector3 lineEnd = sketchPlane.transform.TransformPoint(line.Points[1].Position.x, 0, line.Points[1].Position.y);

                LineController createdLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, lineParent.transform).GetComponent<LineController>();
                currentLineIds.Add(line.ID);

                // lineObjects.Add(createdLine);
            }
        }
    }
}
