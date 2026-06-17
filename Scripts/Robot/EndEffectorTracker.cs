using UnityEngine;

namespace ExpressivePainting.Robot
{
    public class EndEffectorTracker : MonoBehaviour
    {
        public Transform brushTip;
        public Transform canvasOrigin;

        public Vector3 WorldPosition => brushTip.position;
        public Vector3 CanvasPosition => canvasOrigin.InverseTransformPoint(brushTip.position);
    }
}
