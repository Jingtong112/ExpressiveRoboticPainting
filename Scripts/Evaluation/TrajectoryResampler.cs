using System.Collections.Generic;
using UnityEngine;

namespace ExpressivePainting.Evaluation
{
    public static class TrajectoryResampler
    {
        public static List<Vector3> Resample(IReadOnlyList<Vector3> points, int count)
        {
            var result = new List<Vector3>();
            if (points == null || points.Count == 0 || count <= 0) return result;
            if (points.Count == 1)
            {
                for (int i = 0; i < count; i++) result.Add(points[0]);
                return result;
            }

            float totalLength = 0f;
            var cumulative = new float[points.Count];
            for (int i = 1; i < points.Count; i++)
            {
                totalLength += Vector3.Distance(points[i - 1], points[i]);
                cumulative[i] = totalLength;
            }

            for (int i = 0; i < count; i++)
            {
                float target = totalLength * i / Mathf.Max(count - 1, 1);
                int segment = 1;
                while (segment < cumulative.Length - 1 && cumulative[segment] < target) segment++;
                float start = cumulative[segment - 1];
                float end = cumulative[segment];
                float t = Mathf.InverseLerp(start, end, target);
                result.Add(Vector3.Lerp(points[segment - 1], points[segment], t));
            }

            return result;
        }
    }
}
