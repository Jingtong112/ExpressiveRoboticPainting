using System.Collections.Generic;
using UnityEngine;

namespace ExpressivePainting.Evaluation
{
    public static class ErrorMetricCalculator
    {
        public static float MeanTrajectoryError(IReadOnlyList<Vector3> reference, IReadOnlyList<Vector3> reproduction)
        {
            var a = TrajectoryResampler.Resample(reference, 100);
            var b = TrajectoryResampler.Resample(reproduction, 100);
            if (a.Count == 0 || b.Count == 0) return float.PositiveInfinity;

            float total = 0f;
            for (int i = 0; i < a.Count; i++) total += Vector3.Distance(a[i], b[i]);
            return total / a.Count;
        }

        public static float ReproductionAccuracy(float meanError, float canvasDiagonal)
        {
            float normalized = Mathf.Clamp01(meanError / Mathf.Max(canvasDiagonal, 0.0001f));
            return Mathf.Clamp01(1f - normalized) * 100f;
        }
    }
}
