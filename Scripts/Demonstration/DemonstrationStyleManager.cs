using ExpressivePainting.Robot;
using UnityEngine;

namespace ExpressivePainting.Demonstration
{
    public class DemonstrationStyleManager : MonoBehaviour
    {
        public RoboticArmAgent agent;
        public PaintingStyle currentStyle = PaintingStyle.SmoothCircle;

        public void SetStyle(int styleIndex)
        {
            currentStyle = (PaintingStyle)Mathf.Clamp(styleIndex, 0, 2);
            if (agent != null)
            {
                agent.style = currentStyle;
            }
        }
    }
}
