using ExpressivePainting.Robot;
using UnityEngine;

namespace ExpressivePainting.Painting
{
    public class ArtisticStyleTestDriver : MonoBehaviour
    {
        public Transform brushTip;
        public CanvasPainter canvasPainter;
        public Transform canvasPlane;
        public PaintingStyle style = PaintingStyle.SmoothCircle;
        public bool playOnStart = true;
        public float durationSeconds = 8f;
        public float canvasOffset = 0.012f;
        public float circleRadius = 0.42f;
        public float spiralStartRadius = 0.04f;
        public float spiralEndRadius = 0.50f;
        public float floralBaseRadius = 0.30f;
        public float floralPetalAmplitude = 0.15f;
        public float brushWidth = 0.03f;
        public bool configurePresentationView = true;

        Transform baseJoint;
        Transform shoulderJoint;
        Transform elbowJoint;
        Transform wristJoint;
        Transform baseVisual;
        Transform upperArm;
        Transform forearm;
        Transform wrist;
        Transform visualRoot;
        Transform baseCap;
        Transform shoulderCap;
        Transform elbowCap;
        Transform wristCap;
        Transform brushFerrule;
        Transform brushNib;
        Transform upperArmAccent;
        Transform forearmAccent;
        Transform upperArmShadow;
        Transform forearmShadow;
        Transform wristShadow;
        Transform upperArmSideRail;
        Transform forearmSideRail;
        Transform shoulderRing;
        Transform elbowRing;
        Transform wristRing;
        float elapsed;
        bool isRunning;

        public void PlayStyle(PaintingStyle selectedStyle)
        {
            ApplyPresentationDrawingScale();
            style = selectedStyle;
            elapsed = 0f;
            isRunning = true;
            CacheRobotParts();
            DisableRuntimeJointControllers();
            canvasPainter?.ClearCanvas();
            ApplyPaintingStyle(selectedStyle);
        }

        public void Stop()
        {
            isRunning = false;
        }

        void Start()
        {
            ApplyPresentationDrawingScale();
            ConfigurePresentationView();
            CacheRobotParts();
            DisableRuntimeJointControllers();
            if (playOnStart)
            {
                PlayStyle(style);
            }
        }

        void Update()
        {
            if (!isRunning || canvasPainter == null || canvasPlane == null) return;

            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / Mathf.Max(durationSeconds, 0.01f));
            Vector2 canvasPoint = SamplePoint(style, progress);
            Vector3 paintPoint = CanvasToWorld(canvasPoint);

            PoseOriginalArm(paintPoint, progress);
            canvasPainter.AddPaintPoint(paintPoint, StyleColor(style), StyleWidth(style));

            if (progress >= 1f)
            {
                isRunning = false;
            }
        }

        void CacheRobotParts()
        {
            baseJoint = GameObject.Find("BaseJoint")?.transform;
            shoulderJoint = GameObject.Find("ShoulderJoint")?.transform;
            elbowJoint = GameObject.Find("ElbowJoint")?.transform;
            wristJoint = GameObject.Find("WristJoint")?.transform;
            baseVisual = GameObject.Find("BaseVisual")?.transform;
            upperArm = GameObject.Find("UpperArm")?.transform;
            forearm = GameObject.Find("Forearm")?.transform;
            wrist = GameObject.Find("Wrist")?.transform;
            EnsureOriginalArmDecor();
            ApplyOriginalArmMaterials();
        }

        void ConfigurePresentationView()
        {
            if (!configurePresentationView || canvasPlane == null) return;

            canvasPlane.position = new Vector3(0f, 1.05f, 0.34f);
            canvasPlane.rotation = Quaternion.Euler(-90f, 0f, 0f);
            canvasPlane.localScale = new Vector3(0.30f, 0.30f, 0.30f);

            Camera camera = Camera.main;
            if (camera == null) return;

            Vector3 right = canvasPlane.right;
            Vector3 vertical = canvasPlane.forward;
            Vector3 normal = canvasPlane.up;
            Vector3 lookTarget = canvasPlane.position + right * 0.02f + vertical * 0.02f;

            camera.orthographic = true;
            camera.orthographicSize = 1.28f;
            camera.fieldOfView = 38f;
            camera.nearClipPlane = 0.03f;
            camera.farClipPlane = 20f;
            camera.transform.position = canvasPlane.position + normal * 3.10f + right * 0.90f + vertical * 0.44f;
            camera.transform.rotation = Quaternion.LookRotation(lookTarget - camera.transform.position, vertical);
            camera.backgroundColor = new Color(0.72f, 0.80f, 0.88f);
        }

