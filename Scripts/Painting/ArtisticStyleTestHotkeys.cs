using ExpressivePainting.Robot;
using UnityEngine;

namespace ExpressivePainting.Painting
{
    public class ArtisticStyleTestHotkeys : MonoBehaviour
    {
        public ArtisticStyleTestDriver driver;

        void Update()
        {
            if (driver == null) return;

            if (Input.GetKeyDown(KeyCode.Alpha1)) driver.PlayStyle(PaintingStyle.SmoothCircle);
            if (Input.GetKeyDown(KeyCode.Alpha2)) driver.PlayStyle(PaintingStyle.SpiralGrowth);
            if (Input.GetKeyDown(KeyCode.Alpha3)) driver.PlayStyle(PaintingStyle.OrganicFloral);
            if (Input.GetKeyDown(KeyCode.Space)) driver.Stop();
        }
    }
}
