using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using ExpressivePainting.Demonstration;
using UnityEngine;

namespace ExpressivePainting.Robot
{
    public enum PaintingStyle
    {
        SmoothCircle = 0,
        SpiralGrowth = 1,
        OrganicFloral = 2
    }

    public class RoboticArmAgent : Agent
    {
        [Header("Robot joints")]
        public JointController[] joints;
        public Transform brushTip;
        public Transform canvasOrigin;

        [Header("Episode")]
        public float episodeSeconds = 12f;
        public PaintingStyle style;
        public bool includeStyleObservation = false;
        public ScriptedDemonstrationController scriptedDemonstration;

        float elapsed;
        Vector3 previousBrushPosition;

        public override void Initialize()
        {
            previousBrushPosition = brushTip.position;
        }

        public override void OnEpisodeBegin()
        {
            elapsed = 0f;
            foreach (var joint in joints)
            {
                joint.ResetJoint();
            }

            previousBrushPosition = brushTip.position;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            foreach (var joint in joints)
            {
                sensor.AddObservation(joint.NormalizedAngle);
                sensor.AddObservation(joint.NormalizedVelocity);
            }

            Vector3 localBrush = canvasOrigin.InverseTransformPoint(brushTip.position);
            Vector3 localVelocity = canvasOrigin.InverseTransformDirection((brushTip.position - previousBrushPosition) / Mathf.Max(Time.fixedDeltaTime, 0.0001f));
            sensor.AddObservation(localBrush);
            sensor.AddObservation(localVelocity);
            sensor.AddObservation(Mathf.Clamp01(elapsed / episodeSeconds));

            if (includeStyleObservation)
            {
                sensor.AddOneHotObservation((int)style, 3);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            for (int i = 0; i < joints.Length && i < actions.ContinuousActions.Length; i++)
            {
                joints[i].ApplyNormalizedVelocity(actions.ContinuousActions[i]);
            }

            elapsed += Time.fixedDeltaTime;
            previousBrushPosition = brushTip.position;

            if (elapsed >= episodeSeconds)
            {
                EndEpisode();
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var actions = actionsOut.ContinuousActions;
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = 0f;
            }

            if (scriptedDemonstration != null && scriptedDemonstration.useForHeuristic)
            {
                scriptedDemonstration.WriteActions(actions);
                return;
            }

            if (actions.Length > 0) actions[0] = Input.GetAxisRaw("Horizontal");
            if (actions.Length > 1) actions[1] = Input.GetAxisRaw("Vertical");
            if (actions.Length > 2) actions[2] = KeyAxis(KeyCode.Q, KeyCode.E);
            if (actions.Length > 3) actions[3] = KeyAxis(KeyCode.Z, KeyCode.C);
        }

        static float KeyAxis(KeyCode negative, KeyCode positive)
        {
            float value = 0f;
            if (Input.GetKey(negative)) value -= 1f;
            if (Input.GetKey(positive)) value += 1f;
            return value;
        }
    }
}
