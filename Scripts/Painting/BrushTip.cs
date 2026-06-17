using UnityEngine;

namespace ExpressivePainting.Painting
{
    public class BrushTip : MonoBehaviour
    {
        public CanvasPainter painter;
        public float contactDistance = 0.035f;
        public Color paintColor = Color.black;
        public float brushWidth = 0.025f;
        public float minSpeedToPaint = 0.002f;

        Vector3 previousPosition;

        void Awake()
        {
            previousPosition = transform.position;
        }

        void FixedUpdate()
        {
            if (painter == null) return;

            float speed = Vector3.Distance(transform.position, previousPosition) / Mathf.Max(Time.fixedDeltaTime, 0.0001f);
            previousPosition = transform.position;
            if (speed < minSpeedToPaint)
            {
                return;
            }

            if (painter.TryProjectToCanvas(transform.position, contactDistance, out Vector3 canvasPoint))
            {
                painter.AddPaintPoint(canvasPoint, paintColor, brushWidth);
            }
        }
    }
}
