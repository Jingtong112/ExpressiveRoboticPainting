using System.Collections.Generic;
using UnityEngine;

namespace ExpressivePainting.Evaluation
{
    public static class PathSimilarityCalculator
    {
        public static float SimilarityScore(IReadOnlyList<Vector3> reference, IReadOnlyList<Vector3> reproduction, float canvasDiagonal)
        {
            float error = ErrorMetricCalculator.MeanTrajectoryError(reference, reproduction);
            return ErrorMetricCalculator.ReproductionAccuracy(error, canvasDiagonal);
        }

        public static float PathLength(IReadOnlyList<Vector3> points)
        {
            float length = 0f;
            for (int i = 1; i < points.Count; i++) length += Vector3.Distance(points[i - 1], points[i]);
            return length;
        }

        public static float DirectionChangeScore(IReadOnlyList<Vector3> points)
        {
            if (points.Count < 3) return 0f;
            float total = 0f;
            int samples = 0;
            for (int i = 2; i < points.Count; i++)
            {
                Vector3 a = (points[i - 1] - points[i - 2]).normalized;
                Vector3 b = (points[i] - points[i - 1]).normalized;
                total += Vector3.Angle(a, b);
                samples++;
            }
            return total / Mathf.Max(samples, 1);
        }
    }
}
