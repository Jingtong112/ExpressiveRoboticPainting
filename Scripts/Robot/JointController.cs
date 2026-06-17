using UnityEngine;

namespace ExpressivePainting.Robot
{
    public class JointController : MonoBehaviour
    {
        public Vector3 rotationAxis = Vector3.up;
        public float minAngle = -90f;
        public float maxAngle = 90f;
        public float maxDegreesPerSecond = 80f;
        public float smoothing = 10f;

        float currentAngle;
        float currentVelocity;
        float targetVelocity;

        public float NormalizedAngle => Mathf.InverseLerp(minAngle, maxAngle, currentAngle) * 2f - 1f;
        public float NormalizedVelocity => Mathf.Clamp(currentVelocity / maxDegreesPerSecond, -1f, 1f);

        void FixedUpdate()
        {
            currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, 1f - Mathf.Exp(-smoothing * Time.fixedDeltaTime));
            currentAngle = Mathf.Clamp(currentAngle + currentVelocity * Time.fixedDeltaTime, minAngle, maxAngle);
            transform.localRotation = Quaternion.AngleAxis(currentAngle, rotationAxis.normalized);
        }

        public void ApplyNormalizedVelocity(float normalizedVelocity)
        {
            targetVelocity = Mathf.Clamp(normalizedVelocity, -1f, 1f) * maxDegreesPerSecond;
        }

        public void ResetJoint()
        {
            currentAngle = 0f;
            currentVelocity = 0f;
            targetVelocity = 0f;
            transform.localRotation = Quaternion.identity;
        }
    }
}
