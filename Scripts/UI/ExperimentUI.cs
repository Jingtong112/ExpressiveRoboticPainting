using ExpressivePainting.Demonstration;
using ExpressivePainting.Painting;
using ExpressivePainting.Robot;
using TMPro;
using UnityEngine;

namespace ExpressivePainting.UI
{
    public class ExperimentUI : MonoBehaviour
    {
        public DemonstrationRecorder recorder;
        public DemonstrationStyleManager styleManager;
        public PaintTrailManager paintTrailManager;
        public TMP_Text statusText;

        public void SetStyle(int index)
        {
            styleManager.SetStyle(index);
            UpdateStatus($"Style: {(PaintingStyle)index}");
        }

        public void ToggleRecording()
        {
            recorder.ToggleRecording();
            UpdateStatus("Recording toggled");
        }

        public void ClearCanvas()
        {
            paintTrailManager.ClearAll();
            UpdateStatus("Canvas cleared");
        }

        void UpdateStatus(string message)
        {
            if (statusText != null) statusText.text = message;
        }
    }
}
