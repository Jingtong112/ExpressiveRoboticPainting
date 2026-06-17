using ExpressivePainting.Demonstration;
using TMPro;
using UnityEngine;

namespace ExpressivePainting.UI
{
    public class RecordingStatusUI : MonoBehaviour
    {
        public TrajectoryLogger logger;
        public TMP_Text label;

        void Update()
        {
            if (label != null && logger != null)
            {
                label.text = logger.IsRecording ? "Recording" : "Ready";
            }
        }
    }
}
