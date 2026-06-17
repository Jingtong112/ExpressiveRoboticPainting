using ExpressivePainting.Robot;
using UnityEngine;

namespace ExpressivePainting.Demonstration
{
    public class ManualRobotController : MonoBehaviour
    {
        public JointController[] joints;
        public float keyboardScale = 1f;
        public ScriptedDemonstrationController scriptedController;
        public float[] LatestActions { get; } = new float[4];

        void Update()
        {
            if (scriptedController != null && scriptedController.useForHeuristic)
            {
                for (int i = 0; i < LatestActions.Length; i++) LatestActions[i] = 0f;
                return;
            }

            LatestActions[0] = Input.GetAxisRaw("Horizontal") * keyboardScale;
            LatestActions[1] = Input.GetAxisRaw("Vertical") * keyboardScale;
            LatestActions[2] = KeyAxis(KeyCode.Q, KeyCode.E) * keyboardScale;
            LatestActions[3] = KeyAxis(KeyCode.Z, KeyCode.C) * keyboardScale;

            for (int i = 0; i < joints.Length && i < LatestActions.Length; i++)
            {
                joints[i].ApplyNormalizedVelocity(LatestActions[i]);
            }
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