        void ApplyPresentationDrawingScale()
        {
            canvasOffset = 0.012f;
            circleRadius = 0.34f;
            spiralStartRadius = 0.045f;
            spiralEndRadius = 0.38f;
            floralBaseRadius = 0.23f;
            floralPetalAmplitude = 0.095f;
            brushWidth = 0.006f;
        }

        void DisableRuntimeJointControllers()
        {
            GameObject robotRoot = GameObject.Find("RobotRoot");
            if (robotRoot == null) return;

            foreach (var joint in robotRoot.GetComponentsInChildren<JointController>())
            {
                joint.enabled = false;
            }

            var agent = robotRoot.GetComponent<RoboticArmAgent>();
            if (agent != null) agent.enabled = false;
        }

        Vector2 SamplePoint(PaintingStyle selectedStyle, float progress)
        {
            float theta = progress * Mathf.PI * 2f;
            return selectedStyle switch
            {
                PaintingStyle.SpiralGrowth => LayeredSpiral(progress),
                PaintingStyle.OrganicFloral => Butterfly(progress),
                _ => LayeredInkCircle(progress)
            };
        }

        Vector3 CanvasToWorld(Vector2 point)
        {
            return canvasPlane.position
                   + canvasPlane.right * point.x
                   + canvasPlane.forward * point.y
                   + canvasPlane.up * canvasOffset;
        }

        Vector2 LayeredInkCircle(float progress)
        {
            float layerProgress = (progress * 3f) % 1f;
            int layer = Mathf.Min(Mathf.FloorToInt(progress * 3f), 2);
            float theta = layerProgress * Mathf.PI * 2f + layer * 0.42f;
            float radiusScale = layer switch
            {
                0 => 1.00f,
                1 => 0.91f,
                _ => 1.08f
            };
            Vector2 centreOffset = layer switch
            {
                0 => Vector2.zero,
                1 => new Vector2(0.025f, -0.010f),
                _ => new Vector2(-0.018f, 0.018f)
            };

            return InkCircle(theta, radiusScale) + centreOffset;
        }

        Vector2 InkCircle(float theta)
        {
            return InkCircle(theta, 1f);
        }

        Vector2 InkCircle(float theta, float radiusScale)
        {
            float wobble = 1f + 0.040f * Mathf.Sin(theta * 3.0f) + 0.025f * Mathf.Sin(theta * 8.0f);
            float x = Mathf.Cos(theta) * circleRadius * radiusScale * wobble;
            float y = Mathf.Sin(theta) * circleRadius * radiusScale * (0.92f + 0.030f * Mathf.Cos(theta * 5.0f));
            return new Vector2(x, y);
        }

        Vector2 LayeredSpiral(float progress)
        {
            float theta = progress * Mathf.PI * 2f * 5.35f;
            float radius = Mathf.Lerp(spiralStartRadius, spiralEndRadius, progress);
            float overlap = 1f + 0.080f * Mathf.Sin(theta * 1.7f) + 0.030f * Mathf.Sin(theta * 5.0f);
            float x = Mathf.Cos(theta) * radius * overlap;
            float y = Mathf.Sin(theta) * radius * 0.88f * (1f + 0.050f * Mathf.Cos(theta * 2.0f));
            return new Vector2(x, y);
        }

        Vector2 Spiral(float progress, float theta)
        {
            float radius = Mathf.Lerp(spiralStartRadius, spiralEndRadius, progress);
            float expressiveRadius = radius * (1f + 0.055f * Mathf.Sin(theta * 2.2f));
            float x = Mathf.Cos(theta) * expressiveRadius;
            float y = Mathf.Sin(theta) * expressiveRadius * 0.88f;
            return new Vector2(x, y);
        }

        Vector2 Floral(float theta)
        {
            float radius = floralBaseRadius + floralPetalAmplitude * Mathf.Sin(theta * 5f);
            return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * radius;
        }

        Vector2 Butterfly(float progress)
        {
            if (progress < 0.72f)
            {
                return ButterflyOutline(progress / 0.72f);
            }

            return ButterflyInnerWing((progress - 0.72f) / 0.28f);
        }

        Vector2 ButterflyOutline(float progress)
        {
            float theta = progress * Mathf.PI * 12f;
            float wing = Mathf.Exp(Mathf.Cos(theta)) - 2f * Mathf.Cos(4f * theta) - Mathf.Pow(Mathf.Sin(theta / 12f), 5f);
            float x = Mathf.Sin(theta) * wing * 0.105f;
            float y = Mathf.Cos(theta) * wing * 0.085f;
            return new Vector2(x, y - 0.02f);
        }

