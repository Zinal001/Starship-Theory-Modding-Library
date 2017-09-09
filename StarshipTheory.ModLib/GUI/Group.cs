using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib.GUI
{
    /// <summary>
    /// A wrapper class for the GUILayout.BeginHorizontal and GUILayout.BeginVertical Unity methods
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.BeginHorizontal.html</see>
    /// <see>https://docs.unity3d.com/ScriptReference/GUILayout.BeginVertical.html</see>
    /// </summary>
    public class Group : GUIItem, IGroupItem
    {
        /// <summary>
        /// The direction of this Group.
        /// </summary>
        public Direction LayoutDirection { get; set; }

        /// <summary>
        /// The children that should be drawn within the group.
        /// </summary>
        public List<GUIItem> Items { get; private set; }

        /// <summary>
        /// Creates a new Group.
        /// </summary>
        /// <param name="layoutDirection">The direction of this group</param>
        /// <param name="items">The children that should be drawn within this group</param>
        public Group(Direction layoutDirection = Direction.Horizontal, IEnumerable<GUIItem> items = null)
        {
            this.LayoutDirection = layoutDirection;
            if (items == null)
                this.Items = new List<GUIItem>();
            else
                this.Items = items.ToList();
        }


        public override void Draw()
        {
            if (this.Visible)
            {
                Direction dir = LayoutDirection;
                GroupDepth++;
                if (Style != null)
                {
                    if (dir == Direction.Horizontal)
                        UnityEngine.GUILayout.BeginHorizontal(Style, this.Options);
                    else
                        UnityEngine.GUILayout.BeginVertical(Style, this.Options);
                }
                else
                {
                    if (dir == Direction.Horizontal)
                        UnityEngine.GUILayout.BeginHorizontal(this.Options);
                    else
                        UnityEngine.GUILayout.BeginVertical(this.Options);
                }

                foreach(GUIItem item in Items)
                {
                    if (item.Visible)
                        item.Draw();
                }

                if (dir == Direction.Horizontal)
                    UnityEngine.GUILayout.EndHorizontal();
                else
                    UnityEngine.GUILayout.EndVertical();
                GroupDepth--;
            }
        }
    }
}
