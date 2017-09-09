using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    public class FlexibleSpace : GUIItem
    {
        public override void Draw()
        {
            if(this.Visible)
            {
                UnityEngine.GUILayout.FlexibleSpace();
            }
        }
    }
}
