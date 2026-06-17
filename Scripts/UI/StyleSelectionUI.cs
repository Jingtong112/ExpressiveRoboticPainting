using ExpressivePainting.Demonstration;
using UnityEngine;

namespace ExpressivePainting.UI
{
    public class StyleSelectionUI : MonoBehaviour
    {
        public DemonstrationStyleManager styleManager;

        public void SelectCircle() => styleManager.SetStyle(0);
        public void SelectSpiral() => styleManager.SetStyle(1);
        public void SelectFloral() => styleManager.SetStyle(2);
    }
}
