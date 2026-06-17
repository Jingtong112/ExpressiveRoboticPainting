using UnityEngine;

namespace ExpressivePainting.Painting
{
    public class PaintTrailManager : MonoBehaviour
    {
        public CanvasPainter[] canvases;

        public void ClearAll()
        {
            foreach (var canvas in canvases)
            {
                canvas.ClearCanvas();
            }
        }
    }
}
