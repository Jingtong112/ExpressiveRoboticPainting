using ExpressivePainting.Robot;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace ExpressivePainting.Demonstration
{
    public class ScriptedDemonstrationController : MonoBehaviour
    {
        public PaintingStyle style = PaintingStyle.SmoothCircle;
        public bool useForHeuristic;
        public float cycleSeconds = 8f;
        public float actionScale = 0.85f;
        public JointController[] joints;

        readonly float[] latestActions = new float[4];
        float elapsed;

        public float[] LatestActions => latestActions;

        public void SetStyle(int styleIndex)
        {
            style = (PaintingStyle)Mathf.Clamp(styleIndex, 0, 2);
            elapsed = 0f;
        }

        public void WriteActions(ActionSegment<float> actions)
        {
            elapsed += Time.deltaTime;
            GenerateActions(elapsed / Mathf.Max(cycleSeconds, 0.01f));

            for (int i = 0; i < actions.Length && i < latestActions.Length; i++)
            {
                actions[i] = latestActions[i];
            }
        }

        void FixedUpdate()
        {
            if (!useForHeuristic || joints == null) return;

            GenerateActions(elapsed / Mathf.Max(cycleSeconds, 0.01f));
            for (int i = 0; i < joints.Length && i < latestActions.Length; i++)
            {
                joints[i].ApplyNormalizedVelocity(latestActions[i]);
            }
        }

        void GenerateActions(float normalizedTime)
        {
            float t = normalizedTime * Mathf.PI * 2f;
            switch (style)
            {
                case PaintingStyle.SmoothCircle:
                    latestActions[0] = Mathf.Sin(t) * actionScale;
                    latestActions[1] = Mathf.Cos(t) * actionScale;
                    latestActions[2] = Mathf.Sin(t + Mathf.PI * 0.5f) * actionScale * 0.65f;
                    latestActions[3] = Mathf.Cos(t * 2f) * actionScale * 0.35f;
                    break;
                case PaintingStyle.SpiralGrowth:
                    float growth = Mathf.Clamp01(normalizedTime);
                    latestActions[0] = Mathf.Sin(t * 1.6f) * Mathf.Lerp(0.25f, actionScale, growth);
                    latestActions[1] = Mathf.Cos(t * 1.6f) * Mathf.Lerp(0.25f, actionScale, growth);
                    latestActions[2] = Mathf.Sin(t * 0.8f) * actionScale * 0.55f;
                    latestActions[3] = Mathf.Cos(t * 1.2f) * actionScale * 0.45f;
                    break;
                case PaintingStyle.OrganicFloral:
                    latestActions[0] = Mathf.Sin(t) * actionScale;
                    latestActions[1] = Mathf.Sin(t * 2.5f) * actionScale * 0.75f;
                    latestActions[2] = Mathf.Cos(t * 5f) * actionScale * 0.65f;
                    latestActions[3] = Mathf.Sin(t * 4f + 0.7f) * actionScale * 0.6f;
                    break;
            }
        }
    }
}
