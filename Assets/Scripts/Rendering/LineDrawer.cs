using System.Collections.Generic;
using Editors.SketchEdit;
using UnityEngine;

namespace Rendering
{
    public class LineDrawer : MonoBehaviour
    {
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private GameObject constructionLinePrefab;
        [SerializeField] private GameObject lineParent;
        [SerializeField] private GameObject sketchPlane;
        public List<GameObject> lineObjects;

        private Sketch _sketch;
        private List<uint> _currentLineIds;

        private void Awake()
        {
            lineObjects = new List<GameObject>();
            _currentLineIds = new List<uint>();
        }
        public void SetSketch(Sketch sketch)
        {
            _sketch = sketch;
            Sketch.OnLineAdded += Editor_OnLineAdded;
            DestroyAll();
            DrawAll();
        }
        private void DestroyAll()
        {
            foreach (GameObject point in lineObjects)
            {
                Destroy(point);
            }
            lineObjects.Clear();
            _currentLineIds.Clear();
        }
        private void Editor_OnLineAdded()
        {
            DrawAll();
        }
        private void DrawAll()
        {
            DrawLines(_sketch.Lines, linePrefab);
            DrawLines(_sketch.ConstructionLines, constructionLinePrefab);
        }
        private void DrawLines(List<SketchLine> lines, GameObject prefab)
        {
            foreach (SketchLine line in lines)
            {
                if (!_currentLineIds.Contains(line.ID))
                {
                    Vector3 lineStart = sketchPlane.transform.TransformPoint(line.Points[0].Position.x, line.Points[0].Position.y, 0);
                    Vector3 lineEnd = sketchPlane.transform.TransformPoint(line.Points[1].Position.x, line.Points[1].Position.y, 0);

                    GameObject createdLineObject = Instantiate(prefab, Vector3.zero, Quaternion.identity, lineParent.transform);
                    LineController createdLine = createdLineObject.GetComponent<LineController>();
                    createdLine.SetPoints(lineStart, lineEnd);

                    _currentLineIds.Add(line.ID);

                    lineObjects.Add(createdLineObject);
                }
            }
        }
    }
}
