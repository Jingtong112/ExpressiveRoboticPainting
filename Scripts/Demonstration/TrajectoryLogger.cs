using System;
using System.Globalization;
using System.IO;
using System.Text;
using ExpressivePainting.Robot;
using UnityEngine;

namespace ExpressivePainting.Demonstration
{
    public class TrajectoryLogger : MonoBehaviour
    {
        public Transform brushTip;
        public Transform canvasOrigin;
        public JointController[] joints;
        public ManualRobotController manualController;
        public ScriptedDemonstrationController scriptedController;
        public PaintingStyle style;
        public string outputFolder = "Data/demonstrations";

        StringBuilder buffer;
        bool isRecording;
        int episodeId;
        Vector3 previousBrushPosition;

        public bool IsRecording => isRecording;

        public void StartRecording()
        {
            episodeId++;
            buffer = new StringBuilder();
            buffer.AppendLine("timestamp,style,episode,joint_1_angle,joint_2_angle,joint_3_angle,joint_4_angle,joint_1_velocity,joint_2_velocity,joint_3_velocity,joint_4_velocity,brush_x,brush_y,brush_z,brush_vx,brush_vy,brush_vz,action_1,action_2,action_3,action_4");
            previousBrushPosition = brushTip.position;
            isRecording = true;
        }

        public string StopRecording()
        {
            isRecording = false;
            Directory.CreateDirectory(outputFolder);
            string safeStyle = style.ToString().ToLowerInvariant();
            string path = Path.Combine(outputFolder, $"{safeStyle}_episode_{episodeId:000}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            File.WriteAllText(path, buffer.ToString());
            return path;
        }

        void FixedUpdate()
        {
            if (!isRecording || brushTip == null || canvasOrigin == null) return;

            Vector3 local = canvasOrigin.InverseTransformPoint(brushTip.position);
            Vector3 velocity = canvasOrigin.InverseTransformDirection((brushTip.position - previousBrushPosition) / Mathf.Max(Time.fixedDeltaTime, 0.0001f));
            previousBrushPosition = brushTip.position;

            buffer.Append(Float(Time.time)).Append(',');
            buffer.Append(style).Append(',');
            buffer.Append(episodeId).Append(',');

            for (int i = 0; i < 4; i++) buffer.Append(Float(i < joints.Length ? joints[i].NormalizedAngle : 0f)).Append(',');
            for (int i = 0; i < 4; i++) buffer.Append(Float(i < joints.Length ? joints[i].NormalizedVelocity : 0f)).Append(',');

            buffer.Append(Float(local.x)).Append(',');
            buffer.Append(Float(local.y)).Append(',');
            buffer.Append(Float(local.z)).Append(',');
            buffer.Append(Float(velocity.x)).Append(',');
            buffer.Append(Float(velocity.y)).Append(',');
            buffer.Append(Float(velocity.z)).Append(',');

            float[] actions = GetLatestActions();
            for (int i = 0; i < 4; i++)
            {
                buffer.Append(Float(i < actions.Length ? actions[i] : 0f));
                buffer.Append(i == 3 ? '\n' : ',');
            }
        }

        float[] GetLatestActions()
        {
            if (scriptedController != null && scriptedController.useForHeuristic)
            {
                return scriptedController.LatestActions;
            }

            return manualController != null ? manualController.LatestActions : System.Array.Empty<float>();
        }

        static string Float(float value)
        {
            return value.ToString("0.######", CultureInfo.InvariantCulture);
        }
    }
}
