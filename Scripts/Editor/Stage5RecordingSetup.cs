using ExpressivePainting.Demonstration;
using ExpressivePainting.Painting;
using ExpressivePainting.Robot;
using Unity.MLAgents.Actuators;
using Unity.MLAgents;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.MLAgents.Policies;
using CsvDemonstrationRecorder = ExpressivePainting.Demonstration.DemonstrationRecorder;
using MlAgentsDemonstrationRecorder = Unity.MLAgents.Demonstrations.DemonstrationRecorder;

namespace ExpressivePainting.EditorTools
{
    public static class Stage5RecordingSetup
    {
        [MenuItem("Expressive Painting/Stage 5/Setup Demonstration Recording")]
        public static void SetupDemonstrationRecording()
        {
            GameObject robotRoot = GameObject.Find("RobotRoot");
            GameObject brushTip = GameObject.Find("BrushTip");
            GameObject canvasPlane = GameObject.Find("CanvasPlane");
            GameObject paintingSystem = GameObject.Find("PaintingSystem");

            if (robotRoot == null || brushTip == null || canvasPlane == null)
            {
                Debug.LogError("Stage 5 setup failed: RobotRoot, BrushTip, or CanvasPlane is missing.");
                return;
            }

            if (paintingSystem == null)
            {
                paintingSystem = new GameObject("PaintingSystem");
            }

            JointController[] joints = robotRoot.GetComponentsInChildren<JointController>();
            RoboticArmAgent agent = robotRoot.GetComponent<RoboticArmAgent>();
            ManualRobotController manual = robotRoot.GetComponent<ManualRobotController>();
            if (agent == null || manual == null)
            {
                Debug.LogError("Stage 5 setup failed: RobotRoot must contain RoboticArmAgent and ManualRobotController.");
                return;
            }

            ScriptedDemonstrationController scripted = robotRoot.GetComponent<ScriptedDemonstrationController>();
            if (scripted == null) scripted = robotRoot.AddComponent<ScriptedDemonstrationController>();
            scripted.useForHeuristic = false;
            scripted.cycleSeconds = 8f;
            scripted.actionScale = 0.85f;
            scripted.joints = joints;
            manual.scriptedController = scripted;
            agent.scriptedDemonstration = scripted;
            agent.includeStyleObservation = true;
            agent.style = PaintingStyle.SmoothCircle;
            ConfigureBehaviorParameters(robotRoot);
            ConfigureDecisionRequester(robotRoot);

            DemonstrationStyleManager styleManager = paintingSystem.GetComponent<DemonstrationStyleManager>();
            if (styleManager == null) styleManager = paintingSystem.AddComponent<DemonstrationStyleManager>();
            styleManager.agent = agent;
            styleManager.currentStyle = PaintingStyle.SmoothCircle;

            TrajectoryLogger logger = paintingSystem.GetComponent<TrajectoryLogger>();
            if (logger == null) logger = paintingSystem.AddComponent<TrajectoryLogger>();
            logger.brushTip = brushTip.transform;
            logger.canvasOrigin = canvasPlane.transform;
            logger.joints = joints;
            logger.manualController = manual;
            logger.scriptedController = scripted;
            logger.style = PaintingStyle.SmoothCircle;
            logger.outputFolder = "../Data/demonstrations/style_a_circle";

            CsvDemonstrationRecorder recorder = paintingSystem.GetComponent<CsvDemonstrationRecorder>();
            if (recorder == null) recorder = paintingSystem.AddComponent<CsvDemonstrationRecorder>();
            recorder.trajectoryLogger = logger;
            recorder.paintTrailManager = paintingSystem.GetComponent<PaintTrailManager>();

            MlAgentsDemonstrationRecorder mlRecorder = EnsureMlAgentsDemoRecorder(robotRoot);

            RecordingRuntimeControls controls = paintingSystem.GetComponent<RecordingRuntimeControls>();
            if (controls == null) controls = paintingSystem.AddComponent<RecordingRuntimeControls>();
            controls.csvRecorder = recorder;
            controls.trajectoryLogger = logger;
            controls.styleManager = styleManager;
            controls.scriptedController = scripted;
            controls.drawingTestDriver = paintingSystem.GetComponent<ArtisticStyleTestDriver>();
            controls.mlAgentsRecorder = mlRecorder;
            controls.scriptedMode = false;
            controls.SetStyle(PaintingStyle.SmoothCircle);

            EditorUtility.SetDirty(robotRoot);
            EditorUtility.SetDirty(paintingSystem);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("Stage 5 demonstration recording setup complete. RobotRoot has Behavior Parameters, Decision Requester, and ML-Agents Demonstration Recorder configured. Play Mode: 1/2/3 style, T scripted/manual, R CSV toggle, M .demo toggle.");
        }

        static void ConfigureBehaviorParameters(GameObject robotRoot)
        {
            BehaviorParameters behavior = robotRoot.GetComponent<BehaviorParameters>();
            if (behavior == null) behavior = robotRoot.AddComponent<BehaviorParameters>();

            behavior.BehaviorName = "RoboticPaintingAgent";
            behavior.BehaviorType = BehaviorType.Default;
            behavior.BrainParameters.VectorObservationSize = 18;
            behavior.BrainParameters.NumStackedVectorObservations = 1;
            behavior.BrainParameters.ActionSpec = ActionSpec.MakeContinuous(4);
        }

        static void ConfigureDecisionRequester(GameObject robotRoot)
        {
            DecisionRequester requester = robotRoot.GetComponent<DecisionRequester>();
            if (requester == null) requester = robotRoot.AddComponent<DecisionRequester>();
            requester.DecisionPeriod = 1;
            requester.TakeActionsBetweenDecisions = true;
        }

        static MlAgentsDemonstrationRecorder EnsureMlAgentsDemoRecorder(GameObject robotRoot)
        {
            MlAgentsDemonstrationRecorder demoRecorder = robotRoot.GetComponent<MlAgentsDemonstrationRecorder>();
            if (demoRecorder == null) demoRecorder = robotRoot.AddComponent<MlAgentsDemonstrationRecorder>();

            demoRecorder.Record = false;
            demoRecorder.NumStepsToRecord = 1200;
            demoRecorder.DemonstrationName = "StyleA";
            demoRecorder.DemonstrationDirectory = "Assets/ML-Agents/Demonstrations/StyleA";
            return demoRecorder;
        }
    }
}
