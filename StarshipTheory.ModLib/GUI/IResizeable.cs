using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    public interface IResizeable
    {
        void ResizeStart(UnityEngine.Vector2 mousePosition);

        void ResizeEnd(UnityEngine.Vector2 mousePosition);
    }
}
