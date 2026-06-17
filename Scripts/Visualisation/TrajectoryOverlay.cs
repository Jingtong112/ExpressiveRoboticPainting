using System.Collections.Generic;
using UnityEngine;

namespace ExpressivePainting.Visualisation
{
    public class TrajectoryOverlay : MonoBehaviour
    {
        public LineRenderer demonstrationLine;
        public LineRenderer reproductionLine;
        public Color demonstrationColor = new(0.1f, 0.35f, 1f);
        public Color reproductionColor = new(1f, 0.25f, 0.15f);

        public void Show(IReadOnlyList<Vector3> demonstration, IReadOnlyList<Vector3> reproduction)
        {
            Configure(demonstrationLine, demonstrationColor, demonstration);
            Configure(reproductionLine, reproductionColor, reproduction);
        }

        static void Configure(LineRenderer line, Color color, IReadOnlyList<Vector3> points)
        {
            if (line == null || points == null) return;
            line.positionCount = points.Count;
            line.startColor = color;
            line.endColor = color;
            line.startWidth = 0.015f;
            line.endWidth = 0.015f;
            for (int i = 0; i < points.Count; i++) line.SetPosition(i, points[i]);
        }
    }
}
