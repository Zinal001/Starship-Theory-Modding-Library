using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.Area Unity method
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.BeginArea.html</see>
    /// </summary>
    public class Area : GUIItem
    {
        /// <summary>
        /// The position, width and height of this area.
        /// </summary>
        public UnityEngine.Rect Size { get; set; }

        /// <summary>
        /// The children that should be drawn in this area.
        /// </summary>
        public List<GUIItem> Items { get; private set; }

        /// <summary>
        /// Creates a new Area.
        /// </summary>
        /// <param name="size">The position, width and height</param>
        /// <param name="items">Chilren that should be drawn in this area</param>
        public Area(UnityEngine.Rect size, IEnumerable<GUIItem> items = null)
        {
            this.Size = size;
            if (items == null)
                this.Items = new List<GUIItem>();
            else
                this.Items = items.ToList();
        }


        internal override void Draw()
        {
            if(this.Style == null)
                this.Style = UnityEngine.GUI.skin.box;

            if (this.Visible)
            {
                UnityEngine.GUILayout.BeginArea(Size, Style);

                foreach(GUIItem item in Items)
                {
                    if (item.Visible)
                        item.Draw();
                }

                UnityEngine.GUILayout.EndArea();
            }
        }
    }
}