        Vector2 ButterflyInnerWing(float progress)
        {
            float side = progress < 0.5f ? -1f : 1f;
            float local = progress < 0.5f ? progress * 2f : (progress - 0.5f) * 2f;
            float theta = local * Mathf.PI * 2f;
            float x = side * (0.070f + Mathf.Cos(theta) * 0.085f);
            float y = Mathf.Sin(theta) * 0.105f - 0.015f;
            return new Vector2(x, y);
        }

        void PoseOriginalArm(Vector3 paintPoint, float progress)
        {
            if (baseJoint == null || shoulderJoint == null || elbowJoint == null || wristJoint == null || brushTip == null || canvasPlane == null) return;

            Vector3 right = canvasPlane.right;
            Vector3 up = canvasPlane.forward;
            Vector3 normal = canvasPlane.up;

            Vector3 basePos = canvasPlane.position - right * 0.54f - up * 0.42f + normal * 0.36f;
            Vector3 shoulderPos = basePos + up * 0.22f + normal * 0.02f;
            Vector3 target = paintPoint + normal * 0.035f;
            Vector3 wristPos = Vector3.Lerp(shoulderPos + right * 0.20f + up * 0.16f + normal * 0.08f, target, 0.90f);
            Vector3 elbowPos = SolveElbow(shoulderPos, wristPos, right, up, progress);

            SetWorld(baseJoint, basePos);
            SetWorld(shoulderJoint, shoulderPos);
            SetWorld(elbowJoint, elbowPos);
            SetWorld(wristJoint, wristPos);
            SetWorld(brushTip, target);

            SetLink(baseVisual, basePos - up * 0.10f, shoulderPos, 0.18f);
            Vector3 upperSide = LinkSide(shoulderPos, elbowPos, normal);
            Vector3 forearmSide = LinkSide(elbowPos, wristPos, normal);
            Vector3 wristSide = LinkSide(wristPos, target, normal);

            SetLink(upperArmShadow, shoulderPos, elbowPos, 0.17f, -upperSide * 0.035f - up * 0.025f);
            SetLink(forearmShadow, elbowPos, wristPos, 0.15f, -forearmSide * 0.03f - up * 0.022f);
            SetLink(wristShadow, wristPos, target, 0.088f, -wristSide * 0.02f - up * 0.015f);

            SetLink(upperArm, shoulderPos, elbowPos, 0.17f);
            SetLink(forearm, elbowPos, wristPos, 0.155f);
            SetLink(wrist, wristPos, target, 0.088f);

            SetDecorSphere(baseCap, basePos, 0.24f, 0.15f);
            SetDecorSphere(shoulderCap, shoulderPos, 0.21f, 0.13f);
            SetDecorSphere(elbowCap, elbowPos, 0.20f, 0.125f);
            SetDecorSphere(wristCap, wristPos, 0.15f, 0.095f);
            SetDisc(shoulderRing, shoulderPos + normal * 0.018f, normal, 0.25f, 0.030f);
            SetDisc(elbowRing, elbowPos + normal * 0.018f, normal, 0.23f, 0.028f);
            SetDisc(wristRing, wristPos + normal * 0.016f, normal, 0.18f, 0.024f);
            SetDecorSphere(brushFerrule, target + normal * 0.015f, 0.10f, 0.08f);
            SetDecorSphere(brushNib, paintPoint + normal * 0.016f, 0.055f, 0.050f);

            SetLink(upperArmAccent, shoulderPos, elbowPos, 0.038f, upperSide * 0.07f + normal * 0.025f);
            SetLink(forearmAccent, elbowPos, wristPos, 0.034f, forearmSide * -0.065f + normal * 0.025f);
            SetLink(upperArmSideRail, shoulderPos, elbowPos, 0.030f, upperSide * -0.075f + normal * 0.02f);
            SetLink(forearmSideRail, elbowPos, wristPos, 0.028f, forearmSide * 0.068f + normal * 0.02f);
        }

        Vector3 LinkSide(Vector3 start, Vector3 end, Vector3 normal)
        {
            Vector3 direction = end - start;
            if (direction.sqrMagnitude < 0.0001f) return Vector3.right;
            return Vector3.Cross(normal, direction.normalized).normalized;
        }

