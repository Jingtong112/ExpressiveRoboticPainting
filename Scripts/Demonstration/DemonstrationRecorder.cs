using ExpressivePainting.Painting;
using UnityEngine;

namespace ExpressivePainting.Demonstration
{
    public class DemonstrationRecorder : MonoBehaviour
    {
        public TrajectoryLogger trajectoryLogger;
        public PaintTrailManager paintTrailManager;

        public void ToggleRecording()
        {
            if (trajectoryLogger.IsRecording)
            {
                string path = trajectoryLogger.StopRecording();
                Debug.Log($"Saved trajectory demonstration: {path}");
            }
            else
            {
                paintTrailManager?.ClearAll();
                trajectoryLogger.StartRecording();
                Debug.Log("Started trajectory demonstration recording.");
            }
        }
    }
}
