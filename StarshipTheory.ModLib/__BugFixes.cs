using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib
{
    public static class __BugFixes
    {
        public static void ToggleShipVisual(UnityEngine.GameObject ship, bool visual)
        {
            ship.GetComponent<Structures>().toggleShipVisual(visual);
        }

    }
}