        Vector3 SolveElbow(Vector3 shoulder, Vector3 wristTarget, Vector3 right, Vector3 up, float progress)
        {
            Vector3 delta = wristTarget - shoulder;
            float x = Vector3.Dot(delta, right);
            float y = Vector3.Dot(delta, up);
            float upperLength = 0.56f;
            float lowerLength = 0.54f;
            float distance = Mathf.Clamp(new Vector2(x, y).magnitude, 0.10f, upperLength + lowerLength - 0.02f);
            float baseAngle = Mathf.Atan2(y, x);
            float bend = Mathf.Acos(Mathf.Clamp((upperLength * upperLength + distance * distance - lowerLength * lowerLength) / (2f * upperLength * distance), -1f, 1f));
            float expressiveOffset = Mathf.Sin(progress * Mathf.PI * 2f) * 0.10f;
            float elbowAngle = baseAngle + bend + expressiveOffset;
            return shoulder + (right * Mathf.Cos(elbowAngle) + up * Mathf.Sin(elbowAngle)) * upperLength;
        }

        void SetWorld(Transform target, Vector3 position)
        {
            target.position = position;
        }

        void SetLink(Transform link, Vector3 start, Vector3 end, float radius)
        {
            SetLink(link, start, end, radius, Vector3.zero);
        }

        void SetLink(Transform link, Vector3 start, Vector3 end, float radius, Vector3 offset)
        {
            if (link == null) return;
            Vector3 delta = end - start;
            if (delta.sqrMagnitude < 0.0001f) return;
            link.position = start + delta * 0.5f + offset;
            link.rotation = Quaternion.FromToRotation(Vector3.up, delta.normalized);
            float lengthScale = UsesCylinderMesh(link) ? delta.magnitude * 0.5f : delta.magnitude;
            link.localScale = new Vector3(radius, lengthScale, radius);
        }

        void SetDecorSphere(Transform target, Vector3 position, float width, float height)
        {
            if (target == null) return;
            target.position = position;
            target.localScale = new Vector3(width, height, width);
        }

        void SetDisc(Transform target, Vector3 position, Vector3 normal, float diameter, float thickness)
        {
            if (target == null) return;
            target.position = position;
            target.rotation = Quaternion.FromToRotation(Vector3.up, normal.normalized);
            target.localScale = new Vector3(diameter, thickness, diameter);
        }

