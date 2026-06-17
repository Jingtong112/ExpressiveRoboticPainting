using System.Collections.Generic;
using UnityEngine;

namespace ExpressivePainting.Painting
{
    public class StrokeRenderer : MonoBehaviour
    {
        readonly List<GameObject> segmentObjects = new();
        Color startColor = Color.black;
        Color endColor = Color.black;
        float strokeRadius = 0.006f;
        Material sharedStrokeMaterial;
        int renderedSegmentCount;

        void Awake()
        {
            var legacyLine = GetComponent<LineRenderer>();
            if (legacyLine != null)
            {
                legacyLine.enabled = false;
                legacyLine.positionCount = 0;
            }
        }

        public void Configure(Color color, float width)
        {
            Configure(color, color, width, null);
        }

        public void Configure(Color start, Color end, float width, AnimationCurve widthCurve)
        {
            startColor = start;
            endColor = end;
            strokeRadius = Mathf.Clamp(width * 4.00f, 0.034f, 0.052f);
            renderedSegmentCount = 0;
            sharedStrokeMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color") ?? Shader.Find("Standard"));

            var legacyLine = GetComponent<LineRenderer>();
            if (legacyLine != null)
            {
                legacyLine.enabled = false;
                legacyLine.positionCount = 0;
            }
        }

        public void SetPoints(IReadOnlyList<Vector3> points)
        {
            if (points == null || points.Count < 2) return;

            while (renderedSegmentCount < points.Count - 1)
            {
                int i = renderedSegmentCount;
                CreateSegment(points[i], points[i + 1], i, points.Count - 2);
                renderedSegmentCount++;
            }
        }

        void CreateSegment(Vector3 start, Vector3 end, int index, int maxIndex)
        {
            Vector3 delta = end - start;
            if (delta.sqrMagnitude < 0.000001f) return;

            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            segment.name = "PaintLineSegment";
            segment.transform.SetParent(transform, true);
            segment.transform.position = start + delta * 0.5f;
            segment.transform.rotation = Quaternion.FromToRotation(Vector3.up, delta.normalized);
            segment.transform.localScale = new Vector3(strokeRadius, delta.magnitude * 0.5f, strokeRadius);

            var collider = segment.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            float t = maxIndex <= 0 ? 0f : index / (float)maxIndex;
            Color color = Color.Lerp(startColor, endColor, t);
            ApplyPaintMaterial(segment, color);

            segmentObjects.Add(segment);
            if (index % 7 == 0)
            {
                CreatePaintDeposit(end, color, index);
            }
        }

        void CreatePaintDeposit(Vector3 position, Color color, int index)
        {
            GameObject deposit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            deposit.name = "PaintTextureDot";
            deposit.transform.SetParent(transform, true);
            deposit.transform.position = position;
            float size = strokeRadius * (index % 14 == 0 ? 2.25f : 1.65f);
            deposit.transform.localScale = new Vector3(size, size * 0.45f, size);

            var collider = deposit.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            Color richer = Color.Lerp(color, Color.white, 0.10f);
            ApplyPaintMaterial(deposit, richer);
            segmentObjects.Add(deposit);
        }

        void ApplyPaintMaterial(GameObject target, Color color)
        {
            var renderer = target.GetComponent<Renderer>();
            if (renderer == null) return;

            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            Material material = new Material(sharedStrokeMaterial);
            material.color = color;
            renderer.material = material;
        }

        void OnDestroy()
        {
            for (int i = 0; i < segmentObjects.Count; i++)
            {
                if (segmentObjects[i] != null)
                {
                    Destroy(segmentObjects[i]);
                }
            }
            segmentObjects.Clear();
        }
    }
}
