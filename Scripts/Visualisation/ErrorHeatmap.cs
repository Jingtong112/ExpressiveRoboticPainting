using System.Collections.Generic;
using ExpressivePainting.Evaluation;
using UnityEngine;

namespace ExpressivePainting.Visualisation
{
    public class ErrorHeatmap : MonoBehaviour
    {
        public GameObject markerPrefab;
        public Transform markerParent;
        public Gradient errorGradient;
        public float maxExpectedError = 0.15f;

        public void Render(IReadOnlyList<Vector3> demonstration, IReadOnlyList<Vector3> reproduction)
        {
            foreach (Transform child in markerParent) Destroy(child.gameObject);

            var a = TrajectoryResampler.Resample(demonstration, 100);
            var b = TrajectoryResampler.Resample(reproduction, 100);
            for (int i = 0; i < a.Count; i++)
            {
                float error = Vector3.Distance(a[i], b[i]);
                GameObject marker = Instantiate(markerPrefab, a[i], Quaternion.identity, markerParent);
                var renderer = marker.GetComponent<Renderer>();
                if (renderer != null) renderer.material.color = errorGradient.Evaluate(Mathf.Clamp01(error / maxExpectedError));
                marker.transform.localScale = Vector3.one * Mathf.Lerp(0.01f, 0.05f, Mathf.Clamp01(error / maxExpectedError));
            }
        }
    }
}
