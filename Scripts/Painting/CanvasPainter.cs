using System.Collections.Generic;
using UnityEngine;

namespace ExpressivePainting.Painting
{
    public class CanvasPainter : MonoBehaviour
    {
        public Transform canvasPlane;
        public StrokeRenderer strokePrefab;
        public float minPointDistance = 0.01f;
        public int maxVisiblePoints = 900;

        readonly List<Vector3> points = new();
        StrokeRenderer activeStroke;
        Color brushStartColor = Color.black;
        Color brushEndColor = Color.black;
        float brushWidth = 0.03f;
        AnimationCurve brushWidthCurve;
        bool hasCustomBrushStyle;

        public IReadOnlyList<Vector3> Points => points;

        public bool TryProjectToCanvas(Vector3 worldPoint, float maxDistance, out Vector3 projectedPoint)
        {
            if (canvasPlane == null)
            {
                projectedPoint = worldPoint;
                return false;
            }

            Vector3 normal = canvasPlane.up;
            Plane plane = new Plane(normal, canvasPlane.position);
            float distance = plane.GetDistanceToPoint(worldPoint);
            projectedPoint = worldPoint - normal * distance;
            return Mathf.Abs(distance) <= maxDistance;
        }

        public void AddPaintPoint(Vector3 worldPoint, Color color, float width)
        {
            if (!hasCustomBrushStyle)
            {
                brushStartColor = color;
                brushEndColor = color;
                brushWidth = width;
            }
            if (maxVisiblePoints > 0 && points.Count >= maxVisiblePoints)
            {
                ClearCanvas();
            }

            if (points.Count > 0 && Vector3.Distance(points[^1], worldPoint) < minPointDistance)
            {
                return;
            }

            if (activeStroke == null)
            {
                if (strokePrefab == null)
                {
                    Debug.LogError("CanvasPainter cannot draw because Stroke Prefab is not assigned.", this);
                    return;
                }

                activeStroke = Instantiate(strokePrefab, transform);
                activeStroke.gameObject.name = "RuntimeStroke";
                activeStroke.Configure(brushStartColor, brushEndColor, brushWidth, brushWidthCurve);
            }

            points.Add(worldPoint);
            activeStroke.SetPoints(points);
        }

        public void SetBrushStyle(Color startColor, Color endColor, float width, AnimationCurve widthCurve)
        {
            brushStartColor = startColor;
            brushEndColor = endColor;
            brushWidth = width;
            brushWidthCurve = widthCurve;
            hasCustomBrushStyle = true;
        }

        public void ClearCanvas()
        {
            points.Clear();

            foreach (var stroke in FindObjectsByType<StrokeRenderer>(FindObjectsSortMode.None))
            {
                if (stroke == null || stroke == strokePrefab) continue;
                if (stroke.gameObject.name == "StrokeRendererPrefab") continue;
                Destroy(stroke.gameObject);
            }

            if (activeStroke != null)
            {
                Destroy(activeStroke.gameObject);
                activeStroke = null;
            }
        }
    }
}
