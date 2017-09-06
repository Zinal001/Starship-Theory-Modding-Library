using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.Space Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.Space.html</see>
    /// </summary>
    public class Space : GUIItem
    {
        /// <summary>
        /// The width or height of this space (In pixels).
        /// </summary>
        public float Size { get; set; }

        /// <summary>
        /// Creates a new Space.
        /// </summary>
        /// <param name="Size">The width or height of this space (In pixels).</param>
        public Space(float Size)
        {
            this.Size = Size;
        }

        internal override void Draw()
        {
            if (this.Visible)
                UnityEngine.GUILayout.Space(Size);
        }
    }
}
