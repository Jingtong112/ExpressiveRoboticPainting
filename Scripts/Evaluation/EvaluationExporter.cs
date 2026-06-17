using System.Globalization;
using System.IO;
using UnityEngine;

namespace ExpressivePainting.Evaluation
{
    public class EvaluationExporter : MonoBehaviour
    {
        public string outputPath = "Data/evaluation_results/unity_evaluation.csv";

        public void Export(string style, EvaluationResult result)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            bool writeHeader = !File.Exists(outputPath);
            using var writer = new StreamWriter(outputPath, append: true);
            if (writeHeader)
            {
                writer.WriteLine("style,mean_trajectory_error,path_similarity,reproduction_accuracy,completion_success");
            }

            writer.WriteLine(string.Join(",",
                style,
                result.MeanTrajectoryError.ToString("0.######", CultureInfo.InvariantCulture),
                result.PathSimilarity.ToString("0.######", CultureInfo.InvariantCulture),
                result.ReproductionAccuracy.ToString("0.######", CultureInfo.InvariantCulture),
                result.CompletionSuccess ? "1" : "0"));
        }
    }
}
