using UnityEngine;

namespace Rendering
{
    public class LineController : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private Vector3[] _points = new Vector3[2];

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 0;
        }
        public void SetPoints(Vector3 first, Vector3 second)
        {
            _points[0] = first;
            _points[1] = second;
            _lineRenderer.positionCount = _points.Length;
        }
        private void LateUpdate()
        {
            _lineRenderer.SetPositions(_points);
        }
    }
}
