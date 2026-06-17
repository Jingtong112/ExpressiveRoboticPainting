using System.Collections.Generic;
using UnityEngine;

namespace ExpressivePainting.Evaluation
{
    public class TrajectoryEvaluator : MonoBehaviour
    {
        public float canvasDiagonal = 1.4f;
        public float successErrorThreshold = 0.08f;

        public EvaluationResult Evaluate(IReadOnlyList<Vector3> demonstration, IReadOnlyList<Vector3> reproduction)
        {
            float meanError = ErrorMetricCalculator.MeanTrajectoryError(demonstration, reproduction);
            float accuracy = ErrorMetricCalculator.ReproductionAccuracy(meanError, canvasDiagonal);
            float similarity = PathSimilarityCalculator.SimilarityScore(demonstration, reproduction, canvasDiagonal);
            bool success = meanError <= successErrorThreshold;

            return new EvaluationResult(meanError, similarity, accuracy, success);
        }
    }

    public readonly struct EvaluationResult
    {
        public readonly float MeanTrajectoryError;
        public readonly float PathSimilarity;
        public readonly float ReproductionAccuracy;
        public readonly bool CompletionSuccess;

        public EvaluationResult(float meanTrajectoryError, float pathSimilarity, float reproductionAccuracy, bool completionSuccess)
        {
            MeanTrajectoryError = meanTrajectoryError;
            PathSimilarity = pathSimilarity;
            ReproductionAccuracy = reproductionAccuracy;
            CompletionSuccess = completionSuccess;
        }
    }
}
