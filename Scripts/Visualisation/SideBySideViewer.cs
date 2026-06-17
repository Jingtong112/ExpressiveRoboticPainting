using UnityEngine;

namespace ExpressivePainting.Visualisation
{
    public class SideBySideViewer : MonoBehaviour
    {
        public Camera demonstrationCamera;
        public Camera reproductionCamera;

        void Start()
        {
            if (demonstrationCamera != null) demonstrationCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
            if (reproductionCamera != null) reproductionCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
        }
    }
}
