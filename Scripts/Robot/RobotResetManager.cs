using UnityEngine;

namespace ExpressivePainting.Robot
{
    public class RobotResetManager : MonoBehaviour
    {
        public JointController[] joints;

        public void ResetRobot()
        {
            foreach (var joint in joints)
            {
                joint.ResetJoint();
            }
        }
    }
}