        bool UsesCylinderMesh(Transform target)
        {
            var meshFilter = target.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null) return false;
            return meshFilter.sharedMesh.name.ToLowerInvariant().Contains("cylinder");
        }

        void ApplyOriginalArmMaterials()
        {
            ApplyColor(baseVisual, new Color(0.10f, 0.11f, 0.12f));
            ApplyColor(upperArm, new Color(0.78f, 0.86f, 0.92f));
            ApplyColor(forearm, new Color(0.82f, 0.89f, 0.94f));
            ApplyColor(wrist, new Color(0.08f, 0.44f, 0.62f));
            ApplyColor(brushTip, Color.black);
            ApplyColor(baseCap, new Color(0.08f, 0.09f, 0.10f));
            ApplyColor(shoulderCap, new Color(0.08f, 0.36f, 0.54f));
            ApplyColor(elbowCap, new Color(0.08f, 0.36f, 0.54f));
            ApplyColor(wristCap, new Color(0.07f, 0.08f, 0.09f));
            ApplyColor(brushFerrule, new Color(0.10f, 0.11f, 0.12f));
            ApplyColor(brushNib, new Color(0.02f, 0.02f, 0.02f));
            ApplyColor(upperArmAccent, new Color(0.10f, 0.50f, 0.70f));
            ApplyColor(forearmAccent, new Color(0.10f, 0.50f, 0.70f));
            ApplyColor(upperArmShadow, new Color(0.24f, 0.29f, 0.34f));
            ApplyColor(forearmShadow, new Color(0.24f, 0.29f, 0.34f));
            ApplyColor(wristShadow, new Color(0.16f, 0.20f, 0.24f));
            ApplyColor(upperArmSideRail, new Color(0.20f, 0.23f, 0.27f));
            ApplyColor(forearmSideRail, new Color(0.20f, 0.23f, 0.27f));
            ApplyColor(shoulderRing, new Color(0.04f, 0.05f, 0.06f));
            ApplyColor(elbowRing, new Color(0.04f, 0.05f, 0.06f));
            ApplyColor(wristRing, new Color(0.04f, 0.05f, 0.06f));
        }

        void EnsureOriginalArmDecor()
        {
            GameObject robotRoot = GameObject.Find("RobotRoot");
            if (robotRoot == null) return;

            visualRoot = robotRoot.transform.Find("OriginalArmVisualDetails");
            if (visualRoot == null)
            {
                visualRoot = new GameObject("OriginalArmVisualDetails").transform;
                visualRoot.SetParent(robotRoot.transform, false);
            }

            baseCap = EnsurePrimitive("BaseServoHousing", PrimitiveType.Sphere, visualRoot);
            shoulderCap = EnsurePrimitive("ShoulderServoHousing", PrimitiveType.Sphere, visualRoot);
            elbowCap = EnsurePrimitive("ElbowServoHousing", PrimitiveType.Sphere, visualRoot);
            wristCap = EnsurePrimitive("WristServoHousing", PrimitiveType.Sphere, visualRoot);
            brushFerrule = EnsurePrimitive("BrushFerrule", PrimitiveType.Sphere, visualRoot);
            brushNib = EnsurePrimitive("BrushNib", PrimitiveType.Sphere, visualRoot);
            upperArmAccent = EnsurePrimitive("UpperArmBlueCable", PrimitiveType.Cylinder, visualRoot);
            forearmAccent = EnsurePrimitive("ForearmBlueCable", PrimitiveType.Cylinder, visualRoot);
            upperArmShadow = EnsurePrimitive("UpperArmDepthShadow", PrimitiveType.Cylinder, visualRoot);
            forearmShadow = EnsurePrimitive("ForearmDepthShadow", PrimitiveType.Cylinder, visualRoot);
            wristShadow = EnsurePrimitive("WristDepthShadow", PrimitiveType.Cylinder, visualRoot);
            upperArmSideRail = EnsurePrimitive("UpperArmSideRail", PrimitiveType.Cylinder, visualRoot);
            forearmSideRail = EnsurePrimitive("ForearmSideRail", PrimitiveType.Cylinder, visualRoot);
            shoulderRing = EnsurePrimitive("ShoulderLayeredServoRing", PrimitiveType.Cylinder, visualRoot);
            elbowRing = EnsurePrimitive("ElbowLayeredServoRing", PrimitiveType.Cylinder, visualRoot);
            wristRing = EnsurePrimitive("WristLayeredServoRing", PrimitiveType.Cylinder, visualRoot);
        }

        Transform EnsurePrimitive(string objectName, PrimitiveType primitiveType, Transform parent)
        {
            Transform existing = parent.Find(objectName);
            if (existing != null) return existing;

            GameObject created = GameObject.CreatePrimitive(primitiveType);
            created.name = objectName;
            created.transform.SetParent(parent, false);

            var collider = created.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            return created.transform;
        }

        void ApplyColor(Transform target, Color color)
        {
            if (target == null) return;
            var renderer = target.GetComponent<Renderer>();
            if (renderer == null) return;
            renderer.material.color = color;
        }

        Color StyleColor(PaintingStyle selectedStyle)
        {
            return selectedStyle switch
            {
                PaintingStyle.SpiralGrowth => new Color(0.12f, 0.02f, 0.46f),
                PaintingStyle.OrganicFloral => new Color(0.88f, 0.10f, 0.45f),
                _ => new Color(0.04f, 0.05f, 0.06f)
            };
        }

        Color StyleEndColor(PaintingStyle selectedStyle)
        {
            return selectedStyle switch
            {
                PaintingStyle.SpiralGrowth => new Color(0.00f, 0.88f, 1.00f),
                PaintingStyle.OrganicFloral => new Color(1.00f, 0.66f, 0.18f),
                _ => new Color(0.30f, 0.33f, 0.36f)
            };
        }

        float StyleWidth(PaintingStyle selectedStyle)
        {
            return selectedStyle switch
            {
                PaintingStyle.SpiralGrowth => brushWidth * 1.05f,
                PaintingStyle.OrganicFloral => brushWidth,
                _ => brushWidth * 1.75f
            };
        }

        AnimationCurve StyleWidthCurve(PaintingStyle selectedStyle)
        {
            return selectedStyle switch
            {
                PaintingStyle.SpiralGrowth => new AnimationCurve(
                    new Keyframe(0.00f, 0.90f),
                    new Keyframe(0.50f, 1.00f),
                    new Keyframe(1.00f, 0.95f)
                ),
                PaintingStyle.OrganicFloral => new AnimationCurve(
                    new Keyframe(0.00f, 0.90f),
                    new Keyframe(0.50f, 1.00f),
                    new Keyframe(1.00f, 0.90f)
                ),
                _ => new AnimationCurve(
                    new Keyframe(0.00f, 0.90f),
                    new Keyframe(0.50f, 1.00f),
                    new Keyframe(1.00f, 0.90f)
                )
            };
        }

        void ApplyPaintingStyle(PaintingStyle selectedStyle)
        {
            if (canvasPainter == null) return;
            canvasPainter.SetBrushStyle(
                StyleColor(selectedStyle),
                StyleEndColor(selectedStyle),
                StyleWidth(selectedStyle),
                StyleWidthCurve(selectedStyle)
            );
        }
    }
}
