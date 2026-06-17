using UnityEngine;
using UnityEngine.UI;

namespace ExpressivePainting.Visualisation
{
    public class ResultsGraphManager : MonoBehaviour
    {
        public Image styleABar;
        public Image styleBBar;
        public Image styleCBar;

        public void ShowAccuracy(float styleA, float styleB, float styleC)
        {
            SetBar(styleABar, styleA);
            SetBar(styleBBar, styleB);
            SetBar(styleCBar, styleC);
        }

        static void SetBar(Image bar, float value)
        {
            if (bar == null) return;
            Vector3 scale = bar.rectTransform.localScale;
            scale.y = Mathf.Clamp01(value / 100f);
            bar.rectTransform.localScale = scale;
        }
    }
}
