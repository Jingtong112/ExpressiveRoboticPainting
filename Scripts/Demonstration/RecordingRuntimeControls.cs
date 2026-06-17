using ExpressivePainting.Painting;
using ExpressivePainting.Robot;
using UnityEngine;
using MlDemoRecorder = Unity.MLAgents.Demonstrations.DemonstrationRecorder;

namespace ExpressivePainting.Demonstration
{
    public class RecordingRuntimeControls : MonoBehaviour
    {
        public DemonstrationRecorder csvRecorder;
        public TrajectoryLogger trajectoryLogger;
        public DemonstrationStyleManager styleManager;
        public ScriptedDemonstrationController scriptedController;
        public ArtisticStyleTestDriver drawingTestDriver;
        public MlDemoRecorder mlAgentsRecorder;

        public bool scriptedMode;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetStyle(PaintingStyle.SmoothCircle);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetStyle(PaintingStyle.SpiralGrowth);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SetStyle(PaintingStyle.OrganicFloral);
            if (Input.GetKeyDown(KeyCode.T)) ToggleScriptedMode();
            if (Input.GetKeyDown(KeyCode.R)) csvRecorder?.ToggleRecording();
            if (Input.GetKeyDown(KeyCode.M)) ToggleMlAgentsRecording();
        }

        public void SetStyle(PaintingStyle style)
        {
            int styleIndex = (int)style;
            styleManager?.SetStyle(styleIndex);
            if (scriptedController != null) scriptedController.SetStyle(styleIndex);
            if (drawingTestDriver != null) drawingTestDriver.style = style;
            if (trajectoryLogger != null)
            {
                trajectoryLogger.style = style;
                trajectoryLogger.outputFolder = StyleFolder(style);
            }
            if (mlAgentsRecorder != null)
            {
                mlAgentsRecorder.DemonstrationName = DemoName(style);
                mlAgentsRecorder.DemonstrationDirectory = MlAgentsFolder(style);
            }
            Debug.Log($"Recording style set to {style}. CSV folder: {trajectoryLogger?.outputFolder}");
        }

        public void ToggleScriptedMode()
        {
            scriptedMode = !scriptedMode;
            if (scriptedController != null)
            {
                scriptedController.useForHeuristic = scriptedMode;
            }
            Debug.Log(scriptedMode ? "Scripted demonstration mode enabled." : "Manual demonstration mode enabled.");
        }

        public void ToggleMlAgentsRecording()
        {
            if (mlAgentsRecorder == null) return;

            if (mlAgentsRecorder.Record)
            {
                mlAgentsRecorder.Record = false;
                mlAgentsRecorder.Close();
                Debug.Log("Stopped ML-Agents .demo recording.");
            }
            else
            {
                mlAgentsRecorder.Record = true;
                Debug.Log($"Started ML-Agents .demo recording: {mlAgentsRecorder.DemonstrationDirectory}/{mlAgentsRecorder.DemonstrationName}.demo");
            }
        }

        static string StyleFolder(PaintingStyle style)
        {
            return style switch
            {
                PaintingStyle.SmoothCircle => "../Data/demonstrations/style_a_circle",
                PaintingStyle.SpiralGrowth => "../Data/demonstrations/style_b_spiral",
                PaintingStyle.OrganicFloral => "../Data/demonstrations/style_c_floral",
                _ => "../Data/demonstrations"
            };
        }

        static string MlAgentsFolder(PaintingStyle style)
        {
            return style switch
            {
                PaintingStyle.SmoothCircle => "Assets/ML-Agents/Demonstrations/StyleA",
                PaintingStyle.SpiralGrowth => "Assets/ML-Agents/Demonstrations/StyleB",
                PaintingStyle.OrganicFloral => "Assets/ML-Agents/Demonstrations/StyleC",
                _ => "Assets/ML-Agents/Demonstrations"
            };
        }

        static string DemoName(PaintingStyle style)
        {
            return style switch
            {
                PaintingStyle.SmoothCircle => "StyleA",
                PaintingStyle.SpiralGrowth => "StyleB",
                PaintingStyle.OrganicFloral => "StyleC",
                _ => "Style"
            };
        }
    }
}
