using ExpressivePainting.Painting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ExpressivePainting.EditorTools
{
    public static class Stage4PaintingSetup
    {
        const string StrokePrefabPath = "Assets/Prefabs/StrokeRendererPrefab.prefab";

        [MenuItem("Expressive Painting/Stage 4/Setup Painting System")]
        public static void SetupPaintingSystem()
        {
            GameObject canvasPlane = GameObject.Find("CanvasPlane");
            GameObject brushTip = GameObject.Find("BrushTip");

            if (canvasPlane == null)
            {
                Debug.LogError("Stage 4 setup failed: CanvasPlane was not found in the active scene.");
                return;
            }

            if (brushTip == null)
            {
                Debug.LogError("Stage 4 setup failed: BrushTip was not found in the active scene.");
                return;
            }

            EnsureFolders();
            Material canvasMaterial = EnsureMaterial("CanvasWhite", Color.white);
            Material brushMaterial = EnsureMaterial("BrushBlack", Color.black);

            Renderer canvasRenderer = canvasPlane.GetComponent<Renderer>();
            if (canvasRenderer != null) canvasRenderer.sharedMaterial = canvasMaterial;

            Renderer brushRenderer = brushTip.GetComponent<Renderer>();
            if (brushRenderer != null) brushRenderer.sharedMaterial = brushMaterial;

            StrokeRenderer strokePrefab = EnsureStrokePrefab();

            CanvasPainter painter = canvasPlane.GetComponent<CanvasPainter>();
            if (painter == null) painter = canvasPlane.AddComponent<CanvasPainter>();
            painter.canvasPlane = canvasPlane.transform;
            painter.strokePrefab = strokePrefab;
            painter.minPointDistance = 0.006f;

            BrushTip brush = brushTip.GetComponent<BrushTip>();
            if (brush == null) brush = brushTip.AddComponent<BrushTip>();
            brush.painter = painter;
            brush.contactDistance = 0.035f;
            brush.paintColor = Color.black;
            brush.brushWidth = 0.032f;

            GameObject manager = GameObject.Find("PaintingSystem");
            if (manager == null) manager = new GameObject("PaintingSystem");

            PaintTrailManager trailManager = manager.GetComponent<PaintTrailManager>();
            if (trailManager == null) trailManager = manager.AddComponent<PaintTrailManager>();
            trailManager.canvases = new[] { painter };

            ArtisticStyleTestDriver driver = manager.GetComponent<ArtisticStyleTestDriver>();
            if (driver == null) driver = manager.AddComponent<ArtisticStyleTestDriver>();
            driver.brushTip = brushTip.transform;
            driver.canvasPainter = painter;
            driver.canvasPlane = canvasPlane.transform;
            driver.playOnStart = true;
            driver.durationSeconds = 8f;
            driver.circleRadius = 0.48f;
            driver.spiralStartRadius = 0.06f;
            driver.spiralEndRadius = 0.58f;
            driver.floralBaseRadius = 0.36f;
            driver.floralPetalAmplitude = 0.17f;

            ArtisticStyleTestHotkeys hotkeys = manager.GetComponent<ArtisticStyleTestHotkeys>();
            if (hotkeys == null) hotkeys = manager.AddComponent<ArtisticStyleTestHotkeys>();
            hotkeys.driver = driver;

            Selection.activeObject = manager;
            EditorUtility.SetDirty(canvasPlane);
            EditorUtility.SetDirty(brushTip);
            EditorUtility.SetDirty(manager);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("Stage 4 painting system setup complete. Press Play to auto-test Circle. In Play Mode press 1 Circle, 2 Spiral, 3 Flower.");
        }

        static void EnsureFolders()
        {
            EnsureFolder("Assets", "Prefabs");
            EnsureFolder("Assets", "Materials");
        }

        static void EnsureFolder(string parent, string child)
        {
            string path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        static Material EnsureMaterial(string materialName, Color color)
        {
            string path = $"Assets/Materials/{materialName}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null) return material;

            material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            material.color = color;
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        static StrokeRenderer EnsureStrokePrefab()
        {
            StrokeRenderer existing = AssetDatabase.LoadAssetAtPath<StrokeRenderer>(StrokePrefabPath);
            if (existing != null) return existing;

            GameObject sceneObject = GameObject.Find("StrokeRendererPrefab");
            bool destroyAfterSave = false;
            if (sceneObject == null)
            {
                sceneObject = new GameObject("StrokeRendererPrefab");
                destroyAfterSave = true;
            }

            LineRenderer line = sceneObject.GetComponent<LineRenderer>();
            if (line == null) line = sceneObject.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.startWidth = 0.018f;
            line.endWidth = 0.018f;
            line.numCornerVertices = 6;
            line.numCapVertices = 6;
            line.material = new Material(Shader.Find("Sprites/Default"));

            StrokeRenderer stroke = sceneObject.GetComponent<StrokeRenderer>();
            if (stroke == null) stroke = sceneObject.AddComponent<StrokeRenderer>();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(sceneObject, StrokePrefabPath);
            if (destroyAfterSave) Object.DestroyImmediate(sceneObject);
            return prefab.GetComponent<StrokeRenderer>();
        }
    }
}
